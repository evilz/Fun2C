using System;
using Xunit;

namespace Fun2C.Tests;

/*
[Test Strategy]
Make sure each method works on:
* Integer option (value type)
* String option  (reference type)
* None   (0 elements)
*/


public class MaybeTests
{
    private void assertWasNotCalledThunk() => throw new Exception("Thunk should not have been called.");

    [Fact]
    public void Flatten()
    {
        Assert.Equal(new Nothing<int>(), Maybe<Maybe<int>>.Nothing.Flatten<Maybe<int>, int>());   // Nothing
        Assert.Equal(new Nothing<int>(), Maybe<int>.Nothing.ToJust().Flatten<Maybe<int>, int>()); // Just Nothing
        Assert.Equal(1.ToJust(), 1.ToJust().ToJust().Flatten<Maybe<int>, int>()); // Just Nothing
        Assert.Equal("".ToJust(), "".ToJust().ToJust().Flatten<Maybe<string>, string>()); // Just Nothing
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(42)]
    public void FilterSomeIntegerWhenPredicateReturnsTrue(int x)
    {
        var actual = x.ToJust().Where(_ => true);
        var expected = x.ToJust();
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Foo")]
    [InlineData("Bar")]
    public void FilterSomeStringWhenPredicateReturnsTrue(string x)
    {
        var actual = x.ToJust().Where(_ => true);
        var expected = x.ToJust();
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(1337)]
    public void FilterSomeIntegerWhenPredicateReturnsFalse (int x)
    {
        var actual = x.ToJust().Where(_ => false);
        var expected = Maybe<int>.Nothing;
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Ploeh")]
    [InlineData("Fnaah")]
    public void FilterSomeStringWhenPredicateReturnsFalse (string x)
    {
        var actual = x.ToJust().Where(_ => false);
        var expected = Maybe<string>.Nothing;
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void FilterNoneReturnsCorrectResult (bool x)
    {
        var actual = Maybe<object>.Nothing.Where(_ => x);
        var expected = Maybe<object>.Nothing;
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(-2001)]
    public void FilterSomeIntegerWhenPredicateEqualsInput (int x)
    {
        var actual = x.ToJust().Where(n => n == x);
        var expected = x.ToJust();
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData("Xyzz")]
    [InlineData("Sgryt")]
    public void FilterSomeStringWhenPredicateEqualsInput (string x)
    {
        var actual = x.ToJust().Where(s => s == x);
        var expected = x.ToJust();
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(927)]
    public void FilterSomeIntegerWhenPredicateDoesNotEqualsInput (int x)
    {
        var actual = x.ToJust().Where(n => n != x);
        var expected = Maybe<int>.Nothing;
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData("Baz Quux")]
    [InlineData("Corge grault")]
    public void FilterSomeStringWhenPredicateDoesNotEqualsInput (string x)
    {
        var actual = x.ToJust().Where(s => s != x);
        var expected = Maybe<string>.Nothing;
        Assert.Equal(expected, actual);
    }
}