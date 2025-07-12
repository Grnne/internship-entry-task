namespace TicTacToe.Api.Data.Enums;

public enum CellState
{
    Empty,
    X,
    O
}

public enum GameState
{
    InProgress = 0,
    PlayerXWon = 1,
    PlayerOWon = 2,
    Draw = 3
}

public enum PlayerTurn
{
    PlayerX = 0,
    PlayerO = 1
}