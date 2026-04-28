using Mavrynt.BuildingBlocks.Domain.Results;

namespace Mavrynt.BuildingBlocks.Domain.Tests;

public sealed class ErrorTests
{
    [Fact]
    public void Error_Should_Preserve_Code_And_Message()
    {
        var error = new Error("ERR.Code", "ERR message");

        Assert.Equal("ERR.Code", error.Code);
        Assert.Equal("ERR message", error.Message);
    }

    [Fact]
    public void Error_Equality_Is_Reference_Based_For_Distinct_Instances()
    {
        var left = new Error("ERR.Same", "Same message");
        var right = new Error("ERR.Same", "Same message");

        Assert.False(left.Equals(right));
    }
}
