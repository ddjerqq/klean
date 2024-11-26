using System.Text.Json;
using System.Text.Json.Serialization;
using Application;
using Blazored.Modal;
using Blazored.Toast;
using Client.Services;
using Generated;
using TailwindMerge.Extensions;

namespace Client;

public sealed class ConfigureWasmClient : ConfigurationBase
{
    public static readonly ConfigureWasmClient Instance = new();

    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddTailwindMerge();
        services.AddBlazoredModal();
        services.AddBlazoredToast();

        services.AddScoped<ApiService>();
        services.AddScoped(_ => new HttpClient {  BaseAddress = new Uri("https://localhost") });
        services.AddScoped<JsonSerializerOptions>(_ =>
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
    }
}