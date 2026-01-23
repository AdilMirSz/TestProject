using System.Text;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TestProject.Shared.Auth;
using TestProject.Shared.Persistence;

namespace TestProject.FinanceService.WebApi;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(TestProject.FinanceService.Application.Rates.GetMyRatesQuery).Assembly));

        var cs = builder.Configuration.GetConnectionString("Postgres")
                 ?? builder.Configuration["POSTGRES_CONNECTION"];

        if (string.IsNullOrWhiteSpace(cs))
            throw new InvalidOperationException("Connection string not found.");

        builder.Services.AddDbContext<AppDbContext>(o => o.UseNpgsql(cs));
        
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
