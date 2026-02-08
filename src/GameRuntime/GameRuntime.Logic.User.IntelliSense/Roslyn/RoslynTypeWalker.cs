using Microsoft.CodeAnalysis;
using GameRuntime.Logic.User.IntelliSense.Model;

namespace GameRuntime.Logic.User.IntelliSense.Roslyn;

internal sealed class RoslynTypeWalker
{
    public Dictionary<string, ApiTypeDescriptor> Walk(Compilation compilation)
    {
        var result = new Dictionary<string, ApiTypeDescriptor>();

        INamespaceSymbol apiNamespace = compilation
            .GlobalNamespace
            .GetNamespaceMembers()
            .SelectMany(Flatten)
            .First(n => n.ToDisplayString() == "GameRuntime.Logic.User.Api");

        foreach (INamedTypeSymbol type in apiNamespace.GetTypeMembers())
        {
            result[type.Name] = DescribeType(type);
        }

        return result;
    }

    private static IEnumerable<INamespaceSymbol> Flatten(INamespaceSymbol ns)
    {
        yield return ns;
        foreach (INamespaceSymbol child in ns.GetNamespaceMembers())
        {
            foreach (INamespaceSymbol nested in Flatten(child))
            {
                yield return nested;
            }
        }
    }

    private static ApiTypeDescriptor DescribeType(INamedTypeSymbol type)
    {
        return new ApiTypeDescriptor
        {
            Name = type.Name,
            Properties = type
                .GetMembers()
                .OfType<IPropertySymbol>()
                .Where(p => p.DeclaredAccessibility == Accessibility.Public)
                .ToDictionary(
                    p => p.Name,
                    p => new ApiPropertyDescriptor
                    {
                        Type = TypeNormalizer.Normalize(p.Type)
                    })
        };
    }
}
