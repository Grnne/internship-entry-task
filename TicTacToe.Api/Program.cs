using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TicTacToe.Api.Application.Services;
using TicTacToe.Api.Application.Services.Interfaces;
using TicTacToe.Api.Data;

namespace TicTacToe.Api;
// TODO readme, docker, testing, validation of wrong users and etc 
// TODO think bout: maybe add repos and uow, migrations, lazy loading
// signalR(multiplayer and hot seat modes), middleware for logging,
// validation\exception handling, authorization with current user service,
// or drop all this stuff cus overhead. Added some dummies\blank stuff
// signalR exp maybe useful for study(anyway, need to do signalR implementation)
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

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.MapHealthChecks("/health");

        app.Run();
    }
}
