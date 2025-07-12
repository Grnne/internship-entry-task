using Microsoft.EntityFrameworkCore;
using TicTacToe.Api.Application.Mappers;
using TicTacToe.Api.Application.Models.Dto;
using TicTacToe.Api.Application.Services.Interfaces;
using TicTacToe.Api.Data;
using TicTacToe.Api.Data.Entities;
using TicTacToe.Api.Data.Enums;

namespace TicTacToe.Api.Application.Services;

public class GameService : IGameService
{
    private readonly TicTacToeDbContext _context;

    public GameService(TicTacToeDbContext context)
    {
        _context = context;
    }

    public async Task<GameDto?> CreateAsync(CreateGameDto dto)
    {
        var entity = GameMapper.CreateGameDtoToEntity(dto);
        InitCells(entity);
        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();

        return GameMapper.ToGameDto(entity);
    }

    public async Task<GameDto?> GetByIdAsync(int id)
    {
        var entity = await _context.Games
            .Include(g => g.Cells)
            .FirstOrDefaultAsync(g => g.Id == id);

        return entity == null ? null : GameMapper.ToGameDto(entity);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.Games.FindAsync(id);

        if (existing == null)
        {
            return false;
        }

        _context.Games.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<GameStateDto?> UpdateAsync(Game game, Move move)
    {
        //TODO errors
        
        if (move.X < 0 || move.X >= game.BoardWidth || move.Y < 0 || move.Y >= game.BoardHeight)
        {
            return null;
        }

        var boolGrid = new bool[game.BoardWidth, game.BoardHeight];
        var cellState = game.CurrentTurn == PlayerTurn.PlayerX ? CellState.X : CellState.O;

        foreach (var cell in game.Cells.Where(cell => cell.CellState == cellState))
        {
            boolGrid[cell.X, cell.Y] = true;
        }

        if (CheckWinAllDirections(boolGrid, move.X, move.Y, game.WinLength))
        {
            game.GameState = game.CurrentTurn == PlayerTurn.PlayerX
                ? GameState.PlayerXWon : GameState.PlayerOWon;
        }
        else
        {
            game.FilledCellsCount++;

            if (game.FilledCellsCount == game.BoardWidth * game.BoardHeight)
            {
                game.GameState = GameState.Draw;
            }
        }

        if (game.GameState == GameState.InProgress)
        {
            game.CurrentTurn = game.CurrentTurn == PlayerTurn.PlayerX ? PlayerTurn.PlayerO : PlayerTurn.PlayerX;
        }

        await _context.SaveChangesAsync();

        return GameMapper.ToGameStateDto(game);
    }

    private static void InitCells(Game game)
    {
        var cells = new List<Cell>();

        for (var y = 0; y < game.BoardHeight; y++)
        {
            for (var x = 0; x < game.BoardWidth; x++)
            {
                cells.Add(new Cell
                {
                    X = x,
                    Y = y,
                    Game = game
                });
            }
        }

        game.Cells = cells;
    }

    //Уродливый, зато универсальный
    private static bool CheckWinAllDirections(bool[,] boolGrid, int x, int y, int winLength)
    {
        //Making directions, we will add\subtract them from current position
        (int deltaX, int deltaY)[] directions = new (int, int)[]
        {
            (1, 0),
            (0, 1),
            (1, 1),
            (1, -1)
        };

        foreach (var (deltaX, deltaY) in directions)
        {
            var count = 1;
            var width = boolGrid.GetLength(0);
            var height = boolGrid.GetLength(1);

            var posX = x + deltaX;
            var posY = y + deltaY;

            //Going to one direction from our starting point
            while (posX >= 0 && posX < width && posY >= 0 && posY < height && boolGrid[posX, posY])
            {
                count++;
                posX += deltaX;
                posY += deltaY;
            }

            posX = x - deltaX;
            posY = y - deltaY;

            //Going to other direction from our starting point
            while (posX >= 0 && posX < width && posY >= 0 && posY < height && boolGrid[posX, posY])
            {
                count++;
                posX -= deltaX;
                posY -= deltaY;
            }

            if (count >= winLength)
                return true;
        }

        return false;
    }
}