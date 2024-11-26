using Client;
using FluentValidation;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddValidatorsFromAssembly(Application.Application.Assembly);
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

ConfigureWasmClient.Instance.ConfigureServices(builder.Services);

await builder.Build().RunAsync();