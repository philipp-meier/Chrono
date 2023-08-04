using Chrono.Application.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel(option => option.AddServerHeader = false);

// Add services to the container.
builder.Services.AddApplicationServices(builder.Environment.IsDevelopment());
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebUiServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // https://localhost:7151/swagger/index.html
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseForwardedHeaders();

// Other security headers are appended by the reverse proxy (see OWASP recommendations).
app.UseHsts();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    "default",
    "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
    await initializer.InitializeAsync();
    //await initializer.SeedAsync();
}

app.Run();
