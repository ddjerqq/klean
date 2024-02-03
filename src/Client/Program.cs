using System.Text.Json;
using System.Text.Json.Serialization;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Client;
using Client.Services;
using Client.Services.Interfaces;
using Domain.Common.Extensions;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddAuthorizationCore();

var webAppDomain = "WEB_APP__DOMAIN".FromEnv()
                   ?? throw new Exception("WEB_APP__DOMAIN is not set in the environment");
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(webAppDomain) });

builder.Services.AddScoped<JwtAuthManager>();
builder.Services.AddScoped<IAuthService, JwtAuthManager>(sp => sp.GetRequiredService<JwtAuthManager>());
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthManager>(sp => sp.GetRequiredService<JwtAuthManager>());

builder.Services.AddBlazoredLocalStorage(o =>
{
    o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

await app.RunAsync();