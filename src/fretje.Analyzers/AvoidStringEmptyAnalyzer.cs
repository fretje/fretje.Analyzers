using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace fretje.Analyzers;

/// <summary>FRE0001: use <c>""</c> instead of <c>string.Empty</c>.</summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AvoidStringEmptyAnalyzer : DiagnosticAnalyzer
{
    private const string EmptyFieldName = "Empty";

    internal static readonly DiagnosticDescriptor Rule = new(
        id: DiagnosticIds.AvoidStringEmpty,
        title: "Use \"\" instead of string.Empty",
        messageFormat: "Use \"\" instead of 'string.Empty'",
        category: DiagnosticCategories.Style,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Prefer the empty string literal over string.Empty. Unlike string.Empty, the literal is a " +
                     "compile-time constant and can be used in const declarations, attribute arguments, " +
                     "default parameter values and switch labels.",
        helpLinkUri: DiagnosticIds.HelpLink(DiagnosticIds.AvoidStringEmpty));

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterOperationAction(AnalyzeFieldReference, OperationKind.FieldReference);
    }

    private static void AnalyzeFieldReference(OperationAnalysisContext context)
    {
        var fieldReference = (IFieldReferenceOperation)context.Operation;
        var field = fieldReference.Field;

        // Semantic check, so it works for `string.Empty`, `String.Empty`,
        // `System.String.Empty` and `using static System.String; Empty`.
        if (field.Name != EmptyFieldName ||
            field.ContainingType.SpecialType != SpecialType.System_String)
        {
            return;
        }

        // nameof(string.Empty) must stay as is; nameof("") doesn't compile.
        if (fieldReference.Parent is INameOfOperation)
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Rule, fieldReference.Syntax.GetLocation()));
    }
}
