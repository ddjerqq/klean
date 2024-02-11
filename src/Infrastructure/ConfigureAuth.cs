using System.ComponentModel;
using System.Security.Claims;
using Infrastructure;
using Infrastructure.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(ConfigureAuth))]

namespace Infrastructure;

[EditorBrowsable(EditorBrowsableState.Never)]
public class ConfigureAuth : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.MapInboundClaims = false;
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.Events = Jwt.Events;
                    options.Audience = Jwt.ClaimsAudience;
                    options.ClaimsIssuer = Jwt.ClaimsIssuer;
                    options.TokenValidationParameters = Jwt.TokenValidationParameters;
                });

            services.AddAuthorizationBuilder()
                .AddDefaultPolicy("default", policy => policy.RequireAuthenticatedUser())
                .AddPolicy("is_elon", policy => policy.RequireClaim(ClaimTypes.NameIdentifier, "elon"));

        });
    }
}
