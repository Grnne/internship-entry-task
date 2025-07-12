using TicTacToe.Api.Application.Models;
using TicTacToe.Api.Application.Models.Dto;

namespace TicTacToe.Api.Application.Services.Interfaces;

public interface IMoveService
{
    Task<ResponseWrapper<GameStateDto>> CreateAsync(CreateMoveDto dto);
}