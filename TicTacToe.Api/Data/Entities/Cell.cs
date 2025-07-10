using TicTacToe.Api.Data.Enums;

namespace TicTacToe.Api.Data.Entities;

public class Cell
{
    public int Id { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public CellState CellState { get; set; }

    public int GameId { get; set; }
    public Game Game { get; set; }
}