using Microsoft.EntityFrameworkCore;
using TicTacToe.Api.Application.Mappers;
using TicTacToe.Api.Application.Models.Dto;
using TicTacToe.Api.Application.Services.Interfaces;
using TicTacToe.Api.Data;
using TicTacToe.Api.Data.Enums;

namespace TicTacToe.Api.Application.Services;

public class MoveService : IMoveService
{
    private readonly TicTacToeDbContext _context;
    private readonly IGameService _gameService;

    public MoveService(TicTacToeDbContext context, IGameService gameService)
    {
        _context = context;
        _gameService = gameService;
    }

    public async Task<GameStateDto?> CreateAsync(CreateMoveDto dto)
    {
        var moveEntity = MoveMapper.ToEntity(dto);

        // Maybe remove transaction if concurrency check with Etag adding will be before service
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var gameEntity = await _context.Games
                .Include(g => g.Cells)
                .FirstOrDefaultAsync(g => g.Id == moveEntity.GameId);

            if (gameEntity == null)
            {
                return null;
            }

            var currentTurn = moveEntity.PlayerId == gameEntity.PlayerXId
                ? PlayerTurn.PlayerX : PlayerTurn.PlayerO;

            if (currentTurn != gameEntity.CurrentTurn)
            {
                return null;
            }

            moveEntity.CurrentTurn = currentTurn;

            if (gameEntity.GameState != GameState.InProgress)
            {
                return null;
            }

            var destinationCell = gameEntity.Cells.Find(c =>
                c.X == moveEntity.X && c.Y == moveEntity.Y);

            if (destinationCell == null)
            {
                return null;
            }

            if (destinationCell.CellState != CellState.Empty)
            {
                return null;
            }

            destinationCell.CellState = currentTurn == PlayerTurn.PlayerX ? CellState.X : CellState.O;

            await _context.Moves.AddAsync(moveEntity);
            await _context.SaveChangesAsync();

            var updatedGameState = await _gameService.UpdateAsync(gameEntity, moveEntity);

            await transaction.CommitAsync();

            return updatedGameState;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

}