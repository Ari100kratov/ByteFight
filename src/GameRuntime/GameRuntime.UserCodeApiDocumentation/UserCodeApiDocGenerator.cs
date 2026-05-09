using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using SharedKernel;

namespace GameRuntime.UserCodeApiDocumentation;

/// <summary>
/// Генерирует документацию API, доступного пользовательскому коду.
/// </summary>
public sealed class UserCodeApiDocGenerator
{
    private static readonly NullabilityInfoContext NullabilityInfoContext = new();

    /// <summary>
    /// Генерирует документацию пользовательского API по указанным сборкам.
    /// </summary>
    /// <param name="assemblies">Сборки, в которых нужно искать типы с <see cref="UserCodeApiAttribute"/>.</param>
    public UserCodeApiDoc Generate(IEnumerable<Assembly> assemblies)
    {
        Assembly[] assemblyList = [.. assemblies.Distinct()];

        Dictionary<string, XmlMemberDoc> xmlDocs = LoadXmlDocs(assemblyList);

        ApiTypeDoc[] types = [.. assemblyList
            .SelectMany(SafeGetTypes)
            .Where(type => type.GetCustomAttribute<UserCodeApiAttribute>() is not null)
            .Where(type => type.IsPublic || type.IsNestedPublic)
            .OrderBy(type => type.Namespace)
            .ThenBy(type => type.Name)
            .Select(type => CreateTypeDoc(type, xmlDocs))];

        return new UserCodeApiDoc
        {
            Types = types
        };
    }

    private static ApiTypeDoc CreateTypeDoc(Type type, IReadOnlyDictionary<string, XmlMemberDoc> xmlDocs)
    {
        string typeXmlName = XmlDocMemberName.ForType(type);
        xmlDocs.TryGetValue(typeXmlName, out XmlMemberDoc? typeXmlDoc);

        return new ApiTypeDoc
        {
            Name = GetFriendlyTypeName(type, includeNamespace: false),
            FullName = GetFriendlyTypeName(type, includeNamespace: true),
            Namespace = type.Namespace ?? string.Empty,
            Kind = GetTypeKind(type),
            Summary = typeXmlDoc?.Summary,
            Properties = CreatePropertyDocs(type, xmlDocs),
            Methods = CreateMethodDocs(type, xmlDocs),
            EnumValues = type.IsEnum ? CreateEnumValueDocs(type, xmlDocs) : []
        };
    }

    private static ApiPropertyDoc[] CreatePropertyDocs(
        Type type,
        IReadOnlyDictionary<string, XmlMemberDoc> xmlDocs)
    {
        return [.. type
            .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Where(property => property.GetMethod is { IsPublic: true })
            .Where(property => property.GetIndexParameters().Length == 0)
            .OrderBy(property => property.MetadataToken)
            .Select(property =>
            {
                string xmlName = XmlDocMemberName.ForProperty(property);
                xmlDocs.TryGetValue(xmlName, out XmlMemberDoc? xmlDoc);

                return new ApiPropertyDoc
                {
                    Name = property.Name,
                    Type = GetFriendlyTypeName(property.PropertyType, includeNamespace: false),
                    TypeFullName = GetFriendlyTypeName(property.PropertyType, includeNamespace: true),
                    Summary = xmlDoc?.Summary,
                    IsRequired = property.GetCustomAttribute<RequiredMemberAttribute>() is not null,
                    IsNullable = IsNullableProperty(property),
                    IsComputed = property.SetMethod is not { IsPublic: true }
                };
            })];
    }

