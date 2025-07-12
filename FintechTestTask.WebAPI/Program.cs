using FintechTestTask.Application.Options;
using FintechTestTask.Infrastructure;
using FintechTestTask.WebAPI.Extensions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace FintechTestTask.WebAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var configs = builder.Configuration
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json")
            .AddEnvironmentVariables()
            .Build();

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        builder.Services.AddHealthChecks()
            .AddRedis(builder.Configuration.GetConnectionString("Redis"), name: "Redis",
                failureStatus: HealthStatus.Unhealthy)
            .AddNpgSql(builder.Configuration.GetConnectionString("PostgreSQLConnectionString"), name: "PostgreSQL",
                failureStatus: HealthStatus.Unhealthy);
        builder.Services.AddHealthChecksUI().AddInMemoryStorage();

        builder.Services.AddStackExchangeRedisCache(x => x.Configuration = configs.GetConnectionString("Redis"));
        builder.Services.AddControllers();
        builder.Services.Configure<JwtOptions>(builder.Configuration.GetRequiredSection("Jwt"));

        builder.AddDependencies();
        var jwtConfig = builder.Configuration.GetRequiredSection("Jwt").Get<JwtOptions>();
        if (jwtConfig == null)
            throw new NullReferenceException("JWT configuration is missing");


        //Лучше тут делать сразу options который нужно через if чем ебаться еще и с удалением существующего 
        if (builder.Environment.IsEnvironment("Testing"))
        {
            builder.Services.AddDbContext<MainDbContext>(
                options => { options.UseInMemoryDatabase(Guid.NewGuid().ToString()); },
                ServiceLifetime.Singleton);
        }
        else
        {
            builder.Services.AddDbContext<MainDbContext>(options =>
            {
                var connectionStr = configs.GetConnectionString("PostgreSQLConnectionString");

                if (string.IsNullOrEmpty(connectionStr))
                    throw new NullReferenceException("No PostgreSQLConnectionString");

                options
                    .UseNpgsql(connectionStr)
                    .UseLoggerFactory(MainDbContext.CreateLoggerFactory())
                    .EnableSensitiveDataLogging();
            });
            builder.Services.AddAuthentication(auth =>
            {
                var defaultAuthScheme = JwtBearerDefaults.AuthenticationScheme;

                auth.DefaultAuthenticateScheme = defaultAuthScheme;
                auth.DefaultChallengeScheme = defaultAuthScheme;
            }).AddJwtBearer((opts) =>
            {
                opts.RequireHttpsMetadata = false /*TODO*/;

                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = true,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidAlgorithms = new List<string> { jwtConfig.AlgorithmForAccessToken },
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = jwtConfig.GetAccessSymmetricSecurityKey(),

                    ValidateLifetime = true,
                    LifetimeValidator = (notBefore, expires, securityToken, validationParameters) =>
                    {
                        if (expires != null)
                            return expires.Value > DateTime.UtcNow;

                        return false;
                    },
                };
            });
        }

        builder.Services.AddAuthorization();
        builder.Services.AddAutoMapper(exp => { exp.AddProfile(typeof(MainMapperProfile)); });

        builder.Services.AddSwaggerGen(o =>
        {
            o.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            o.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
            {
                Description = "Basic auth added to authorization header",
                Name = "Authorization",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Scheme = "Bearer",
                Type = SecuritySchemeType.Http
            });
            o.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                [new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearerAuth" },
                    Name = "Authorization"
                }] = new List<string>(),
            });
        });

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();

        if (app.Environment.IsProduction())
        {
            app.UseHsts();
            app.UseHttpsRedirection();
        }

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();


        app.ApplyMigration();
        //for the efcore use migrations in a docker container, without this postgres will be without changed tables
        //https://youtube.com/watch?v=WQFx2m5Ub9M


        app.UseHealthChecks("/health", new HealthCheckOptions()
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        app.MapHealthChecksUI(options => { options.UIPath = "/health-ui"; });
        app.MapControllers();

        app.Run();
    }
}