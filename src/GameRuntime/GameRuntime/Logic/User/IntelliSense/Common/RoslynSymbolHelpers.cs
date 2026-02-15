using Microsoft.CodeAnalysis;

namespace GameRuntime.Logic.User.Intellisense.Common;

public static class RoslynSymbolHelpers
{
    public static bool IsForbiddenSymbol(ISymbol symbol)
    {
        INamedTypeSymbol? type = symbol switch
        {
            IMethodSymbol m => m.ContainingType,
            IPropertySymbol p => p.ContainingType,
            IFieldSymbol f => f.ContainingType,
            INamedTypeSymbol t => t,
            _ => null
        };

        string? ns = type?.ContainingNamespace?.ToDisplayString();

        return ns is "System.IO"
            or "System.Net"
            or "System.Reflection"
            or "System.Diagnostics"
            or "System.Security"
            or "System.Runtime.InteropServices"
            || type?.ToDisplayString().StartsWith("System.Console", StringComparison.Ordinal) == true;
    }
}
