using TicTacToe.Api.Application.Models.Dto;
using TicTacToe.Api.Application.Services.Interfaces;
using TicTacToe.Api.Data;

namespace TicTacToe.Api.Application.Services;

public class MoveService : IMoveService
{
    private readonly TicTacToeDbContext _context;

    public MoveService(TicTacToeDbContext context)
    {
        _context = context;
    }

    public Task<GameStateDto?> CreateAsync(CreateMoveDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<MoveDto?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }
}
