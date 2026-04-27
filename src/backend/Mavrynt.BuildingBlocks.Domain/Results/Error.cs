namespace Mavrynt.BuildingBlocks.Domain.Results;

public sealed class Error
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("General.NullValue", "A null value was provided.");

    public Error(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public string Code { get; }
    public string Message { get; }

    public override string ToString() => $"{Code}: {Message}";

    public static implicit operator Result(Error error) => Result.Failure(error);
}
