namespace TicTacToe.Api.Application.Models;

public record Error(int StatusCode, string? ErrorCode = null, string? Message = null);

