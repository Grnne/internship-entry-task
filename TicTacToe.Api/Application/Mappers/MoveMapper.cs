using TicTacToe.Api.Application.Models.Dto;
using TicTacToe.Api.Data.Entities;

namespace TicTacToe.Api.Application.Mappers;

public class MoveMapper
{
    public static Move ToEntity(CreateMoveDto dto)
    {
        return new Move
        {
            GameId = dto.GameId,
            PlayerId = dto.PlayerId,
            X = dto.X,
            Y = dto.Y
        };
    }
}