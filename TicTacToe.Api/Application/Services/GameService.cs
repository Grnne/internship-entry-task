using Microsoft.EntityFrameworkCore;
using TicTacToe.Api.Application.Mappers;
using TicTacToe.Api.Application.Models;
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

    public async Task<ResponseWrapper<GameDto>> CreateAsync(CreateGameDto dto)
    {
        if (dto == null)
        {
            return new ResponseWrapper<GameDto>(ErrorViews.InvalidMoveDto);
        }

        var entity = GameMapper.CreateGameDtoToEntity(dto);
        InitCells(entity);
        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();

        return new ResponseWrapper<GameDto>(GameMapper.ToGameDto(entity));
    }

    public async Task<ResponseWrapper<GameDto>> GetByIdAsync(int id)
    {
        var entity = await _context.Games
            .Include(g => g.Cells)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (entity == null)
        {
            return new ResponseWrapper<GameDto>(ErrorViews.GameNotFound);
        }

        return new ResponseWrapper<GameDto>(GameMapper.ToGameDto(entity));
    }

    public async Task<ResponseWrapper<bool>> DeleteAsync(int id)
    {
        var existing = await _context.Games.FindAsync(id);

        if (existing == null)
        {
            return new ResponseWrapper<bool>(ErrorViews.GameNotFound);
        }

        _context.Games.Remove(existing);
        await _context.SaveChangesAsync();

        return new ResponseWrapper<bool>(true);
    }

    public async Task<ResponseWrapper<GameStateDto>> UpdateAsync(Game game, Move move)
    {
        if (move.X < 0 || move.X >= game.BoardWidth || move.Y < 0 || move.Y >= game.BoardHeight)
        {
            return new ResponseWrapper<GameStateDto>(ErrorViews.InvalidMoveCoordinates);
        }

        var byteGrid = new byte[game.BoardWidth, game.BoardHeight];

        foreach (var cell in game.Cells)
        {
            if (cell.CellState == CellState.X)
            {
                byteGrid[cell.X, cell.Y] = 1;
            }
            else if (cell.CellState == CellState.O)
            {
                byteGrid[cell.X, cell.Y] = 2;
            }
        }

        game.GameState = CheckWinAllDirections(byteGrid, move.X, move.Y, game.WinLength);

        if (game.GameState == GameState.InProgress)
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

        return new ResponseWrapper<GameStateDto>(GameMapper.ToGameStateDto(game));
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

    private static GameState CheckWinAllDirections(byte[,] boolGrid, int x, int y, int winLength)
    {
        (int deltaX, int deltaY)[] directions = new (int, int)[]
        {
            (1, 0),
            (0, 1),
            (1, 1),
            (1, -1)
        };

        var width = boolGrid.GetLength(0);
        var height = boolGrid.GetLength(1);

        foreach (var (deltaX, deltaY) in directions)
        {
            var countX = 0;
            var countO = 0;

            var posX = x;
            var posY = y;

            while (posX >= 0 && posX < width && posY >= 0 && posY < height)
            {
                if (boolGrid[posX, posY] == 1)
                {
                    countX++;
                    countO = 0;
                }
                else if (boolGrid[posX, posY] == 2)
                {
                    countO++;
                    countX = 0;
                }
                else
                {
                    break;
                }

                posX += deltaX;
                posY += deltaY;
            }

            if (boolGrid[x, y] == 1)
            {
                countO = 0;
            }
            else if (boolGrid[x, y] == 2)
            {
                countX = 0;
            }

            posX = x - deltaX;
            posY = y - deltaY;

            while (posX >= 0 && posX < width && posY >= 0 && posY < height)
            {
                if (boolGrid[posX, posY] == 1)
                {
                    countX++;
                    countO = 0;
                }
                else if (boolGrid[posX, posY] == 2)
                {
                    countO++;
                    countX = 0;
                }
                else
                {
                    break;
                }

                posX -= deltaX;
                posY -= deltaY;
            }

            if (countX >= winLength)
            {
                return GameState.PlayerXWon;
            }

            if (countO >= winLength)
            {
                return GameState.PlayerOWon;
            }
        }

        return GameState.InProgress;
    }
}