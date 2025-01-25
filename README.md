
# Usage

1. Add the dependency:
```bash
Install-Package Doner.Wrap
```
Or:
```bash
dotnet add package Doner.Wrap
```
2. Add `[WrapTo(nameof(_field))]` to the class you want the methods to be generated for.

Example:
```csharp
[DelegateTo(nameof(_behavior))]
partial class RuntimeChangableBehavior : IBehavior {
    private IBehavior _behavior = new Behavior1();
    
    public void DoSomething() {
        // Non-delegated method
    }
}
```