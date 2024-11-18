using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Template.Application.Common.Interfaces.Security;
using Template.Application.Common.Models;
using Template.Domain.Constants;
using Template.Infra.ExternalServices.Google;
using Template.Infra.ExternalServices.SendEmails;
using Template.Infra.ExternalServices.Storage;
using Template.Infra.Persistence;
using Template.Infra.Persistence.Contexts;
using Template.Infra.Persistence.Repositories;
using Template.Infra.Settings.Configurations;

namespace Template.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AdicionarInfra(this IServiceCollection services, IConfiguration config)
    {
        var jwtConfigration = new JwtConfiguration();
        config.GetSection(JwtConfiguration.Key).Bind(jwtConfigration);

        services.AddOptions<JwtConfiguration>()
            .BindConfiguration(JwtConfiguration.Key);

        services.AddScoped<IUser, User>();
        services.AddScoped<ICurrentUser, CurrentUser>();

        services.AddHttpContextAccessor();

        services.AddContext(config);

        services.AddHttpClient();

        services
            .AddDefaultIdentity<ContextUser>(o =>
            {
                o.Password.RequireDigit = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequiredLength = 3;
                o.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<ContextRole>()
            .AddErrorDescriber<IdentityPortugueseMessages>()
            .AddEntityFrameworkStores<Context>();

        services.AddMemoryCache();
        services.AddRepository();
        services.AdicionarSendGrid(config);
        services.AddGoogleAPI(config);
        services.AdicionarStorage(config);

        services.AddTransient<IIdentityService, IdentityService>();
        services.AddScoped<ITokenService, TokenService>();
        AdicionarJwt(services, jwtConfigration);

        return services;
    }

    private static void AdicionarJwt(IServiceCollection services, JwtConfiguration jwtConfigration)
    {
        var key = Encoding.ASCII.GetBytes(jwtConfigration.Secret!);

        services.AddAuthentication(authentication =>
        {
            authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = jwtConfigration.Issuer,
                ValidAudience = jwtConfigration.Audience,
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.CanList, policy => policy.RequireClaim("Permission", Policies.CanList));
            options.AddPolicy(Policies.CanView, policy => policy.RequireClaim("Permission", Policies.CanView));
            options.AddPolicy(Policies.CanCreate, policy => policy.RequireClaim("Permission", Policies.CanCreate));
            options.AddPolicy(Policies.CanEdit, policy => policy.RequireClaim("Permission", Policies.CanEdit));
            options.AddPolicy(Policies.CanDelete, policy => policy.RequireClaim("Permission", Policies.CanDelete));
            options.AddPolicy(Policies.CanPurge, policy => policy.RequireClaim("Permission", Policies.CanPurge));
            options.AddPolicy(Policies.CanArchive, policy => policy.RequireClaim("Permission", Policies.CanArchive));
            options.AddPolicy(Policies.CanRestore, policy => policy.RequireClaim("Permission", Policies.CanRestore));
            options.AddPolicy(Policies.CanApprove, policy => policy.RequireClaim("Permission", Policies.CanApprove));
            options.AddPolicy(Policies.CanReject, policy => policy.RequireClaim("Permission", Policies.CanReject));
            options.AddPolicy(Policies.CanExport, policy => policy.RequireClaim("Permission", Policies.CanExport));
            options.AddPolicy(Policies.CanImport, policy => policy.RequireClaim("Permission", Policies.CanImport));
            options.AddPolicy(Policies.CanManageSettings, policy => policy.RequireClaim("Permission", Policies.CanManageSettings));
            options.AddPolicy(Policies.CanManageUsers, policy => policy.RequireClaim("Permission", Policies.CanManageUsers));
            options.AddPolicy(Policies.CanAssignRoles, policy => policy.RequireClaim("Permission", Policies.CanAssignRoles));
            options.AddPolicy(Policies.CanAssignPolicies, policy => policy.RequireClaim("Permission", Policies.CanAssignPolicies));
            options.AddPolicy(Policies.CanViewReports, policy => policy.RequireClaim("Permission", Policies.CanViewReports));
            options.AddPolicy(Policies.CanGenerateReports, policy => policy.RequireClaim("Permission", Policies.CanGenerateReports));

            options.AddPolicy("AdminAccess", policy => policy.RequireRole(Roles.Admin));
            options.AddPolicy("UserAccess", policy => policy.RequireRole(Roles.User));
        });
    }

    public static IServiceCollection AdicionarSwagger(this IServiceCollection services, IConfiguration configuration, string version)
    {
        services.AddSwaggerGen(delegate (SwaggerGenOptions c)
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Template.API",
                Version = "v" + version,
                Description = string.Format("{0} - App {1} - Env {2}", "Template.API", configuration["Config:Ambiente"], Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
            });

            c.CustomSchemaIds((Type x) => x.FullName);
            string applicationBasePath = PlatformServices.Default.Application.ApplicationBasePath;
            string applicationName = PlatformServices.Default.Application.ApplicationName;
            string text = Path.Combine(applicationBasePath, applicationName + ".xml");
            if (File.Exists(text))
            {
                c.IncludeXmlComments(text);
            }

            OpenApiSecurityScheme jwtSecurityScheme = new OpenApiSecurityScheme
            {
                Description = "JWT Authorization Header.\r\n\r\nExample: (Inform without quotes): 'Bearer TokenJWT'",
                Scheme = "bearer",
                BearerFormat = "JWT",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            };
            c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
            { jwtSecurityScheme, Array.Empty<string>() }
            });
        });
        return services;
    }
}