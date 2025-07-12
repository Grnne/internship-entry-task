namespace TicTacToe.Api.Data.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    //TODO сделать нормально, guid, passHash, tokens
    //public Guid Id { get; set; }
    //public string Username { get; set; } = string.Empty;
    //public string PasswordHash { get; set; } = string.Empty;
    //public string Role { get; set; } = string.Empty;
    //public string? RefreshToken { get; set; }
    //public DateTime? RefreshTokenExpiryTime { get; set; }
}