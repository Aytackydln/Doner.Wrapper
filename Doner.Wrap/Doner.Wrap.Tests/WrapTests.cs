using System.Drawing;
using Xunit;

namespace Doner.Wrap.Tests;

public class WrapTests
{
    private readonly double[] _getOnlyProperty = [1.0, 2.0, 3.0];
    private readonly double[] _setOnlyProperty = [4.0, 5.0, 6.0];
    private readonly double[] _property = [7.0, 8.0, 9.0];

    private readonly TestBehavior _testBehavior;
    private readonly WrappedBehavior _wrappedBehavior;

    public WrapTests()
    {
        _testBehavior = new TestBehavior(_getOnlyProperty, _setOnlyProperty, _property);
        _wrappedBehavior = new WrappedBehavior(_testBehavior);
    }

    [Fact]
    public void TestGetOnlyProperty()
    {
        var result = _wrappedBehavior.GetOnlyProperty;

        Assert.Equal(_getOnlyProperty, result);
    }

    [Fact]
    public void TestSetOnlyProperty()
    {
        var expected = new[] { 10.0, 11.0, 12.0 };

        _wrappedBehavior.SetOnlyProperty = expected;

        Assert.Equal(expected, _testBehavior.SetOnlyProperty);
    }

    [Fact]
    public void TestProperty()
    {
        var before = _wrappedBehavior.Property;
        var expected = new[] { 10.0, 11.0, 12.0 };
        Assert.NotEqual(expected, before);

        _wrappedBehavior.Property = expected;

        Assert.Equal(expected, _testBehavior.Property);
    }

    [Fact]
    public void TestVoid()
    {
        Assert.False(_testBehavior.VoidCalled);

        _wrappedBehavior.Void();

        Assert.True(_testBehavior.VoidCalled);
    }

    [Fact]
    public void TestPrimitiveParam()
    {
        const double expected = 0.5;
        Assert.NotEqual(expected, _testBehavior.PrimitiveParamValue);

        _wrappedBehavior.PrimitiveParam(expected);

        Assert.Equal(expected, _testBehavior.PrimitiveParamValue);
    }

    [Fact]
    public void TestStructParam()
    {
        var expected = Color.FromArgb(255, 0, 0, 0);
        Assert.NotEqual(expected, _testBehavior.StructParamValue);

        _wrappedBehavior.StructParam(expected);

        Assert.Equal(expected, _testBehavior.StructParamValue);
    }

    [Fact]
    public void TestRefParam()
    {
        var expected = new Point(1, 2);
        Assert.NotEqual(expected, _testBehavior.RefParamValue);

        _wrappedBehavior.RefParam(ref expected);

        Assert.Equal(expected, _testBehavior.RefParamValue);
    }

    [Fact]
    public void TestRefReadonlyParam()
    {
        var expected = Color.FromArgb(255, 0, 0, 0);
        Assert.NotEqual(expected, _testBehavior.RefReadonlyParamValue);

        _wrappedBehavior.RefReadonlyParam(in expected);

        Assert.Equal(expected, _testBehavior.RefReadonlyParamValue);
    }

    [Fact]
    public void TestObjectParam()
    {
        var expected = new[] { 1.0, 2.0, 3.0 };
        Assert.NotEqual(expected, _testBehavior.ObjectParamValue);

        _wrappedBehavior.ObjectParam(expected);

        Assert.Equal(expected, _testBehavior.ObjectParamValue);
    }

    [Fact]
    public void TestObjectParamChanged()
    {
        var expected = new[] { 1.0, 2.0, 3.0 };
        Assert.NotEqual(expected, _testBehavior.ObjectParamValue);

        _wrappedBehavior.ObjectParam(expected);
        expected[0] = 4.0;

        Assert.Equal(expected, _testBehavior.ObjectParamValue);
    }

    [Fact]
    public void TestDefaultValuedParam()
    {
        // Default value is 0.5
        const double expected = 0.5;
        Assert.NotEqual(expected, _testBehavior.DefaultValuedParamValue);

        _wrappedBehavior.DefaultValuedParam();

        Assert.Equal(expected, _testBehavior.DefaultValuedParamValue);
    }

    [Fact]
    public void TestDefaultCustomValueParam()
    {
        const double expected = 2.5;
        Assert.NotEqual(expected, _testBehavior.DefaultValuedParamValue);

        _wrappedBehavior.DefaultValuedParam(expected);

        Assert.Equal(expected, _testBehavior.DefaultValuedParamValue);
    }

    [Fact]
    public void TestDefaultDefaultParam()
    {
        const double expected = default;
        Assert.NotEqual(expected, _testBehavior.DefaultDefaultParamValue);

        _wrappedBehavior.DefaultDefaultParam();

        Assert.Equal(expected, _testBehavior.DefaultDefaultParamValue);
    }

    [Fact]
    public void TestDefaultNullParam()
    {
        double[]? expected = null;
        Assert.NotEqual(expected, _testBehavior.DefaultNullParamValue);

        _wrappedBehavior.DefaultNullParam();

        Assert.Equal(expected, _testBehavior.DefaultNullParamValue);
    }

    [Fact]
    public void TestDefaultCustomNullParam()
    {
        var expected = new[] { 1.0, 2.0, 3.0 };
        Assert.NotEqual(expected, _testBehavior.DefaultNullParamValue);

        _wrappedBehavior.DefaultNullParam(expected);

        Assert.Equal(expected, _testBehavior.DefaultNullParamValue);
    }

    [Fact]
    public void TestMultipleParams()
    {
        const int arg1 = 1;
        const double arg2 = 2.0;
        const string arg3 = "3";
        Assert.NotEqual(arg1, _testBehavior.MultipleParamsArg1);
        Assert.NotEqual(arg2, _testBehavior.MultipleParamsArg2);
        Assert.NotEqual(arg3, _testBehavior.MultipleParamsArg3);

        _wrappedBehavior.MultipleParams(arg1, arg2, arg3);

        Assert.Equal(arg1, _testBehavior.MultipleParamsArg1);
        Assert.Equal(arg2, _testBehavior.MultipleParamsArg2);
        Assert.Equal(arg3, _testBehavior.MultipleParamsArg3);
    }

    [Fact]
    public void TestObjectReturn()
    {
        // we expect the object given to the constructor
        var expected = _testBehavior.ObjectReturn();

        var result = _wrappedBehavior.ObjectReturn();

        Assert.Equal(expected, result);
    }

    [Fact]
    public void TestStructReturn()
    {
        // we expect the struct given to the constructor
        var expected = _testBehavior.StructReturn();

        var result = _wrappedBehavior.StructReturn();

        Assert.Equal(expected, result);
    }

    [Fact]
    public void TestRefReturn()
    {
        // we expect the ref given to the constructor
        var expected = _testBehavior.RefReturn();

        ref var result = ref _wrappedBehavior.RefReturn();

        Assert.Equal(expected, result);
    }

    [Fact]
    public void TestRefReadonlyReturn()
    {
        // we expect the ref readonly given to the constructor
        var expected = _testBehavior.RefReadonlyReturn();

        ref readonly var result = ref _wrappedBehavior.RefReadonlyReturn();

        Assert.Equal(expected, result);
    }
}