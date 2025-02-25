namespace StainManager.Application.Common.Models;

public class Result
{
    protected Result(
        bool success,
        string? errorMessage = null)
    {
        Success = success;
        Error = errorMessage;
    }

    public bool Success { get; }

    public bool Failure => !Success;

    public string? Error { get; }


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

    public static Result<T> Fail<T>(string? errorMessage)
    {
        return new Result<T>(default, false, errorMessage);
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
        string? errorMessage = null)
        : base(success, errorMessage)
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