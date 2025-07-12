namespace TicTacToe.Api.Application.Models;

public class ResponseWrapper<T>
{
    public bool Success { get; set; }
    public T? Response { get; set; } = default;
    public Error? Error { get; set; } = null;
    public string? ETag { get; set; } = null;
    public bool IsCreated { get; set; }


    public ResponseWrapper(T? response)
    {
        Success = true;
        Response = response;
        IsCreated = true;
    }

    public ResponseWrapper(string etag)
    {
        Success = true;
        IsCreated = false;
        ETag = etag;
    }

    public ResponseWrapper(Error error)
    {
        Success = false;
        Error = error;
        IsCreated = false;
    }
}