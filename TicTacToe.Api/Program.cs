using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TicTacToe.Api.Application.Services;
using TicTacToe.Api.Application.Services.Interfaces;
using TicTacToe.Api.Data;

namespace TicTacToe.Api;
// TODO validating, maybe put error handling and validating into middleware, also logging
// TODO think bout: maybe add repos and uow, signalR+authorization(multiplayer and hot seat modes)

public class Program
{
    public static void Main(string[] args)
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

            for (var retry = 0; retry < maxRetries; retry++)
            {
                try
                {
                    db.Database.Migrate();

                    break;
                }
                catch (Exception ex)
                {
                    if (retry == maxRetries - 1)
                    {
                        throw;
                    }

                    Console.WriteLine($"Migration failed. Attempt {retry + 1} of {maxRetries}. Retrying in 5 seconds...");
                    Task.Delay(delay);
                }
            }
        }

        app.UseSwagger();
        app.UseSwaggerUI();

        // Temporary redirect
        app.MapGet("/", context =>
        {
            context.Response.Redirect("/swagger");
            return Task.CompletedTask;
        });

        app.UseHttpsRedirection();

        app.MapControllers();
        app.MapHealthChecks("/health");

        app.Run();
    }
}