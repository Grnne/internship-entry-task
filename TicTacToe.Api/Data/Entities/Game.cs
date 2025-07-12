using TicTacToe.Api.Data.Enums;

namespace TicTacToe.Api.Data.Entities;

public class Game
{
    public int Id { get; set; }
    public int PlayerXId { get; set; }
    public int PlayerOId { get; set; }
    public int BoardHeight { get; set; } = 3;
    public int BoardWidth { get; set; } = 3;
    public int WinLength { get; set; } = 3;
    public int FilledCellsCount { get; set; } = 0;

    public PlayerTurn CurrentTurn { get; set; } = PlayerTurn.PlayerX;
    public GameState GameState { get; set; } = GameState.InProgress;

    public List<Cell> Cells { get; set; } = [];
    public List<Move> Moves { get; set; } = [];
}