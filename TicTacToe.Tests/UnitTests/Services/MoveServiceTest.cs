using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using TicTacToe.Api.Application.Models;
using TicTacToe.Api.Application.Models.Dto;
using TicTacToe.Api.Application.Services;
using TicTacToe.Api.Application.Services.Interfaces;
using TicTacToe.Api.Data;
using TicTacToe.Api.Data.Entities;
using TicTacToe.Api.Data.Enums;

namespace TicTacToe.Tests.UnitTests.Services;

public class MoveServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly TicTacToeDbContext _context;
    private readonly MoveService _moveService;
    private readonly Mock<IGameService> _mockGameService;

    public MoveServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<TicTacToeDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new TicTacToeDbContext(options);
        _context.Database.EnsureCreated();

        _mockGameService = new Mock<IGameService>();

        _mockGameService
            .Setup(s => s.UpdateAsync(It.IsAny<Game>(), It.IsAny<Move>()))
            .ReturnsAsync(new ResponseWrapper<GameStateDto>(
                new GameStateDto
                {
                    GameState = GameState.InProgress,
                    CurrentTurn = PlayerTurn.PlayerO
                },
                isCreated: false
            ));

        _moveService = new MoveService(_context, _mockGameService.Object);
    }

    public static IEnumerable<object[]> InvalidMoveTestData =>
        new List<object[]>
        {
            // DTO null case
            new object[] { null, new Action<TicTacToeDbContext>(ctx => { }), ErrorViews.InvalidMoveDto },

            // Game not found case
            new object[] { new CreateMoveDto { GameId = 999, PlayerId = 1, X = 0, Y = 0 }, new Action<TicTacToeDbContext>(ctx => { }), ErrorViews.GameNotFound },

            // Player turn mismatch
            new object[]
            {
                new CreateMoveDto { GameId = 1, PlayerId = 2, X = 0, Y = 0 },
                new Action<TicTacToeDbContext>(ctx =>
                {
                    var game = new Game
                    {
                        Id = 1,
                        PlayerXId = 1,
                        PlayerOId = 2,
                        CurrentTurn = PlayerTurn.PlayerX,
                        GameState = GameState.InProgress,
                        BoardHeight = 3,
                        BoardWidth = 3,
                        WinLength = 3,
                        Cells = new List<Cell> { new Cell { X = 0, Y = 0, CellState = CellState.Empty } }
                    };
                    ctx.Games.Add(game);
                    ctx.SaveChanges();
                }),
                ErrorViews.NotPlayersTurn
            },

            // Game not in progress
            new object[]
            {
                new CreateMoveDto { GameId = 2, PlayerId = 1, X = 0, Y = 0 },
                new Action<TicTacToeDbContext>(ctx =>
                {
                    var game = new Game
                    {
                        Id = 2,
                        PlayerXId = 1,
                        PlayerOId = 2,
                        CurrentTurn = PlayerTurn.PlayerX,
                        GameState = GameState.PlayerXWon,
                        BoardHeight = 3,
                        BoardWidth = 3,
                        WinLength = 3,
                        Cells = new List<Cell> { new Cell { X = 0, Y = 0, CellState = CellState.Empty } }
                    };
                    ctx.Games.Add(game);
                    ctx.SaveChanges();
                }),
                ErrorViews.GameNotInProgress
            },

            // Invalid cell (cell not found)
            new object[]
            {
                new CreateMoveDto { GameId = 3, PlayerId = 1, X = 5, Y = 5 },
                new Action<TicTacToeDbContext>(ctx =>
                {
                    var game = new Game
                    {
                        Id = 3,
                        PlayerXId = 1,
                        PlayerOId = 2,
                        CurrentTurn = PlayerTurn.PlayerX,
                        GameState = GameState.InProgress,
                        BoardHeight = 3,
                        BoardWidth = 3,
                        WinLength = 3,
                        Cells = new List<Cell> { new Cell { X = 0, Y = 0, CellState = CellState.Empty } }
                    };
                    ctx.Games.Add(game);
                    ctx.SaveChanges();
                }),
                ErrorViews.InvalidCell
            },

            // Cell occupied
            new object[]
            {
                new CreateMoveDto { GameId = 4, PlayerId = 1, X = 0, Y = 0 },
                new Action<TicTacToeDbContext>(ctx =>
                {
                    var game = new Game
                    {
                        Id = 4,
                        PlayerXId = 1,
                        PlayerOId = 2,
                        CurrentTurn = PlayerTurn.PlayerX,
                        GameState = GameState.InProgress,
                        BoardHeight = 3,
                        BoardWidth = 3,
                        WinLength = 3,
                        Cells = new List<Cell> { new Cell { X = 0, Y = 0, CellState = CellState.X } }
                    };
                    ctx.Games.Add(game);
                    ctx.SaveChanges();
                }),
                ErrorViews.CellOccupied
            }
        };

    [Fact]
    public async Task CreateAsync_ProcessValidMoveSuccessfully()
    {
        // Arrange: create a game with empty cells and current turn PlayerX
        var game = new Game
        {
            PlayerXId = 1,
            PlayerOId = 2,
            CurrentTurn = PlayerTurn.PlayerX,
            GameState = GameState.InProgress,
            BoardHeight = 3,
            BoardWidth = 3,
            WinLength = 3,
            Cells = new List<Cell>()
        };

        for (int y = 0; y < game.BoardHeight; y++)
        {
            for (int x = 0; x < game.BoardWidth; x++)
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

        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        var moveDto = new CreateMoveDto
        {
            GameId = game.Id,
            PlayerId = game.PlayerXId,
            X = 0,
            Y = 0
        };

        // Act
        var result = await _moveService.CreateAsync(moveDto);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Response);
        Assert.Equal(GameState.InProgress, result.Response.GameState);
        Assert.Equal(PlayerTurn.PlayerO, result.Response.CurrentTurn);

        // Verify that the cell was updated
        var updatedCell = await _context.Cells.FirstOrDefaultAsync(c => c.GameId == game.Id && c.X == 0 && c.Y == 0);
        Assert.NotNull(updatedCell);
        Assert.NotEqual(CellState.Empty, updatedCell.CellState);

        // Verify that the move was added
        var moveInDb = await _context.Moves.FirstOrDefaultAsync(m => m.GameId == game.Id && m.X == 0 && m.Y == 0);
        Assert.NotNull(moveInDb);
        Assert.Equal(game.PlayerXId, moveInDb.PlayerId);

        // Verify that UpdateAsync on IGameService was called once
        _mockGameService.Verify(s => s.UpdateAsync(It.IsAny<Game>(), It.IsAny<Move>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_HandlesConcurrentMovesOnSameCell()
    {
        // Arrange
        var game = new Game
        {
            PlayerXId = 1,
            PlayerOId = 2,
            CurrentTurn = PlayerTurn.PlayerX,
            GameState = GameState.InProgress,
            BoardHeight = 3,
            BoardWidth = 3,
            WinLength = 3,
            Cells = []
        };

        for (var y = 0; y < game.BoardHeight; y++)
        {
            for (var x = 0; x < game.BoardWidth; x++)
            {
                game.Cells.Add(new Cell { X = x, Y = y, CellState = CellState.Empty, Game = game });
            }
        }

        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        var moveDto = new CreateMoveDto
        {
            GameId = game.Id,
            PlayerId = game.PlayerXId,
            X = 0,
            Y = 0
        };

        //Act
        var task1 = _moveService.CreateAsync(moveDto);
        var task2 = _moveService.CreateAsync(moveDto);

        var results = await Task.WhenAll(task1, task2);

        //Assert
        Assert.Contains(results, r => r.Success);
        Assert.Contains(results, r => !r.Success && r.Error == ErrorViews.CellOccupied);
    }


    [Theory]
    [MemberData(nameof(InvalidMoveTestData))]
    public async Task CreateAsync_ReturnsExpectedError_ForInvalidMoves(
        CreateMoveDto? dto,
        Action<TicTacToeDbContext> setup,
        Error expectedError)
    {
        // Arrange
        setup(_context);

        // Act
        var result = await _moveService.CreateAsync(dto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(expectedError, result.Error);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Close();
        _connection.Dispose();
    }
}