    private static ApiMethodDoc[] CreateMethodDocs(
        Type type,
        IReadOnlyDictionary<string, XmlMemberDoc> xmlDocs)
    {
        return [.. type
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Where(method => !method.IsSpecialName)
            .Where(method => !method.IsConstructor)
            .Where(method => method.GetBaseDefinition().DeclaringType != typeof(object))
            .Where(method => method.GetCustomAttribute<CompilerGeneratedAttribute>() is null)
            .OrderBy(method => method.MetadataToken)
            .Select(method =>
            {
                string xmlName = XmlDocMemberName.ForMethod(method);
                xmlDocs.TryGetValue(xmlName, out XmlMemberDoc? xmlDoc);

                return new ApiMethodDoc
                {
                    Name = method.Name,
                    ReturnType = GetFriendlyTypeName(method.ReturnType, includeNamespace: false),
                    ReturnTypeFullName = GetFriendlyTypeName(method.ReturnType, includeNamespace: true),
                    Summary = xmlDoc?.Summary,
                    Returns = xmlDoc?.Returns,
                    Parameters = CreateParameterDocs(method, xmlDoc)
                };
            })];
    }

    private static ApiParameterDoc[] CreateParameterDocs(MethodBase method, XmlMemberDoc? xmlDoc)
    {
        return [.. method
            .GetParameters()
            .Select(parameter =>
            {
                string? parameterSummary = null;

                xmlDoc?.Parameters.TryGetValue(parameter.Name ?? string.Empty, out parameterSummary);

                return new ApiParameterDoc
                {
                    Name = parameter.Name ?? string.Empty,
                    Type = GetFriendlyTypeName(parameter.ParameterType, includeNamespace: false),
                    TypeFullName = GetFriendlyTypeName(parameter.ParameterType, includeNamespace: true),
                    Summary = parameterSummary,
                    IsNullable = IsNullableParameter(parameter),
                    IsOptional = parameter.IsOptional,
                    DefaultValue = parameter.IsOptional
                        ? FormatDefaultValue(parameter.DefaultValue)
                        : null
                };
            })];
    }

    private static ApiEnumValueDoc[] CreateEnumValueDocs(
        Type type,
        IReadOnlyDictionary<string, XmlMemberDoc> xmlDocs)
    {
        return [.. type
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .OrderBy(field => Convert.ToInt64(field.GetRawConstantValue(), CultureInfo.InvariantCulture))
            .Select(field =>
            {
                string xmlName = XmlDocMemberName.ForField(field);
                xmlDocs.TryGetValue(xmlName, out XmlMemberDoc? xmlDoc);

                return new ApiEnumValueDoc
                {
                    Name = field.Name,
                    Value = Convert.ToInt64(field.GetRawConstantValue(), CultureInfo.InvariantCulture),
                    Summary = xmlDoc?.Summary
                };
            })];
    }

    private static ApiTypeKind GetTypeKind(Type type)
    {
        if (type.IsEnum)
        {
            return ApiTypeKind.Enum;
        }

        if (type.IsValueType)
        {
            return ApiTypeKind.Struct;
        }

        return IsRecord(type)
            ? ApiTypeKind.Record
            : ApiTypeKind.Class;
    }

    private static bool IsRecord(Type type)
    {
#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
        return type.GetMethod("<Clone>$", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) is not null
               || type.GetMethod("PrintMembers", BindingFlags.Instance | BindingFlags.NonPublic) is not null;
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
    }

    private static bool IsNullableProperty(PropertyInfo property)
    {
        NullabilityInfo info = NullabilityInfoContext.Create(property);
        return info.ReadState == NullabilityState.Nullable;
    }

    private static bool IsNullableParameter(ParameterInfo parameter)
    {
        NullabilityInfo info = NullabilityInfoContext.Create(parameter);
        return info.ReadState == NullabilityState.Nullable;
    }

    private static string? FormatDefaultValue(object? value)
    {
        return value switch
        {
            null => "null",
            string text => $"\"{text}\"",
            bool boolean => boolean ? "true" : "false",
            char character => $"'{character}'",
            Enum enumValue => $"{enumValue.GetType().Name}.{enumValue}",
            _ => Convert.ToString(value, CultureInfo.InvariantCulture)
        };
    }

