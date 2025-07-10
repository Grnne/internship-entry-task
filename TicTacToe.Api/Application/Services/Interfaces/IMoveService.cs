using TicTacToe.Api.Application.Models.Dto;

namespace TicTacToe.Api.Application.Services.Interfaces;

public interface IMoveService
{
    Task<GameStateDto?> CreateAsync(CreateMoveDto dto);
    Task<MoveDto?> GetByIdAsync(int id);
}