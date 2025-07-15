using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using TicTacToe.Api;
using TicTacToe.Api.Application.Models.Dto;
using TicTacToe.Api.Data;

namespace TicTacToe.Tests.IntegrationTests;

public class GameControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly IServiceScope _scope;
    private readonly TicTacToeDbContext _context;
    private IDbContextTransaction? _transaction;

    public GameControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<TicTacToeDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<TicTacToeDbContext>(options =>
                {
                    options.UseNpgsql("Server=localhost;Port=5432;Database=TicTacToeDb_Test;User Id=postgres;Password=postgres;");
                });

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<TicTacToeDbContext>();
                db.Database.Migrate();
            });
        });

        _scope = _factory.Services.CreateScope();
        _context = _scope.ServiceProvider.GetRequiredService<TicTacToeDbContext>();

        _transaction = _context.Database.BeginTransaction();
    }

    public void Dispose()
    {
        _transaction?.Rollback();
        _transaction?.Dispose();

        _context.Dispose();
        _scope.Dispose();
    }

    [Fact]
    public async Task Post_CreateGame_ReturnsCreatedGame()
    {
        var client = _factory.CreateClient();

        var createGameDto = new CreateGameDto
        {
            PlayerXId = 1,
            PlayerOId = 2,
            BoardHeight = 3,
            BoardWidth = 3,
            WinLength = 3
        };

        var response = await client.PostAsJsonAsync("/api/game", createGameDto);

        response.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);

        var gameDto = await response.Content.ReadFromJsonAsync<GameDto>();
        Assert.NotNull(gameDto);
        Assert.Equal(createGameDto.PlayerXId, gameDto.PlayerXId);
        Assert.Equal(createGameDto.PlayerOId, gameDto.PlayerOId);
        Assert.Equal(createGameDto.BoardHeight * createGameDto.BoardWidth, gameDto.Cells.Count);
    }

    [Fact]
    public async Task GetById_ReturnsGame_WhenGameExists()
    {
        var client = _factory.CreateClient();

        var createDto = new CreateGameDto
        {
            PlayerXId = 1,
            PlayerOId = 2,
            BoardHeight = 3,
            BoardWidth = 3,
            WinLength = 3
        };

        var createResponse = await client.PostAsJsonAsync("/api/game", createDto);
        createResponse.EnsureSuccessStatusCode();
        var createdGame = await createResponse.Content.ReadFromJsonAsync<GameDto>();
        Assert.NotNull(createdGame);

        var getResponse = await client.GetAsync($"/api/game/{createdGame.Id}");
        getResponse.EnsureSuccessStatusCode();

        var gameDto = await getResponse.Content.ReadFromJsonAsync<GameDto>();
        Assert.NotNull(gameDto);
        Assert.Equal(createdGame.Id, gameDto.Id);
        Assert.Equal(createDto.PlayerXId, gameDto.PlayerXId);
        Assert.Equal(createDto.PlayerOId, gameDto.PlayerOId);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenGameExists()
    {
        var client = _factory.CreateClient();

        var createDto = new CreateGameDto
        {
            PlayerXId = 1,
            PlayerOId = 2,
            BoardHeight = 3,
            BoardWidth = 3,
            WinLength = 3
        };

        var createResponse = await client.PostAsJsonAsync("/api/game", createDto);
        createResponse.EnsureSuccessStatusCode();
        var createdGame = await createResponse.Content.ReadFromJsonAsync<GameDto>();
        Assert.NotNull(createdGame);

        var deleteResponse = await client.DeleteAsync($"/api/game/{createdGame.Id}");
        Assert.Equal(System.Net.HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await client.GetAsync($"/api/game/{createdGame.Id}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenGameDoesNotExist()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/game/999999");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenGameDoesNotExist()
    {
        var client = _factory.CreateClient();
        var response = await client.DeleteAsync("/api/game/999999");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
