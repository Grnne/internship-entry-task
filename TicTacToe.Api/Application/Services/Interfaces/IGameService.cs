using TicTacToe.Api.Application.Models.Dto;

namespace TicTacToe.Api.Application.Services.Interfaces;

public interface IGameService
{
    Task<GameDto> CreateAsync(GameDto createDto);
    Task<GameDto> GetByIdAsync(int gameId);
    Task UpdateAsync(int gameId, GameDto updateDto);
    Task DeleteAsync(int gameId);
}