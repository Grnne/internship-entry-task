using TicTacToe.Api.Data.Enums;

namespace TicTacToe.Api.Data.Entities;

public class Move
{
    public long Id { get; set; }
    public int PlayerId { get; set; }

    public int X { get; set; }
    public int Y { get; set; }
    public CellState CellState { get; set; }

    public int GameId { get; set; }
    public Game Game { get; set; } = null!;
}
