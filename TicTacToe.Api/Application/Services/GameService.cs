using Microsoft.EntityFrameworkCore;
using TicTacToe.Api.Application.Models.Dto;
using TicTacToe.Api.Application.Services.Interfaces;
using TicTacToe.Api.Application.Mappers;
using TicTacToe.Api.Data;
using TicTacToe.Api.Data.Entities;

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
        var entity = GameMapper.ToEntity(dto);
        InitCells(entity);
        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();

        return GameMapper.ToDto(entity);
    }

    public async Task<GameDto?> GetByIdAsync(int id)
    {
        var entity = await _context.Games
            .Include(g => g.Cells)
            .FirstOrDefaultAsync(g => g.Id == id);

        return entity == null ? null : GameMapper.ToDto(entity);
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

    private static void InitCells(Game game)
    {
        var cells = new List<Cell>();

        for (var i = 0; i < game.BoardHeight; i++)
        {
            for (var j = 0; j < game.BoardWidth; j++)
            {
                cells.Add(new Cell
                {
                    X = j,
                    Y = i,
                    Game = game
                });
            }
        }

        game.Cells = cells;
    }
}