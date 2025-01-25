using System;
using System.Collections.Frozen;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Doner.Wrap;

[Generator(LanguageNames.CSharp)]
public partial class WrapperGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Create a pipeline for finding decorated classes
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => s is ClassDeclarationSyntax,
                transform: static (ctx, _) => GetClassToGenerate(ctx))
            .Where(static m => m is not null);

        // Register the source output
        context.RegisterSourceOutput(classDeclarations,
            static (spc, classInfo) => Execute(spc, classInfo!));
    }

    private static ClassToGenerate? GetClassToGenerate(GeneratorSyntaxContext context)
    {
        var wrappedClass = (ClassDeclarationSyntax)context.Node;
        var model = context.SemanticModel;

        if (model.GetDeclaredSymbol(wrappedClass) is not INamedTypeSymbol wrapperClass)
        {
            return null;
        }

        var delegateAttribute = wrapperClass.GetAttributes()
            .FirstOrDefault(attr => attr.AttributeClass is
                { Name: AttributeGenerator.Classname } && attr.AttributeClass.ContainingNamespace.ToDisplayString() == AttributeGenerator.Namespace);

        if (delegateAttribute == null)
        {
            return null;
        }

        var delegateFieldName = delegateAttribute.ConstructorArguments[0].Value?.ToString();
        if (delegateFieldName == null)
        {
            return null;
        }

        var delegateField = wrapperClass.GetMembers()
            .OfType<IFieldSymbol>()
            .FirstOrDefault(field => field.Name == delegateFieldName);

        if (delegateField == null)
        {
            return null;
        }

        var definedMethodNames = ClassUtils.GetBaseTypes(wrapperClass)
            .SelectMany(c => c.GetMembers())
            .OfType<IMethodSymbol>()
            .Where(m => !m.IsStatic && m.MethodKind == MethodKind.Ordinary)
            .Where(IsPublic)
            .Select(s => s.Name)
            .ToFrozenSet();

        var definedPropertyNames = ClassUtils.GetBaseTypes(wrapperClass)
            .SelectMany(c => c.GetMembers())
            .OfType<IPropertySymbol>()
            .Where(p => !p.IsStatic)
            .Where(IsPublic)
            .Select(s => s.Name)
            .ToFrozenSet();

        var delegateClass = delegateField.Type;
        
        
        // Collect methods and properties
        //var methods = ClassUtils.GetBaseTypes(delegateClass)
        //    .Union<ITypeSymbol>(ClassUtils.GetAllInterfaces(wrapperClass), SymbolEqualityComparer.IncludeNullability)

        // Collect methods and properties
        var methods = ClassUtils.GetAllInterfaces(wrapperClass)
            .SelectMany(c => c.GetMembers())
            .OfType<IMethodSymbol>()
            .Where(IsInstanceAccess)
            .Where(IsOrdinaryMethod)
            .Where(IsPublic)
            .Where(m => !definedMethodNames.Contains(m.Name))
            .Select(GetMethodToGenerate)
            .ToList();

        var properties = ClassUtils.GetBaseTypes(delegateClass)
            .SelectMany(c => c.GetMembers())
            .OfType<IPropertySymbol>()
            .Where(IsInstanceAccess)
            .Where(IsPublic)
            //.Where(m => m.IsDefinition && !m.IsOverride)
            .Where(m => !definedPropertyNames.Contains(m.Name))
            .Select(GetPropertyToGenerate)
            .ToList();
        
        var genericParams = "";
        if (wrapperClass.IsGenericType)
        {
            genericParams = "<" + string.Join(",", wrapperClass.TypeArguments.Select(t => t.ToString())) + ">";
        }

        return new ClassToGenerate(
            wrapperClass.Name,
            genericParams,
            wrapperClass.ContainingNamespace.ToDisplayString(),
            delegateFieldName,
            methods,
            properties);
    }

    private static void Execute(SourceProductionContext context, ClassToGenerate classInfo)
    {
        var source = ClassSourceGenerator.GenerateWrapperClassSource(classInfo);
        context.AddSource($"{classInfo.ClassName}.Wrapper.g.cs", SourceText.From(source, Encoding.UTF8));
    }

    private static PropertyToGenerate GetPropertyToGenerate(IPropertySymbol p)
    {
        return new PropertyToGenerate(p.Name, p.Type.ToDisplayString(),
            p.GetMethod is { DeclaredAccessibility: Accessibility.Public },
            p.SetMethod is { DeclaredAccessibility: Accessibility.Public }
        );
    }

    private static MethodToGenerate GetMethodToGenerate(IMethodSymbol m)
    {
        return new MethodToGenerate(
            m.Name,
            ReturnMods(m),
            ReturnType(m),
            string.Join(", ", m.Parameters.Select(ParameterNameWithModifiers)),
            string.Join(", ", m.Parameters.Select(ReferenceName))
        );
        
        static string ReturnMods(IMethodSymbol m)
        {
            if (m.ReturnsByRef || m.ReturnsByRefReadonly)
            {
                return "ref";
            }
            return "";
        }
        
        static string ReturnType(IMethodSymbol m)
        {
            if (m.ReturnsByRef)
            {
                return "ref " + m.ReturnType.ToDisplayString();
            }
            if (m.ReturnsByRefReadonly)
            {
                return "ref readonly " + m.ReturnType.ToDisplayString();
            }
            return m.ReturnType.ToDisplayString();
        }

        static string ParameterNameWithModifiers(IParameterSymbol p)
        {
            var typeAndName = $"{p.Type.ToDisplayString()} {p.Name}";

            var modifiers = p.RefKind switch
            {
                RefKind.None => string.Empty,
                RefKind.In => "in ",
                RefKind.Out => "out ",
                RefKind.Ref => "ref ",
                RefKind.RefReadOnlyParameter => "ref readonly ",
            };
            var defaultPart = DefaultPart(p);
            return modifiers + typeAndName + defaultPart;
        }

        static string ReferenceName(IParameterSymbol p)
        {
            return p.RefKind switch
            {
                RefKind.RefReadOnlyParameter => "in " + p.Name,
                RefKind.Ref => "ref " + p.Name,
                _ => p.Name,
            };
        }

        static string DefaultPart(IParameterSymbol p)
        {
            if (!p.HasExplicitDefaultValue) return string.Empty;
            if (p.Type.TypeKind == TypeKind.Enum)
            {
                if (p.ExplicitDefaultValue == null)
                    return " = default";
                var enumName = p.Type.ToDisplayString();
                return $" = ({enumName})" + p.ExplicitDefaultValue;
            }

            if (p.Type.IsValueType)
            {
                return " = " + (p.ExplicitDefaultValue ?? "default").ToString().ToLowerInvariant();
            }

            return " = " + (p.ExplicitDefaultValue ?? "default");
        }
    }
}