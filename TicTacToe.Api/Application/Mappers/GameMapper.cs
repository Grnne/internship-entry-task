using System.Linq;
using TicTacToe.Api.Application.Models.Dto;
using TicTacToe.Api.Data.Entities;

namespace TicTacToe.Api.Application.Mappers;

public static class GameMapper
{
    public static GameDto ToDto(Game game)
    {
        return new GameDto
        {
            Id = game.Id,
            PlayerXId = game.PlayerXId,
            PlayerOId = game.PlayerOId,
            BoardHeight = game.BoardHeight,
            BoardWidth = game.BoardWidth,
            CurrentTurn = game.CurrentTurn,
            GameState = game.GameState,
            Cells = game.Cells.Select(c => new CellDto
            {
                X = c.X,
                Y = c.Y,
                CellState = c.CellState
            }).ToList()
        };
    }

    public static Game ToEntity(CreateGameDto dto)
    {
        var entity = new Game
        {
            PlayerXId = dto.PlayerXId,
            PlayerOId = dto.PlayerOId
        };

        if (dto.BoardHeight != null)
        {
            entity.BoardHeight = dto.BoardHeight.Value;
        }

        if (dto.BoardWidth != null)
        {
            entity.BoardWidth = dto.BoardWidth.Value;
        }

        return entity;
    }
}