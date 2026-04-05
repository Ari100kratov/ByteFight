using System.Collections.Immutable;
using GameRuntime.Logic.User.Intellisense.Common;
using GameRuntime.Logic.User.Intellisense.Dtos;
using GameRuntime.Logic.User.Intellisense.Workspace;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GameRuntime.Logic.User.Intellisense.Services;

public sealed class UserScriptSignatureHelpService(UserScriptRoslynContextFactory contextFactory)
{
    public async Task<UserScriptSignatureHelpDto?> GetSignatureHelpAsync(
        string userCode,
        int line,
        int column,
        CancellationToken ct)
    {
        using UserScriptRoslynContext context = contextFactory.CreateContext(userCode);
        Document document = context.Document;

        SyntaxNode? root = await document.GetSyntaxRootAsync(ct);
        SemanticModel? model = await document.GetSemanticModelAsync(ct);

        if (root is null || model is null)
        {
            return null;
        }

        int position = await RoslynMappingHelpers.MapToSourceOffsetAsync(document, line, column, ct);

        BaseArgumentListSyntax? argumentList = root.FindToken(Math.Max(position - 1, 0))
            .Parent?
            .AncestorsAndSelf()
            .OfType<BaseArgumentListSyntax>()
            .FirstOrDefault();

        if (argumentList is null)
        {
            return null;
        }

        ImmutableArray<IMethodSymbol> methodSymbols = GetMethodSymbols(argumentList, model, ct);

        if (methodSymbols.IsDefaultOrEmpty)
        {
            return null;
        }

        IMethodSymbol[] methods = [.. methodSymbols
            .Where(m => !RoslynSymbolHelpers.IsForbiddenSymbol(m))
            .OrderBy(m => m.Parameters.Length)];

        if (methods.Length == 0)
        {
            return null;
        }

        int activeParameter = GetActiveParameter(argumentList, position);
        int activeSignature = GetActiveSignature(methods, argumentList, model, ct);

        UserScriptSignatureDto[] signatures = [.. methods
            .Select(m => new UserScriptSignatureDto(
                Signature: m.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
                Documentation: DocumentationFormatter.Format(m.GetDocumentationCommentXml(cancellationToken: ct)),
                Parameters: [.. m.Parameters.Select(p => p.ToDisplayString())]))];

        return new UserScriptSignatureHelpDto(
            Signatures: signatures,
            ActiveSignature: activeSignature,
            ActiveParameter: activeParameter);
    }

    private static ImmutableArray<IMethodSymbol> GetMethodSymbols(
        BaseArgumentListSyntax argumentList,
        SemanticModel model,
        CancellationToken ct)
    {
        return argumentList.Parent switch
        {
            InvocationExpressionSyntax invocation => GetInvocationMethods(invocation, model, ct),
            ObjectCreationExpressionSyntax creation => GetConstructorMethods(creation, model, ct),
            _ => []
        };
    }

    private static ImmutableArray<IMethodSymbol> GetInvocationMethods(
        InvocationExpressionSyntax invocation,
        SemanticModel model,
        CancellationToken ct)
    {
        var memberGroup = model
            .GetMemberGroup(invocation.Expression, ct)
            .OfType<IMethodSymbol>()
            .ToImmutableArray();

        if (!memberGroup.IsDefaultOrEmpty)
        {
            return memberGroup;
        }

        SymbolInfo symbolInfo = model.GetSymbolInfo(invocation.Expression, ct);

        return symbolInfo.Symbol is IMethodSymbol method
            ? [method]
            : [.. symbolInfo.CandidateSymbols.OfType<IMethodSymbol>()];
    }

    private static ImmutableArray<IMethodSymbol> GetConstructorMethods(
        ObjectCreationExpressionSyntax creation,
        SemanticModel model,
        CancellationToken ct)
    {
        SymbolInfo symbolInfo = model.GetSymbolInfo(creation, ct);

        if (symbolInfo.Symbol is IMethodSymbol ctor)
        {
            return [ctor];
        }

        IMethodSymbol[] candidates = [.. symbolInfo.CandidateSymbols.OfType<IMethodSymbol>()];

        if (candidates.Length > 0)
        {
            return [.. candidates];
        }

        TypeInfo typeInfo = model.GetTypeInfo(creation.Type, ct);

        if (typeInfo.Type is not INamedTypeSymbol typeSymbol)
        {
            return [];
        }

        return [.. typeSymbol.InstanceConstructors.Where(c => c.MethodKind == MethodKind.Constructor)];
    }

    private static int GetActiveParameter(BaseArgumentListSyntax argumentList, int position)
    {
        return argumentList.Arguments
            .TakeWhile(a => a.SpanStart < position)
            .Count();
    }

    private static int GetActiveSignature(
        IMethodSymbol[] methods,
        BaseArgumentListSyntax argumentList,
        SemanticModel model,
        CancellationToken ct)
    {
        int argumentCount = argumentList.Arguments.Count;

        for (int i = 0; i < methods.Length; i++)
        {
            IMethodSymbol method = methods[i];

            if (CanAcceptArgumentCount(method, argumentCount))
            {
                return i;
            }
        }

        if (argumentCount > 0)
        {
            ITypeSymbol?[] argumentTypes = [.. argumentList.Arguments.Select(a => model.GetTypeInfo(a.Expression, ct).ConvertedType)];

            for (int i = 0; i < methods.Length; i++)
            {
                if (MatchesByTypes(methods[i], argumentTypes))
                {
                    return i;
                }
            }
        }

        return 0;
    }

    private static bool CanAcceptArgumentCount(IMethodSymbol method, int argumentCount)
    {
        if (method.Parameters.Length == argumentCount)
        {
            return true;
        }

        if (method.Parameters.Length < argumentCount)
        {
            return method.Parameters.LastOrDefault()?.IsParams == true;
        }

        int requiredCount = method.Parameters.Count(p => !p.IsOptional && !p.IsParams);
        return argumentCount >= requiredCount;
    }

    private static bool MatchesByTypes(IMethodSymbol method, ITypeSymbol?[] argumentTypes)
    {
        if (!CanAcceptArgumentCount(method, argumentTypes.Length))
        {
            return false;
        }

        int count = Math.Min(method.Parameters.Length, argumentTypes.Length);

        for (int i = 0; i < count; i++)
        {
            ITypeSymbol? argType = argumentTypes[i];
            if (argType is null)
            {
                continue;
            }

            IParameterSymbol parameter = method.Parameters[Math.Min(i, method.Parameters.Length - 1)];
            ITypeSymbol parameterType = parameter.IsParams && parameter.Type is IArrayTypeSymbol arrayType
                ? arrayType.ElementType
                : parameter.Type;

            if (!SymbolEqualityComparer.Default.Equals(argType, parameterType) &&
                !argType.AllInterfaces.Any(x => SymbolEqualityComparer.Default.Equals(x, parameterType)) &&
                !IsBaseType(argType, parameterType))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsBaseType(ITypeSymbol type, ITypeSymbol candidateBaseType)
    {
        ITypeSymbol? current = type.BaseType;

        while (current is not null)
        {
            if (SymbolEqualityComparer.Default.Equals(current, candidateBaseType))
            {
                return true;
            }

            current = current.BaseType;
        }

        return false;
    }
}
