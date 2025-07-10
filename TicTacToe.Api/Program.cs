
using Microsoft.EntityFrameworkCore;
using TicTacToe.Api.Data;

namespace TicTacToe.Api;

// TODO think bout: 
// maybe add repos and uow, migrations, signalR, middleware for logging, validation\exception handling,
// authorization with current user service, mode for hot seat\multiplayer or drop all this stuff cus overhead
// still signalR exp maybe useful for study, so added dummies(anyway, need to do signalR implementation)
public class Program
{
    public static void Main(string[] args) 
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddDbContext<TicTacToeDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")
                              ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHealthChecks();
        builder.Services.AddSignalR();

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