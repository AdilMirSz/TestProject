using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TestProject.Shared.Persistence;
using TestProject.UserService.Application.Abstractions;
using TestProject.UserService.Infrastructure.Persistence;
using TestProject.UserService.Infrastructure.Security;

namespace TestProject.UserService.WebApi;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Controllers
        builder.Services.AddControllers();

        // MediatR (сканируем Application сборку)
        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(UserService.Application.Auth.Login.LoginCommand).Assembly));

        // DB
        var cs = builder.Configuration.GetConnectionString("Postgres")
                 ?? builder.Configuration["POSTGRES_CONNECTION"];

        if (string.IsNullOrWhiteSpace(cs))
            throw new InvalidOperationException("Connection string not found.");

        builder.Services.AddDbContext<AppDbContext>(o => o.UseNpgsql(cs));

        // Options
        builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

        // Infrastructure services
        builder.Services.AddScoped<IUserRepository, EfUserRepository>();
        builder.Services.AddSingleton<IPasswordHasher, AspNetPasswordHasher>();
        builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        // Auth (JWT Bearer)
        var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.FromSeconds(10)
                };
            });

        builder.Services.AddAuthorization();

        var app = builder.Build();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        await app.RunAsync();
    }
}
