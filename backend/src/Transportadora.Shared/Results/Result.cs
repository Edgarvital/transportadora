namespace Transportadora.Shared.Results;

public sealed class Result<T>
{
    public bool Success { get; private init; }
    public string? ErrorCode { get; private init; }
    public string? ErrorMessage { get; private init; }
    public T? Data { get; private init; }

    public static Result<T> Ok(T data) => new()
    {
        Success = true,
        Data = data
    };

    public static Result<T> Fail(string errorCode, string errorMessage) => new()
    {
        Success = false,
        ErrorCode = errorCode,
        ErrorMessage = errorMessage
    };
}
