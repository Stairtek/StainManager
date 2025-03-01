using System.Text.Json.Serialization;

namespace StainManager.Blazor.WebUI.Server.Common.Models;

public class Result
{
    public bool Success { get; set; }

    public bool Failure => !Success;
    
    public string? Error { get; set; }

    public bool HandledError { get; set; }
    
    
    public Result() { }

    public Result(
        bool success,
        string? error = null,
        bool handledError = false)
    {
        Success = success;
        Error = error;
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

    public static Result<T> FromValue<T>(T? value)
    {
        return value != null ? Ok(value) : Fail<T>("Provided value is null");
    }
    
    public string GetErrorMessage(string errorMessage)
    {
        if (Success)
            return string.Empty;

        const string defaultErrorMessage = "Something Unexpected Happened.";

        if (!HandledError)
        {
            return string.IsNullOrEmpty(errorMessage)
                ? defaultErrorMessage
                : errorMessage;
        }
        
        if (string.IsNullOrEmpty(Error) && string.IsNullOrEmpty(errorMessage))
            return defaultErrorMessage;
            
        return string.IsNullOrEmpty(Error) 
            ? errorMessage 
            : Error;
    }
}

public class Result<T> : Result
{
    public T? Value { get; set; }
    
    
    public Result() { }
    
    public Result(
        T? value,
        bool success,
        string? error = null,
        bool handledError = false)
        : base(success, error, handledError)
    {
        Value = value;
    }

    
    

    public static implicit operator Result<T>(T value)
    {
        return FromValue(value);
    }

    public static implicit operator T?(Result<T> result)
    {
        return result.Value;
    }
}