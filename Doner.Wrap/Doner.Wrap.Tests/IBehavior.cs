using System.Drawing;

namespace Doner.Wrap.Tests;

public interface IBehavior
{
    double[] GetOnlyProperty { get; }
    double[] SetOnlyProperty { set; }
    double[] Property { get; set; }

    void Void();

    void PrimitiveParam(double layerOpacity);
    void StructParam(Color color);
    void RefParam(ref Point point);
    void RefReadonlyParam(ref readonly Color color);
    void ObjectParam(double[] keys);

    void DefaultValuedParam(double layerOpacity = 0.5);
    void DefaultDefaultParam(double layerOpacity = default);
    void DefaultNullParam(double[]? layerOpacity = null);

    void MultipleParams(int arg1, double arg2, string arg3);


    double[] ObjectReturn();
    Point StructReturn();
    ref Point RefReturn();
    ref readonly Point RefReadonlyReturn();
}