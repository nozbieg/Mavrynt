namespace Mavrynt.BuildingBlocks.Domain.Results;

public sealed class Result<TValue> : Result
{
    private readonly TValue? _value;

    internal Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access value of a failed result.");

    public static implicit operator Result<TValue>(TValue value) => Result.Success(value);
    public static implicit operator Result<TValue>(Error error) => Result.Failure<TValue>(error);
}
