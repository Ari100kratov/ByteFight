using Microsoft.CodeAnalysis;

namespace GameRuntime.Logic.User.IntelliSense.Roslyn;

internal static class TypeNormalizer
{
    public static string Normalize(ITypeSymbol type)
    {
        if (type is IArrayTypeSymbol arr)
        {
            return Normalize(arr.ElementType) + "[]";
        }

        if (type is INamedTypeSymbol named &&
            named.Name == "IReadOnlyList" &&
            named.TypeArguments.Length == 1)
        {
            return Normalize(named.TypeArguments[0]) + "[]";
        }

        return type.Name;
    }
}
