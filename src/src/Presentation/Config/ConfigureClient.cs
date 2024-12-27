using Application;
using Blazored.Modal;
using Blazored.Toast;
using TailwindMerge.Extensions;

namespace Presentation.Config;

public sealed class ConfigureClient : ConfigurationBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorComponents()
            .AddInteractiveServerComponents();

        services.AddTailwindMerge();
        services.AddBlazoredModal();
        services.AddBlazoredToast();
    }
}