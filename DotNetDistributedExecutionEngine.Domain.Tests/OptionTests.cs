namespace DotNetDistributedExecutionEngine.Domain.Tests;

using DistributedExecutionEngine.Domain.Common;

public sealed class OptionTests
{
    [Fact]
    public void Some_ShouldCreateOptionWithValue()
    {
        var option = Option<string>.Some("foo");

        Assert.True(option.IsSome);
        Assert.False(option.IsNone);
        Assert.Equal("foo", option.Value);
    }

    [Fact]
    public void Some_WhenValueIsNull_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Option<string>.Some(null!));
    }

    [Fact]
    public void None_ShouldCreateOptionWithoutValue()
    {
        var option = Option<string>.None;

        Assert.False(option.IsSome);
        Assert.True(option.IsNone);
    }

    [Fact]
    public void None_WhenAccessingValue_ShouldThrow()
    {
        var option = Option<string>.None;

        Assert.Throws<InvalidOperationException>(() => option.Value);
    }

    [Fact]
    public void Some_Map_ShouldTransformValue()
    {
        var option = Option<string>
            .Some("foo")
            .Map(value => value.Length);

        Assert.True(option.IsSome);
        Assert.Equal(3, option.Value);
    }

    [Fact]
    public void None_Map_ShouldRemainNone()
    {
        var option = Option<string>
            .None
            .Map(value => value.Length);

        Assert.True(option.IsNone);
    }

    [Fact]
    public void Map_WhenMapperIsNull_ShouldThrow()
    {
        var option = Option<string>.Some("foo");

        Assert.Throws<ArgumentNullException>(() =>
            option.Map<string>(null!));
    }

    [Fact]
    public void Some_ValueOr_ShouldReturnValue()
    {
        var option = Option<string>.Some("foo");

        var value = option.ValueOr("fallback");

        Assert.Equal("foo", value);
    }

    [Fact]
    public void None_ValueOr_ShouldReturnDefaultValue()
    {
        var option = Option<string>.None;

        var value = option.ValueOr("fallback");

        Assert.Equal("fallback", value);
    }

    [Fact]
    public void Some_ShouldWorkWithValueTypes()
    {
        var option = Option<int>.Some(42);

        Assert.True(option.IsSome);
        Assert.Equal(42, option.Value);
    }

    [Fact]
    public void None_ShouldWorkWithValueTypes()
    {
        var option = Option<int>.None;

        Assert.True(option.IsNone);
        Assert.Throws<InvalidOperationException>(() => option.Value);
    }
}