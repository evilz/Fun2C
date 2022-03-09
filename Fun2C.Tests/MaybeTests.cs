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
    public void Flatten ()
    {
        Assert.Equal(new Nothing<int>(), Maybe<Maybe<int>>.Nothing.Flatten<Maybe<int>,int>());   // Nothing
        Assert.Equal(new Nothing<int>(), Maybe<int>.Nothing.ToJust().Flatten<Maybe<int>, int>()); // Just Nothing
        Assert.Equal(1.ToJust(), 1.ToJust().ToJust().Flatten<Maybe<int>, int>()); // Just Nothing
        Assert.Equal("".ToJust(), "".ToJust().ToJust().Flatten<Maybe<string>, string>()); // Just Nothing
    }
}