    private static Dictionary<string, XmlMemberDoc> LoadXmlDocs(IEnumerable<Assembly> assemblies)
    {
        Dictionary<string, XmlMemberDoc> result = new(StringComparer.Ordinal);

        foreach (Assembly assembly in assemblies)
        {
            string? xmlPath = GetXmlDocumentationPath(assembly);

            if (xmlPath is null || !File.Exists(xmlPath))
            {
                continue;
            }

            var document = XDocument.Load(xmlPath);

            foreach (XElement member in document.Descendants("member"))
            {
                string? name = member.Attribute("name")?.Value;

                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                result[name] = new XmlMemberDoc
                {
                    Summary = NormalizeXmlText(member.Element("summary")),
                    Returns = NormalizeXmlText(member.Element("returns")),
                    Parameters = member
                        .Elements("param")
                        .Where(param => param.Attribute("name") is not null)
                        .ToDictionary(
                            param => param.Attribute("name")!.Value,
                            param => NormalizeXmlText(param) ?? string.Empty,
                            StringComparer.Ordinal)
                };
            }
        }

        return result;
    }

    private static string? GetXmlDocumentationPath(Assembly assembly)
    {
        string assemblyLocation = assembly.Location;

        if (string.IsNullOrWhiteSpace(assemblyLocation))
        {
            return null;
        }

        return Path.ChangeExtension(assemblyLocation, ".xml");
    }

    private static string? NormalizeXmlText(XElement? element)
    {
        if (element is null)
        {
            return null;
        }

        string text = element.Value;

        string normalized = string.Join(
            Environment.NewLine,
            text
                .Replace("\r\n", "\n", StringComparison.Ordinal)
                .Split('\n')
                .Select(line => line.Trim())
                .Where(line => line.Length > 0));

        return normalized.Length == 0 ? null : normalized;
    }

