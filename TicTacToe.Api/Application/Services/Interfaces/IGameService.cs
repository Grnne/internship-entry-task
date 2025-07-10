using TicTacToe.Api.Application.Models.Dto;

namespace TicTacToe.Api.Application.Services.Interfaces;

public interface IGameService
{
    Task<GameDto?> CreateAsync(CreateGameDto dto);
    Task<GameDto?> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id);
}