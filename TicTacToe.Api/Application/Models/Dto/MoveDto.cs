namespace TicTacToe.Api.Application.Models.Dto;

public class MoveDto
{
}

public class CreateMoveDto
{
    public int GameId { get; set; }
    public int PlayerId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}