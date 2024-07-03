using System.ComponentModel;
using Application;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Presentation.Auth;

namespace Presentation.Config;


/// <inheritdoc />
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ConfigureBlazor : ConfigurationBase
{
    /// <inheritdoc />
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorComponents()
            .AddInteractiveServerComponents();

        services.AddCascadingAuthenticationState();
        services.AddBlazoredLocalStorage();

        services.AddScoped<AuthenticationStateProvider, HttpContextAuthenticationStateProvider>();
    }
}