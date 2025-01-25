using Microsoft.CodeAnalysis;

namespace Doner.Wrap;

public partial class WrapperGenerator
{
    private static bool IsPublic(ISymbol m)
    {
        return m.DeclaredAccessibility.HasFlag(Accessibility.Public);
    }

    private static bool IsInstanceAccess(ISymbol p)
    {
        return !p.IsStatic;
    }

    private static bool IsOrdinaryMethod(IMethodSymbol m)
    {
        return m.MethodKind == MethodKind.Ordinary;
    }
    
    private static bool IsGenericMethod(IMethodSymbol m)
    {
        return m.IsGenericMethod;
    }
}