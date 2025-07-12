namespace TicTacToe.Api.Application.Models;

public class ResponseWrapper<T>
{
    public bool Success { get; set; }
    public T? Response { get; set; } = default;
    public Error? Error { get; set; } = null;


    public ResponseWrapper(T? response)
    {
        Success = true;
        Response = response;
    }

    public ResponseWrapper(Error error)
    {
        Success = false;
        Error = error;
    }
}
