using Application;
using Client;

namespace Presentation.Config;

public sealed class ConfigureClient : ConfigurationBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        ConfigureWasmClient.Instance.ConfigureServices(services);
    }
}