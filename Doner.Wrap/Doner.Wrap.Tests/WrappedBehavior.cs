namespace Doner.Wrap.Tests;

[WrapTo(nameof(_testBehavior))]
public partial class WrappedBehavior(TestBehavior testBehavior) : IBehavior
{
    private readonly IBehavior _testBehavior = testBehavior;
}