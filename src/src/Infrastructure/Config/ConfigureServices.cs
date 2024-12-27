using Application;
using Application.Services;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Config;

public sealed class ConfigureInfrastructure : ConfigurationBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddMemoryCache();
        services.AddHttpContextAccessor();

        services.AddScoped<BrowserInternalizationProvider>();
        services.AddScoped<CookieService>();
        services.AddScoped<IEmailSender, GoogleMailSender>();
        services.AddScoped<IRecaptchaVerifier, GoogleRecaptchaVerifier>();
        services.AddScoped<ICurrentUserAccessor, HttpContextCurrentUserAccessor>();
        services.AddScoped<IUserVerificationTokenGenerator, JwtUserVerificationTokenGenerator>();
    }
}