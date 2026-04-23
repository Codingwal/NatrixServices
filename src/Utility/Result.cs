using Microsoft.AspNetCore.Mvc;

namespace NatrixServices;

public enum ErrorType
{
    None,
    NotFound,
    Conflict,
    Gone,
    Validation,
    Unprocessable,
    Unauthorized,
    Forbidden
}

public record Error(ErrorType Type, string Message)
{
    public IActionResult ToActionResult()
    {
        return Type switch
        {
            ErrorType.NotFound => new NotFoundObjectResult(Message),
            ErrorType.Conflict => new ConflictObjectResult(Message),
            ErrorType.Gone => new ObjectResult(Message) { StatusCode = 410 },
            ErrorType.Validation => new BadRequestObjectResult(Message),
            ErrorType.Unprocessable => new UnprocessableEntityObjectResult(Message),
            ErrorType.Unauthorized => new UnauthorizedObjectResult(Message),
            ErrorType.Forbidden => new ForbidResult(Message),
            _ => new ObjectResult("Internal server error") { StatusCode = 500 }
        };
    }
}

public record Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error => error ?? throw new InvalidOperationException("Cannot access error of successful Result.");

    private readonly Error? error;

    protected Result()
    {
        IsSuccess = true;
        error = null;
    }
    protected Result(Error _error)
    {
        IsSuccess = false;
        error = _error;
    }

    public static implicit operator Result(Error error) =>
        new(error);

    public static Result Success() => new();
    public static Result Failure(Error error) => new(error);

    public T Match<T>(Func<T> onSuccess, Func<Error, T> onFailure)
    {
        return IsSuccess ? onSuccess() : onFailure(Error);
    }

    public IActionResult ToActionResult()
    {
        return IsSuccess ? new OkResult() : Error.ToActionResult();
    }
}

public sealed record Result<TValue> : Result
{
    private readonly TValue? value;
    public TValue Value => value ?? throw new InvalidOperationException("Cannot access value of failed Result.");

    private Result(TValue value)
        : base()
    {
        this.value = value;
    }

    private Result(Error error)
        : base(error)
    {
        value = default;
    }

    public static implicit operator Result<TValue>(Error error) =>
        new(error);

    public static implicit operator Result<TValue>(TValue value) =>
        new(value);

    public static Result<TValue> Success(TValue value) =>
    new(value);

    public static new Result<TValue> Failure(Error error) =>
        new(error);

    public T Match<T>(Func<TValue, T> onSuccess, Func<Error, T> onFailure)
    {
        return IsSuccess ? onSuccess(Value) : onFailure(Error);
    }
}
