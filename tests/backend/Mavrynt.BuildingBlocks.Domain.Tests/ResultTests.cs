using Mavrynt.BuildingBlocks.Domain.Results;

namespace Mavrynt.BuildingBlocks.Domain.Tests;

public sealed class ResultTests
{
    [Fact]
    public void Success_Should_Set_Success_Flags()
    {
        var result = Result.Success();
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
    }

    [Fact]
    public void Failure_Should_Set_Failure_Flags_And_Error()
    {
        var error = new Error("E-1", "Failure message");
        var result = Result.Failure(error);

        Assert.True(result.IsFailure);
        Assert.False(result.IsSuccess);
        Assert.Same(error, result.Error);
    }

    [Fact]
    public void Generic_Success_Should_Carry_Value()
    {
        var result = Result.Success(42);
        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void Generic_Failure_Value_Access_Should_Throw()
    {
        var result = Result.Failure<int>(new Error("E-2", "Fail"));
        var ex = Assert.Throws<InvalidOperationException>(() => _ = result.Value);
        Assert.Contains("failed result", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}
