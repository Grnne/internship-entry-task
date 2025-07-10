namespace TicTacToe.Api.Data.Entities;

public class User
{
    public int Id { get; set; } //TODO сделать нормально, guid, passHash, tokens
    public string Name { get; set; }
}