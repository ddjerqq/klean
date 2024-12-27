#pragma warning disable CS1591
using Application;
using Application.Common;
using Application.Services;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Config;

public sealed class ConfigureAuth : ConfigurationBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddCascadingAuthenticationState();

        services.AddScoped<ILookupNormalizer, LowerInvariantLookupNormalizer>();
        services.AddScoped<IJwtGenerator, JwtGenerator>();
        services.AddScoped<IdentityRevalidatingAuthenticationStateProvider>();
        services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<IdentityRevalidatingAuthenticationStateProvider>());

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.Events = JwtGenerator.Events;
                options.Audience = JwtGenerator.ClaimsAudience;
                options.ClaimsIssuer = JwtGenerator.ClaimsIssuer;
                options.TokenValidationParameters = JwtGenerator.TokenValidationParameters;

                options.TokenValidationParameters.NameClaimType = ClaimsPrincipalExt.IdClaimType;
                options.TokenValidationParameters.RoleClaimType = ClaimsPrincipalExt.RoleClaimType;
            });

        services.AddAuthorizationBuilder()
            .AddDefaultPolicy("default", policy => policy.RequireAuthenticatedUser())
            .AddPolicy("is_elon", policy => policy.RequireClaim(ClaimsPrincipalExt.UsernameClaimType, "elon"));
    }
}