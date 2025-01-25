using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Doner.Wrap;

[Generator(LanguageNames.CSharp)]
public class AttributeGenerator : IIncrementalGenerator
{
    public const string Namespace = "Doner.Wrap";
    public const string Classname = "WrapToAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Register the attribute
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "WrapToAttribute.g.cs",
            SourceText.From($$"""
                              #nullable enable

                              using System;

                              namespace {{Namespace}}
                              {
                                   /// <summary>
                                   /// Delegates unimplemented methods to the specified field/property.
                                   /// <example>
                                   /// <code>
                                   /// [DelegateTo(nameof(_behavior))]
                                   /// partial class RuntimeChangableBehavior : IBehavior {
                                   ///     private IBehavior _behavior = new Behavior1();
                                   ///     
                                   ///     public void DoSomething() {
                                   ///         // Non-delegated method
                                   ///     }
                                   /// }
                                   /// </code>
                                   /// </example>
                                   /// </summary>
                                   [AttributeUsage(AttributeTargets.Class)]
                                   public class {{Classname}}(string field) : System.Attribute
                                   {
                                       /// <summary>
                                       /// The field name to delegate to.
                                       /// </summary>
                                       public string Field { get; } = field;
                                   }
                              }
                              """, Encoding.UTF8)
        ));
    }
}