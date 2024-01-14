using System.Security.Claims;
using System.Text.Json;
using Chrono.Entities;
using Chrono.Shared.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureSecurity
{
    public static void AddWebUiSecurityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                //options.Cookie.SameSite = SameSiteMode.Strict;

                options.Cookie.Name = configuration["IdentityProvider:CookieName"];
                options.Events.OnSigningOut = e => e.HttpContext.RevokeUserRefreshTokenAsync();
            })
            .AddOpenIdConnect(options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                options.Authority = configuration["IdentityProvider:Authority"];
                options.ClientId = configuration["IdentityProvider:ClientId"];
                options.ClientSecret = configuration["IdentityProvider:ClientSecret"];
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.ResponseMode = OpenIdConnectResponseMode.Query;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.RequireHttpsMetadata = true;

                options.Scope.Add("openid");
                options.Scope.Add("email");
                options.Scope.Add("profile");

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = options.Authority,
                    ValidAudience = options.ClientId,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(5),
                    IssuerSigningKeyResolver = (_, _, _, _) =>
                    {
                        var client = new HttpClient();
                        var response = client
                            .GetAsync(configuration["IdentityProvider:JwksUri"])
                            .Result;
                        var responseString = response.Content.ReadAsStringAsync().Result;
                        var keys = JsonSerializer.Deserialize<JwkList>(responseString);

                        return keys.Keys;
                    }
                };

                options.SaveTokens = true;

                options.Events.OnUserInformationReceived = async context =>
                {
                    if (!(context.Principal?.Identity?.IsAuthenticated ?? false))
                    {
                        return;
                    }

                    var db = context.HttpContext.RequestServices.GetRequiredService<IApplicationDbContext>();
                    var userId = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                    var userName = context.Principal?.FindFirstValue(ClaimTypes.Name) ??
                                   context.User?.RootElement.GetString("name");

                    if (userId == null)
                    {
                        context.Fail("user_id was not provided.");
                    }

                    if (userName == null)
                    {
                        context.Fail("user_name was not provided.");
                    }

                    var user = db.Users.FirstOrDefault(x => x.UserId == userId);
                    if (user == null)
                    {
                        db.Users.Add(new User { UserId = userId, Name = userName });
                    }
                    else
                    {
                        if (user.Name != userName)
                        {
                            user.Name = userName;
                        }
                    }

                    await db.SaveChangesAsync(context.HttpContext.RequestAborted);
                };
            });

        // Ensures endpoints are secured by default.
        services.AddAuthorizationBuilder()
            .SetFallbackPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build());
    }

    private class JwkList
    {
        public List<JsonWebKey> Keys { get; set; }
    }
}
