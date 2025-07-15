namespace TicTacToe.Api.Application.Models;

public static class ErrorViews
{
    // Game logic errors
    public static readonly Error GameNotFound = new(
        StatusCode: 404,
        ErrorCode: "GameNotFound",
        Message: "The specified game does not exist."
    );

    public static readonly Error NotPlayersTurn = new(
        StatusCode: 400,
        ErrorCode: "NotPlayersTurn",
        Message: "It is not the current player's turn."
    );

    public static readonly Error PlayersMustBeDifferent = new(
        StatusCode: 400,
        ErrorCode: "PlayersMustBeDifferent",
        Message: "Игроки X и O не могут быть одинаковыми."
    );

    public static readonly Error GameNotInProgress = new(
        StatusCode: 400,
        ErrorCode: "GameNotInProgress",
        Message: "The game is not currently in progress."
    );

    public static readonly Error InvalidCell = new(
        StatusCode: 400,
        ErrorCode: "InvalidCell",
        Message: "The specified cell coordinates are invalid."
    );

    public static readonly Error CellOccupied = new(
        StatusCode: 400,
        ErrorCode: "CellOccupied",
        Message: "The target cell is already occupied."
    );

    public static readonly Error InvalidMoveCoordinates = new(
        StatusCode: 400,
        ErrorCode: "InvalidMoveCoordinates",
        Message: "The move coordinates are outside the bounds of the game board."
    );

    // DTO validation errors
    public static readonly Error InvalidMoveDto = new(
        StatusCode: 400,
        ErrorCode: "InvalidMoveDto",
        Message: "The provided move data is invalid or incomplete. Please check the input and try again."
    );

    public static readonly Error InvalidPlayerId = new(
        StatusCode: 400,
        ErrorCode: "InvalidPlayerId",
        Message: "Player IDs must be positive integers."
    );

    public static readonly Error InvalidBoardConfiguration = new(
        StatusCode: 400,
        ErrorCode: "InvalidBoardConfiguration",
        Message: "Board height, width and win length must be at least 3."
    );

    //Other
    public static readonly Error DatabaseError = new(
        StatusCode: 500,
        ErrorCode: "DatabaseError",
        Message: "An unexpected error occurred while accessing the database."
    );

    public static readonly Error UnknownError = new(
        StatusCode: 500,
        ErrorCode: "UnknownError",
        Message: "An unexpected error occurred. Please try again later."
    );
}