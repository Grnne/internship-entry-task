using TicTacToe.Api.Application.Models.Dto;
using TicTacToe.Api.Application.Services.Interfaces;
using TicTacToe.Api.Data;

namespace TicTacToe.Api.Application.Services;

public class GameService : IGameService
{
    private readonly TicTacToeDbContext _context;

    public GameService(TicTacToeDbContext context)
    {
        _context = context;
    }

    public Task<GameDto> CreateAsync(GameDto createDto)
    {
        throw new NotImplementedException();
    }

    public Task<GameDto> GetByIdAsync(int gameId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(int gameId, GameDto updateDto)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int gameId)
    {
        throw new NotImplementedException();
    }
}
