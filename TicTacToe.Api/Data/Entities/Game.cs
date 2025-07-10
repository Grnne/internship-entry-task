using TicTacToe.Api.Data.Enums;

namespace TicTacToe.Api.Data.Entities;

public class Game
{
    public int Id { get; set; }
    public int Player1Id { get; set; }
    public int Player2Id { get; set; }
    public int BoardHeight { get; set; } = 3;
    public int BoardWidth { get; set; } = 3;

    public PlayerTurn CurrentTurn { get; set; } = PlayerTurn.None;
    public GameState GameResult { get; set; } = GameState.InProgress;

    public List<Cell> Cells { get; set; } = [];
    public List<Move> Moves { get; set; } = [];
}   