using System.Collections.Generic;
using System.Text;

namespace Doner.Wrap;

public static class ClassSourceGenerator
{
    public static string GenerateWrapperClassSource(ClassToGenerate classInfo)
    {
        var sb = new StringBuilder();

        foreach (var s in GenerateWrapperClassStrings(classInfo))
        {
            sb.AppendLine(s);
        }

        return sb.ToString();
    }

    private static IEnumerable<string> GenerateWrapperClassStrings(ClassToGenerate classInfo)
    {
        yield return $$"""
                       #nullable enable

                       namespace {{classInfo.Namespace}}
                       {
                           public partial class {{classInfo.ClassName}}{{classInfo.GenericParams}}
                           {
                       """;

        // Generate delegating methods and properties
        foreach (var property in classInfo.Properties)
        {
            yield return $$"""
                                   public {{property.Type}} {{property.Name}}
                                   {
                           """;
            if (property.HasGetter)
            {
                yield return $"            get => {classInfo.FieldName}.{property.Name};";
            }

            if (property.HasSetter)
            {
                yield return $"            set => {classInfo.FieldName}.{property.Name} = value;";
            }

            yield return "        }";
        }

        foreach (var method in classInfo.Methods)
        {
            if (method.ReturnType != "void")
            {
                yield return $$"""
                                       public {{method.ReturnType}} {{method.Name}}({{method.Parameters}})
                                       {
                                           return {{method.ReturnMods}} {{classInfo.FieldName}}.{{method.Name}}({{method.ParameterNames}});
                                       }
                               """;
            }
            else
            {
                yield return $$"""
                                       public {{method.ReturnType}} {{method.Name}}({{method.Parameters}})
                                       {
                                           {{classInfo.FieldName}}.{{method.Name}}({{method.ParameterNames}});
                                       }
                               """;
            }
        }

        yield return """
                         }
                     }
                     """;
    }
}

public sealed record ClassToGenerate(
    string ClassName,
    string GenericParams,
    string Namespace,
    string FieldName,
    List<MethodToGenerate> Methods,
    List<PropertyToGenerate> Properties);

public sealed record MethodToGenerate(string Name, string ReturnMods, string ReturnType, string Parameters, string ParameterNames);

public sealed record PropertyToGenerate(string Name, string Type, bool HasGetter, bool HasSetter);