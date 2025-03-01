namespace StainManager.Domain.Common;

public class Result
{
    public bool Success { get; }

    public bool Failure => !Success;

    public string? Error { get; }
    
    public bool HandledError { get; set; }
    
    
    protected Result(
        bool success,
        string? errorMessage = null,
        bool handledError = false)
    {
        Success = success;
        Error = errorMessage;
        HandledError = handledError;
    }



    public static Result Ok()
    {
        return new Result(true);
    }

    public static Result Fail(string errorMessage)
    {
        return new Result(false, errorMessage);
    }

    public static Result<T> Ok<T>(T value)
    {
        return new Result<T>(value, true);
    }

    public static Result<T> Fail<T>(string? errorMessage, bool handledError = false)
    {
        return new Result<T>(default, false, errorMessage, handledError);
    }
    
    public static Result<T> Fail<T>(string errorMessage, T? value)
    {
        return new Result<T>(value, false, errorMessage);
    }

    public static Result<T> FromValue<T>(T? value)
    {
        return value != null ? Ok(value) : Fail<T>("Provided value is null");
    }
}

public class Result<T> : Result
{
    protected internal Result(
        T? value,
        bool success,
        string? errorMessage = null,
        bool handledError = false)
        : base(success, errorMessage, handledError)
    {
        Value = value;
    }

    public T? Value { get; }

    public static implicit operator Result<T>(T value)
    {
        return FromValue(value);
    }

    public static implicit operator T?(Result<T> result)
    {
        return result.Value;
    }
}