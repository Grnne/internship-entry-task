using Microsoft.EntityFrameworkCore;
using TicTacToe.Api.Application.Mappers;
using TicTacToe.Api.Application.Models;
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

    public async Task<ResponseWrapper<GameStateDto>> CreateAsync(CreateMoveDto dto)
    {
        if (dto == null)
        {
            return new ResponseWrapper<GameStateDto>(ErrorViews.InvalidMoveDto);
        }

        var moveEntity = MoveMapper.ToEntity(dto);
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var gameEntity = await _context.Games
                .Include(g => g.Cells)
                .FirstOrDefaultAsync(g => g.Id == moveEntity.GameId);

            if (gameEntity == null)
            {
                return new ResponseWrapper<GameStateDto>(ErrorViews.GameNotFound);
            }

            var currentTurn = moveEntity.PlayerId == gameEntity.PlayerXId
                ? PlayerTurn.PlayerX : PlayerTurn.PlayerO;

            if (currentTurn != gameEntity.CurrentTurn)
            {
                return new ResponseWrapper<GameStateDto>(ErrorViews.NotPlayersTurn);
            }

            if (gameEntity.GameState != GameState.InProgress)
            {
                return new ResponseWrapper<GameStateDto>(ErrorViews.GameNotInProgress);
            }

            var destinationCell = gameEntity.Cells.Find(c => c.X == moveEntity.X && c.Y == moveEntity.Y);

            if (destinationCell == null)
            {
                return new ResponseWrapper<GameStateDto>(ErrorViews.InvalidCell);
            }

            if (destinationCell.CellState != CellState.Empty)
            {
                return new ResponseWrapper<GameStateDto>(ErrorViews.CellOccupied);
            }

            if (gameEntity.FilledCellsCount % 3 == 0 && gameEntity.FilledCellsCount > 0 && Random.Shared.Next(0, 10) == 1)
            {
                destinationCell.CellState = currentTurn == PlayerTurn.PlayerX ? CellState.O : CellState.X;
            }
            else
            {
                destinationCell.CellState = currentTurn == PlayerTurn.PlayerX ? CellState.X : CellState.O;
            }

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
