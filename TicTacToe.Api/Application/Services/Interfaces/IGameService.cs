using TicTacToe.Api.Application.Models;
using TicTacToe.Api.Application.Models.Dto;
using TicTacToe.Api.Data.Entities;

namespace TicTacToe.Api.Application.Services.Interfaces;

public interface IGameService
{
    Task<ResponseWrapper<GameDto>> CreateAsync(CreateGameDto dto);
    Task<ResponseWrapper<GameDto>> GetByIdAsync(int id);
    Task<ResponseWrapper<bool>> DeleteAsync(int id);
    Task<ResponseWrapper<GameStateDto>> UpdateAsync(Game entity, Move move);
}