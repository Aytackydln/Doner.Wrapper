namespace Doner.Wrap.Tests.Unsupported;

public interface IGenericBehavior<T>
{
    T Value { get; }
    
    void SetValue(T value);
    
    void NonGenericMethod();
    
    void GenericMethod<U>(U value);
}

public class GenericBehaviorPass<T> : IGenericBehavior<T>
{
    public T Value { get; private set; }
    
    public void SetValue(T value)
    {
        Value = value;
    }

    public void NonGenericMethod() { }

    public void GenericMethod<U>(U value) { }
}

public class GenericBehaviorDefined : IGenericBehavior<int>
{
    public int Value { get; private set; }
    
    public void SetValue(int value)
    {
        Value = value;
    }

    public void NonGenericMethod() { }

    public void GenericMethod<U>(U value) { }
}

[WrapTo(nameof(_behavior))]
public partial class PassWrappedBehavior<T> : IGenericBehavior<T>
{
    private readonly IGenericBehavior<T> _behavior;
    
    public PassWrappedBehavior(IGenericBehavior<T> behavior)
    {
        _behavior = behavior;
    }

    // unsupported, we define it ourselves
    public void GenericMethod<U>(U value)
    {
        throw new System.NotImplementedException();
    }
}

[WrapTo(nameof(_behavior))]
public partial class DefinedWrappedBehavior : IGenericBehavior<int>
{
    private readonly IGenericBehavior<int> _behavior;
    
    public DefinedWrappedBehavior(IGenericBehavior<int> behavior)
    {
        _behavior = behavior;
    }

    // unsupported, we define it ourselves
    public void GenericMethod<U>(U value)
    {
        throw new System.NotImplementedException();
    }
}

public class UnsupportedCompileTests
{
    
}