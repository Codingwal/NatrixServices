using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;

namespace NatrixServices.Shared.Core;

public enum ErrorType
{
    None,
    BadRequest,
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
            ErrorType.BadRequest => new BadRequestObjectResult(Message),
            ErrorType.Conflict => new ConflictObjectResult(Message),
            ErrorType.Gone => new ObjectResult(Message) { StatusCode = 410 },
            ErrorType.Validation => new BadRequestObjectResult(Message),
            ErrorType.Unprocessable => new UnprocessableEntityObjectResult(Message),
            ErrorType.Unauthorized => new UnauthorizedObjectResult(Message),
            ErrorType.Forbidden => new ObjectResult(Message) { StatusCode = StatusCodes.Status403Forbidden },
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
    public static Result Failure(ErrorType errorType, string message) => new(new Error(errorType, message));

    public T Match<T>(Func<T> onSuccess, Func<Error, T> onFailure)
    {
        return IsSuccess ? onSuccess() : onFailure(Error);
    }

    public IActionResult ToActionResult()
    {
        return IsSuccess ? new OkResult() : Error.ToActionResult();
    }

    public bool TryGetError([NotNullWhen(true)] out Error? value)
    {
        value = IsFailure ? Error : default;
        return IsFailure;
    }
}

public sealed record Result<TValue> : Result
{
    private readonly TValue? value;
    public TValue Value => IsSuccess ? value! : throw new InvalidOperationException($"Cannot access value of failed Result.");

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

    public static Result<TValue> Success(TValue value) => new(value);
    public static new Result<TValue> Failure(Error error) => new(error);
    public static new Result<TValue> Failure(ErrorType errorType, string message) => new(new Error(errorType, message));

    public T Match<T>(Func<TValue, T> onSuccess, Func<Error, T> onFailure)
        => IsSuccess ? onSuccess(Value) : onFailure(Error);

    public new IActionResult ToActionResult()
        => IsSuccess ? new OkObjectResult(Value) : Error.ToActionResult();

    public bool TryGetValue([NotNullWhen(true)] out TValue? value, [NotNullWhen(false)] out Error? error)
    {
        value = IsSuccess ? Value : default;
        error = IsFailure ? Error : default;
        return IsSuccess;
    }

    public Result Then(Action<TValue> onSuccess)
    {
        if (IsFailure) return Error;
        onSuccess(Value);
        return Success();
    }
    public Result Then(Func<TValue, Result> onSuccess)
        => IsSuccess ? onSuccess(Value) : Error;
    public Result<T> Map<T>(Func<TValue, Result<T>> onSuccess)
        => IsSuccess ? onSuccess(Value) : Error;
}

public static class ResultExtensions
{
    // public T Match<T>(Func<TValue, T> onSuccess, Func<Error, T> onFailure)
    //     => IsSuccess ? onSuccess(Value) : onFailure(Error);

    public static async Task<TOut> Match<TIn, TOut>(this Task<Result<TIn>> resultTask, Func<TIn, TOut> onSuccess, Func<Error, TOut> onFailure)
    {
        var result = await resultTask;
        return result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error);
    }

    public static async Task<TOut> MatchAsync<TIn, TOut>(this Task<Result<TIn>> resultTask, Func<TIn, Task<TOut>> onSuccess, Func<Error, Task<TOut>> onFailure)
    {
        var result = await resultTask;
        return result.IsSuccess ? await onSuccess(result.Value) : await onFailure(result.Error);
    }

    public static async Task<Result> Then<TIn>(this Task<Result<TIn>> resultTask, Action<TIn> onSuccess)
    {
        var result = await resultTask;
        if (result.IsSuccess)
        {
            onSuccess(result.Value);
            return Result.Success();
        }
        return result.Error;
    }

    public static async Task<Result> Then<TIn>(this Task<Result<TIn>> resultTask, Func<TIn, Result> onSuccess)
    {
        var result = await resultTask;
        if (result.IsSuccess)
            return onSuccess(result.Value);

        return result.Error;
    }

    public static async Task<Result<TOut>> Map<TIn, TOut>(this Task<Result<TIn>> resultTask, Func<TIn, TOut> onSuccess)
    {
        var result = await resultTask;
        if (result.IsSuccess)
            return onSuccess(result.Value);

        return result.Error;
    }
    public static async Task<Result<TOut>> Map<TIn, TOut>(this Task<Result<TIn>> resultTask, Func<TIn, Result<TOut>> onSuccess)
    {
        var result = await resultTask;
        if (result.IsSuccess)
            return onSuccess(result.Value);

        return result.Error;
    }

    public static async Task<Result> ThenAsync<TIn>(this Task<Result<TIn>> resultTask, Func<TIn, Task<Result>> onSuccess)
    {
        var result = await resultTask;
        if (result.IsSuccess)
            return await onSuccess(result.Value);

        return result.Error;
    }

    public static async Task<Result<TOut>> MapAsync<TIn, TOut>(this Task<Result<TIn>> resultTask, Func<TIn, Task<TOut>> onSuccess)
    {
        var result = await resultTask;
        if (result.IsSuccess)
            return await onSuccess(result.Value);

        return result.Error;
    }
    public static async Task<Result<TOut>> MapAsync<TIn, TOut>(this Task<Result<TIn>> resultTask, Func<TIn, Task<Result<TOut>>> onSuccess)
    {
        var result = await resultTask;
        if (result.IsSuccess)
            return await onSuccess(result.Value);

        return result.Error;
    }

    public static async Task<IActionResult> ToActionResult(this Task<Result> resultTask)
    {
        var result = await resultTask;
        return result.ToActionResult();
    }

    public static async Task<IActionResult> ToActionResult<TValue>(this Task<Result<TValue>> resultTask)
    {
        var result = await resultTask;
        return result.ToActionResult();
    }
}
