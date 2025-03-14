using System.Security.Claims;
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
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.FromSeconds(5),
                    IssuerSigningKeyResolver = (_, _, kid, _) =>
                    {
                        var jsonAsync = new HttpClient().GetStringAsync(configuration["IdentityProvider:JwksUri"]);
                        jsonAsync.Wait();

                        return JsonWebKeySet.Create(jsonAsync.Result)
                            .GetSigningKeys()
                            .Where(x => x.KeyId == kid)
                            .ToArray();
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
                                   context.User.RootElement.GetString("name");

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
}
