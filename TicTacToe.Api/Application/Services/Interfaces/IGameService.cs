using TicTacToe.Api.Application.Models.Dto;
using TicTacToe.Api.Data.Entities;
using TicTacToe.Api.Data.Enums;

namespace TicTacToe.Api.Application.Services.Interfaces;

public interface IGameService
{
    Task<GameDto?> CreateAsync(CreateGameDto dto);
    Task<GameDto?> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id);
    Task<GameStateDto?> UpdateAsync(Game entity, Move move);
}