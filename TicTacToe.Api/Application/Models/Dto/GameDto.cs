using TicTacToe.Api.Data.Enums;

namespace TicTacToe.Api.Application.Models.Dto;

public class CreateGameDto
{
    public int PlayerXId { get; set; }
    public int PlayerOId { get; set; }
    public int? BoardHeight { get; set; }
    public int? BoardWidth { get; set; }
}

public class GameDto
{
    public int Id { get; set; }
    public int PlayerXId { get; set; }
    public int PlayerOId { get; set; }
    public int BoardHeight { get; set; }
    public int BoardWidth { get; set; }
    public PlayerTurn CurrentTurn { get; set; }
    public GameState GameState { get; set; }

    public List<CellDto> Cells { get; set; } = [];
}

public class CellDto
{
    public int X { get; set; }
    public int Y { get; set; }
    public CellState CellState { get; set; }
}

public class GameStateDto
{
    public GameState GameState { get; set; }
    public PlayerTurn PlayerTurn { get; set; }

    public List<CellDto> Cells { get; set; } = [];
}

