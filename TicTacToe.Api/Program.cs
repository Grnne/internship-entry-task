using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TicTacToe.Api.Application.Services;
using TicTacToe.Api.Application.Services.Interfaces;
using TicTacToe.Api.Data;

namespace TicTacToe.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        Env.Load();
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddDbContext<TicTacToeDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")
                              ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "TicTacToe API",
                Version = "v1",
                Description = "REST API для игры в крестики-нолики"
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        });

        builder.Services.AddHealthChecks();
        builder.Services.AddScoped<IGameService, GameService>();
        builder.Services.AddScoped<IMoveService, MoveService>();
        builder.Services.AddScoped<IAuthService, AuthService>();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<TicTacToeDbContext>();

            const int maxRetries = 5;
            const int delay = 5000;

            var retry = 0;
            while (true)
            {
                try
                {
                    await db.Database.MigrateAsync();
                    Console.WriteLine("Database migration successful.");
                    break;
                }
                catch (Exception ex)
                {
                    retry++;
                    if (retry >= maxRetries)
                    {
                        Console.WriteLine($"Migration failed after {retry} attempts. Exception: {ex}");
                        throw;
                    }

                    Console.WriteLine($"Migration failed. Attempt {retry} of {maxRetries}. Retrying in {delay / 1000} seconds...");
                    await Task.Delay(delay);
                }
            }
        }

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "TicTacToe API V1");
        });

        // Temporary redirect "/" → "/swagger"
        app.MapGet("/", context =>
        {
            context.Response.Redirect("/swagger");
            return Task.CompletedTask;
        });

        app.UseHttpsRedirection();

        app.MapControllers();
        app.MapHealthChecks("/health");

        await app.RunAsync();
    }
}