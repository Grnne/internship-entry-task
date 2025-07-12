using TicTacToe.Api.Application.Models.Dto;
using TicTacToe.Api.Data.Entities;

namespace TicTacToe.Api.Application.Mappers;

public static class GameMapper
{
    public static GameDto ToGameDto(Game game)
    {
        return new GameDto
        {
            Id = game.Id,
            PlayerXId = game.PlayerXId,
            PlayerOId = game.PlayerOId,
            BoardHeight = game.BoardHeight,
            BoardWidth = game.BoardWidth,
            WinLength = game.WinLength,
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

    public static GameStateDto ToGameStateDto(Game game)
    {
        return new GameStateDto
        {
            GameId = game.Id,
            GameState = game.GameState,
            CurrentTurn = game.CurrentTurn,
            Cells = game.Cells.Select(c => new CellDto
            {
                X = c.X,
                Y = c.Y,
                CellState = c.CellState
            }).ToList()
        };
    }

    public static Game CreateGameDtoToEntity(CreateGameDto dto)
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

        if (dto.WinLength != null)
        {
            entity.WinLength = dto.WinLength.Value;
        }

        return entity;
    }
}