    private static IEnumerable<Type> SafeGetTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException exception)
        {
            return exception.Types.OfType<Type>();
        }
    }

    private static string GetFriendlyTypeName(Type type, bool includeNamespace)
    {
        if (type == typeof(void))
        {
            return "void";
        }

        Type? nullableUnderlyingType = Nullable.GetUnderlyingType(type);

        if (nullableUnderlyingType is not null)
        {
            return $"{GetFriendlyTypeName(nullableUnderlyingType, includeNamespace)}?";
        }

        if (type.IsArray)
        {
            return $"{GetFriendlyTypeName(type.GetElementType()!, includeNamespace)}[]";
        }

        if (type.IsGenericType)
        {
            Type genericDefinition = type.GetGenericTypeDefinition();
            Type[] arguments = type.GetGenericArguments();

            string genericName = genericDefinition.Name;
            int tickIndex = genericName.IndexOf('`', StringComparison.Ordinal);
            if (tickIndex >= 0)
            {
                genericName = genericName[..tickIndex];
            }

            string typeName = MapGenericTypeName(genericDefinition, genericName);
            string args = string.Join(", ", arguments.Select(arg => GetFriendlyTypeName(arg, includeNamespace)));

            if (!includeNamespace || IsWellKnownGeneric(genericDefinition))
            {
                return $"{typeName}<{args}>";
            }

            string ns = genericDefinition.Namespace;
            return string.IsNullOrWhiteSpace(ns)
                ? $"{typeName}<{args}>"
                : $"{ns}.{typeName}<{args}>";
        }

        return GetSimpleFriendlyTypeName(type, includeNamespace);
    }

    private static string GetSimpleFriendlyTypeName(Type type, bool includeNamespace)
    {
        string? keyword = type switch
        {
            _ when type == typeof(string) => "string",
            _ when type == typeof(bool) => "bool",
            _ when type == typeof(byte) => "byte",
            _ when type == typeof(short) => "short",
            _ when type == typeof(int) => "int",
            _ when type == typeof(long) => "long",
            _ when type == typeof(float) => "float",
            _ when type == typeof(double) => "double",
            _ when type == typeof(decimal) => "decimal",
            _ when type == typeof(object) => "object",
            _ when type == typeof(Guid) => "Guid",
            _ => null
        };

        if (keyword is not null)
        {
            return keyword;
        }

        if (!includeNamespace || string.IsNullOrWhiteSpace(type.Namespace))
        {
            return type.Name;
        }

        return $"{type.Namespace}.{type.Name}";
    }

    private static string MapGenericTypeName(Type genericDefinition, string fallback)
    {
        if (genericDefinition == typeof(IEnumerable<>))
        {
            return "IEnumerable";
        }

        if (genericDefinition == typeof(IReadOnlyList<>))
        {
            return "IReadOnlyList";
        }

        if (genericDefinition == typeof(IReadOnlyCollection<>))
        {
            return "IReadOnlyCollection";
        }

        if (genericDefinition == typeof(IReadOnlyDictionary<,>))
        {
            return "IReadOnlyDictionary";
        }

        if (genericDefinition == typeof(List<>))
        {
            return "List";
        }

        if (genericDefinition == typeof(Dictionary<,>))
        {
            return "Dictionary";
        }

        return fallback;
    }

    private static bool IsWellKnownGeneric(Type genericDefinition)
    {
        return genericDefinition == typeof(IEnumerable<>)
               || genericDefinition == typeof(IReadOnlyList<>)
               || genericDefinition == typeof(IReadOnlyCollection<>)
               || genericDefinition == typeof(IReadOnlyDictionary<,>)
               || genericDefinition == typeof(List<>)
               || genericDefinition == typeof(Dictionary<,>);
    }

    private sealed record XmlMemberDoc
    {
        public string? Summary { get; init; }

        public string? Returns { get; init; }

        public required Dictionary<string, string> Parameters { get; init; }
    }

    private static class XmlDocMemberName
    {
        public static string ForType(Type type)
            => $"T:{GetXmlTypeName(type)}";

        public static string ForProperty(PropertyInfo property)
            => $"P:{GetXmlTypeName(property.DeclaringType!)}.{property.Name}";

        public static string ForField(FieldInfo field)
            => $"F:{GetXmlTypeName(field.DeclaringType!)}.{field.Name}";

        public static string ForMethod(MethodInfo method)
        {
            string result = $"M:{GetXmlTypeName(method.DeclaringType!)}.{method.Name}";

            ParameterInfo[] parameters = method.GetParameters();

            if (parameters.Length == 0)
            {
                return result;
            }

            string parameterList = string.Join(",", parameters.Select(p => GetXmlParameterTypeName(p.ParameterType)));
            return $"{result}({parameterList})";
        }

        private static string GetXmlTypeName(Type type)
        {
            if (!type.IsGenericType)
            {
                return type.FullName ?? type.Name;
            }

            Type genericDefinition = type.GetGenericTypeDefinition();
            string fullName = genericDefinition.FullName ?? genericDefinition.Name;
            int tickIndex = fullName.IndexOf('`', StringComparison.Ordinal);

            return tickIndex >= 0 ? fullName[..tickIndex] : fullName;
        }

        private static string GetXmlParameterTypeName(Type type)
        {
            if (type.IsByRef)
            {
                type = type.GetElementType()!;
            }

            if (type.IsArray)
            {
                return $"{GetXmlParameterTypeName(type.GetElementType()!)}[]";
            }

            Type? nullableUnderlyingType = Nullable.GetUnderlyingType(type);
            if (nullableUnderlyingType is not null)
            {
                return $"System.Nullable{{{GetXmlParameterTypeName(nullableUnderlyingType)}}}";
            }

            if (type.IsGenericType)
            {
                string genericTypeName = GetXmlTypeName(type);
                string genericArgs = string.Join(",", type.GetGenericArguments().Select(GetXmlParameterTypeName));
                return $"{genericTypeName}{{{genericArgs}}}";
            }

            return type.FullName ?? type.Name;
        }
    }
}
