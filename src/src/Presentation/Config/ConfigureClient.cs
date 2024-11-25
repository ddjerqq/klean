using System.Text.Json;
using System.Text.Json.Serialization;
using Application;
using Blazored.Modal;
using Blazored.Toast;
using Client.Services;
using Domain.Common;
using Generated;
using TailwindMerge.Extensions;

namespace Presentation.Config;

public sealed class ConfigureClient : ConfigurationBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        // TODO this exists in two places...
        // TODO change this to only be in one place. one central config maybe???
        // imported from Client
        services.AddTailwindMerge();
        services.AddBlazoredModal();
        services.AddBlazoredToast();

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

        services.AddScoped(_ => new HttpClient { BaseAddress = new Uri("https://localhost") });
        services.AddScoped<ApiService>();
    }
}