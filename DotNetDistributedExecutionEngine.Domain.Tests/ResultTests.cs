using DistributedExecutionEngine.Domain.Common;

namespace DotNetDistributedExecutionEngine.Domain.Tests;

public sealed class ResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResult()
    {
        var result = Result<string, string>.Success("foo");
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal("foo", result.Value);
    }
    
    [Fact]
    public void Success_MatchShouldReturnCorrectValue()
    {
        var result = Result<string, string>.Success("foo")
            .Match(
                value => value + "bar",
                error => "failed"
            );

        Assert.Equal("foobar", result);
    }
    
    [Fact]
    public void Success_ValueOrShouldReturnCorrectValue()
    {
        var result = Result<string, string>.Success("foo")
            .ValueOr("bar");

        Assert.Equal("foo", result);
    }
    
    [Fact]
    public void Failure_ShouldCreateFailedResult()
    {
        var result = Result<string, string>.Failure("foo");
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.ThrowsAny<Exception>(() => result.Value);
    }
    
    [Fact]
    public void Failure_MatchShouldReturnCorrectValue()
    {
        var result = Result<string, string>.Failure("bad")
            .Match(
                value => value,
                error => error + "!"
            );
        
        Assert.Equal("bad!", result);
    }
    
    [Fact]
    public void Failure_ValueOrShouldReturnCorrectValue()
    {
        var result = Result<string, string>.Failure("foo")
            .ValueOr("bar");

        Assert.Equal("bar", result);
    }
    

}