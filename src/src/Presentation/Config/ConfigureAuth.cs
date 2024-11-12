using Application;
using Domain.Aggregates;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Presentation.Config;

/// <inheritdoc />
public sealed class ConfigureAuth : ConfigurationBase
{
    /// <inheritdoc />
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.Events = JwtGenerator.Events;
                options.Audience = JwtGenerator.ClaimsAudience;
                options.ClaimsIssuer = JwtGenerator.ClaimsIssuer;
                options.TokenValidationParameters = JwtGenerator.TokenValidationParameters;

                // for role based authz
                options.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
                options.TokenValidationParameters.RoleClaimType = nameof(User.Role);
            });

        // an example, of an authorization policy
        services.AddAuthorizationBuilder()
            .AddDefaultPolicy("default", policy => policy.RequireAuthenticatedUser())
            .AddPolicy("is_elon", policy => policy.RequireClaim(JwtRegisteredClaimNames.Name, "elon"));
    }
}