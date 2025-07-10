using TicTacToe.Api.Application.Models.Dto;

namespace TicTacToe.Api.Application.Services.Interfaces;

public interface IAuthService
{
    Task<UserDto> CreateAsync(UserDto createDto);
    Task<UserDto> GetByIdAsync(int id);
    Task<UserDto> UpdateAsync(UserDto updateDto);
    Task DeleteAsync(long gameId);
}
