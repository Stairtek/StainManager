using System.Text.Json.Serialization;

namespace StainManager.Blazor.WebUI.Server.Common.Models;

public class Result
{
    public Result() { }

    public Result(
        bool success,
        string? error = null)
    {
        Success = success;
        Error = error;
    }

    public bool Success { get; set; }

    public bool Failure => !Success;
    
    public string? Error { get; set; }


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
    public Result() { }
    
    public Result(
        T? value,
        bool success,
        string? error = null)
        : base(success, error)
    {
        Value = value;
    }

    public T? Value { get; set; }

    public static implicit operator Result<T>(T value)
    {
        return FromValue(value);
    }

    public static implicit operator T?(Result<T> result)
    {
        return result.Value;
    }
}