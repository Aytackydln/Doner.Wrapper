using System.Drawing;

namespace Doner.Wrap.Tests;

public class TestBehavior(double[] getOnlyProperty, double[] setOnlyProperty, double[] property) : IBehavior
{
    public double[] GetOnlyProperty { get; } = getOnlyProperty;
    public double[] SetOnlyProperty { get; set; } = setOnlyProperty;
    public double[] Property { get; set; } = property;

    public bool VoidCalled { get; private set; }
    public void Void()
    {
        VoidCalled = true;
    }

    public double PrimitiveParamValue { get; private set; } = -1.0;
    public void PrimitiveParam(double layerOpacity)
    {
        PrimitiveParamValue = layerOpacity;
    }

    public Color StructParamValue { get; private set; } = Color.Empty;
    public void StructParam(Color color)
    {
        StructParamValue = color;
    }

    public Point RefParamValue;
    public void RefParam(ref Point point)
    {
        RefParamValue = point;
    }

    public Color RefReadonlyParamValue { get; private set; } = Color.Empty;
    public void RefReadonlyParam(ref readonly Color color)
    {
        RefReadonlyParamValue = color;
    }

    public double[] ObjectParamValue { get; private set; } =[];
    public void ObjectParam(double[] keys)
    {
        ObjectParamValue = keys;
    }

    public double DefaultValuedParamValue { get; private set; } = -1.0;
    public void DefaultValuedParam(double layerOpacity = 0.5)
    {
        DefaultValuedParamValue = layerOpacity;
    }

    public double DefaultDefaultParamValue { get; private set; } = -1.0;
    public void DefaultDefaultParam(double layerOpacity = default)
    {
        DefaultDefaultParamValue = layerOpacity;
    }

    public double[]? DefaultNullParamValue { get; private set; } = [];
    public void DefaultNullParam(double[]? layerOpacity = null)
    {
        DefaultNullParamValue = layerOpacity;
    }

    public int MultipleParamsArg1 { get; private set; } = -1;
    public double MultipleParamsArg2 { get; private set; } = -1.0;
    public string MultipleParamsArg3 { get; private set; } = string.Empty;
    public void MultipleParams(int arg1, double arg2, string arg3)
    {
        MultipleParamsArg1 = arg1;
        MultipleParamsArg2 = arg2;
        MultipleParamsArg3 = arg3;
    }

    public double[] ObjectReturnValue { get; } = [0.1, 0.2];
    public double[] ObjectReturn()
    {
        return ObjectReturnValue;
    }

    public Point StructReturnValue { get; }
    public Point StructReturn()
    {
        return StructReturnValue;
    }

    private Point _refReturnValue;
    public Point RefReturnValue => _refReturnValue;
    public ref Point RefReturn()
    {
        return ref _refReturnValue;
    }

    private readonly Point _refReadonlyReturnValue;
    public Point RefReadonlyReturnValue => _refReadonlyReturnValue;
    public ref readonly Point RefReadonlyReturn()
    {
        return ref _refReadonlyReturnValue;
    }
}