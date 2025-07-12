using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TicTacToe.Api.Application.Models.Dto;
using TicTacToe.Api.Application.Services;
using TicTacToe.Api.Application.Services.Interfaces;
using TicTacToe.Api.Data;
using TicTacToe.Api.Data.Entities;
using TicTacToe.Api.Data.Enums;

namespace TicTacToe.Tests.UnitTests.Services;

public class GameServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<TicTacToeDbContext> _options;

    public GameServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<TicTacToeDbContext>()
            .UseSqlite(_connection)
            .Options;
    }

    private async Task<(TicTacToeDbContext context, IGameService service)> DbInit()
    {
        var context = new TicTacToeDbContext(_options);
        await context.Database.EnsureCreatedAsync();

        IGameService service = new GameService(context);

        return (context, service);
    }

    [Fact]
    public async Task CreateAsync_CreatesGame_WhenValidDto()
    {
        var (context, service) = await DbInit();
        var dto = new CreateGameDto
        {
            PlayerXId = 1,
            PlayerOId = 2,
            BoardHeight = 3,
            BoardWidth = 3,
            WinLength = 3
        };

        var result = await service.CreateAsync(dto);

        Assert.True(result.Success);
        Assert.NotNull(result.Response);
        Assert.Equal(dto.PlayerXId, result.Response.PlayerXId);
        Assert.Equal(dto.PlayerOId, result.Response.PlayerOId);
        Assert.Equal(9, result.Response.Cells.Count);
        Assert.True(result.IsCreated);

        await context.DisposeAsync();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsGame_WhenGameExists()
    {
        var (context, service) = await DbInit();

        var game = new Game
        {
            PlayerXId = 1,
            PlayerOId = 2,
            BoardHeight = 3,
            BoardWidth = 3,
            WinLength = 3,
            Cells = new List<Cell>(),
            Moves = new List<Move>()
        };

        context.Games.Add(game);
        await context.SaveChangesAsync();

        var result = await service.GetByIdAsync(game.Id);

        Assert.True(result.Success);
        Assert.NotNull(result.Response);
        Assert.Equal(game.Id, result.Response.Id);
        Assert.False(result.IsCreated);

        await context.DisposeAsync();
    }

    [Fact]
    public async Task DeleteAsync_ReturnTrue_WhenGameExists()
    {
        var (context, service) = await DbInit();

        var game = new Game
        {
            PlayerXId = 1,
            PlayerOId = 2,
            BoardHeight = 3,
            BoardWidth = 3,
            WinLength = 3,
            Cells = new List<Cell>(),
            Moves = new List<Move>()
        };

        context.Games.Add(game);
        await context.SaveChangesAsync();

        var result = await service.DeleteAsync(game.Id);

        Assert.True(result.Success);
        Assert.True(result.Response);

        await context.DisposeAsync();
    }

    [Fact]
    public async Task DeleteAsync_ReturnError_WhenGameDoesNotExist()
    {
        var (context, service) = await DbInit();

        var result = await service.DeleteAsync(-1);

        Assert.False(result.Success);
        Assert.NotNull(result.Error);

        await context.DisposeAsync();
    }

    [Fact]
    public async Task UpdateAsync_UpdatesGameState()
    {
        var (context, service) = await DbInit();

        var game = new Game
        {
            PlayerXId = 1,
            PlayerOId = 2,
            BoardHeight = 3,
            BoardWidth = 3,
            WinLength = 3,
            CurrentTurn = PlayerTurn.PlayerX,
            GameState = GameState.InProgress,
            FilledCellsCount = 0,
            Cells = new List<Cell>()
        };

        for (var y = 0; y < game.BoardHeight; y++)
        {
            for (var x = 0; x < game.BoardWidth; x++)
            {
                game.Cells.Add(new Cell
                {
                    X = x,
                    Y = y,
                    CellState = CellState.Empty,
                    Game = game
                });
            }
        }

        context.Games.Add(game);
        await context.SaveChangesAsync();

        var move = new Move
        {
            GameId = game.Id,
            PlayerId = game.PlayerXId,
            CurrentTurn = PlayerTurn.PlayerX,
            X = 0,
            Y = 0
        };

        var result = await service.UpdateAsync(game, move);

        Assert.True(result.Success);
        Assert.NotNull(result.Response);
        Assert.Equal(GameState.InProgress, result.Response.GameState);
        Assert.Equal(PlayerTurn.PlayerO, result.Response.CurrentTurn);
        Assert.Equal(1, game.FilledCellsCount);

        await context.DisposeAsync();
    }

    public void Dispose()
    {
        _connection.Close();
        _connection.Dispose();
    }
}
