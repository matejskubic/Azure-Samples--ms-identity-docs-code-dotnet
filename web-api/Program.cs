// <ms_docref_import_types>
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Identity.Web;
// </ms_docref_import_types>

// <ms_docref_add_msal>
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
builder.Services.AddAuthorization(config =>
{
    config.AddPolicy("DelegatedManage", policyBuilder =>
        policyBuilder.Requirements.Add(new ScopeAuthorizationRequirement() { RequiredScopesConfigurationKey = $"AzureAd:Scopes" }));
    config.AddPolicy("ApplicationManage", policyBuilder =>
        policyBuilder.Requirements.Add(new RolesAuthorizationRequirement(new[] { "Artifact.Manage" })));
});
// </ms_docref_add_msal>

// <ms_docref_enable_authz_capabilities>
WebApplication app = builder.Build();
// app.UseDeveloperExceptionPage();
app.UseAuthentication();
app.UseAuthorization();
// </ms_docref_enable_authz_capabilities>

var weatherSummaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

// <ms_docref_protect_endpoint>
app.MapGet("/weatherforecast", [Authorize(Policy = "ApplicationManage"), Authorize(Policy = "ApplicationManage")] () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
       new WeatherForecast
       (
           DateTime.Now.AddDays(index),
           Random.Shared.Next(-20, 55),
           weatherSummaries[Random.Shared.Next(weatherSummaries.Length)]
       ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");
// </ms_docref_protect_endpoint>

app.Run();

record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
