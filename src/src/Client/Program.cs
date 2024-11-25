using System.Text.Json;
using System.Text.Json.Serialization;
using Blazored.Modal;
using Blazored.Toast;
using Client;
using Client.Services;
using FluentValidation;
using Generated;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TailwindMerge.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddValidatorsFromAssembly(Application.Application.Assembly);

builder.Services.AddTailwindMerge();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

builder.Services.AddBlazoredModal();
builder.Services.AddBlazoredToast();

builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped(_ => new HttpClient {  BaseAddress = new Uri("https://localhost") });
builder.Services.AddScoped<JsonSerializerOptions>(_ =>
{
    var jsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        Converters = { new JsonStringEnumConverter() },
    };

    jsonOptions.Converters.ConfigureGeneratedConverters();

    return jsonOptions;
});

await builder.Build().RunAsync();