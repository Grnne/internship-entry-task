using TicTacToe.Api.Application.Models.Dto;
using TicTacToe.Api.Application.Services.Interfaces;
using TicTacToe.Api.Data;

namespace TicTacToe.Api.Application.Services;

public class AuthService : IAuthService
{
    private readonly TicTacToeDbContext _context;

    public AuthService(TicTacToeDbContext context)
    {
        _context = context;
    }

    public Task<UserDto> CreateAsync(UserDto createDto)
    {
        throw new NotImplementedException();
    }

    public Task<UserDto> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<UserDto> UpdateAsync(UserDto updateDto)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(long gameId)
    {
        throw new NotImplementedException();
    }
}