using TicTacToe.Api.Application.Models.Dto;

namespace TicTacToe.Api.Application.Services.Interfaces;

public interface IMoveService
{
    Task<MoveDto> CreateAsync(MoveDto createDto);
    Task<MoveDto> GetByIdAsync(int id);
    Task DeleteAsync(long gameId);
}
