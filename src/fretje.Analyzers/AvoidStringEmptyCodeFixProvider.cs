using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace fretje.Analyzers;

/// <summary>Code fix for FRE0001: replaces <c>string.Empty</c> with <c>""</c>.</summary>
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AvoidStringEmptyCodeFixProvider)), Shared]
public sealed class AvoidStringEmptyCodeFixProvider : CodeFixProvider
{
    private const string Title = "Replace with \"\"";

    public override ImmutableArray<string> FixableDiagnosticIds => [DiagnosticIds.AvoidStringEmpty];

    // Enables "Fix all in document / project / solution"
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return;
        }

        var diagnostic = context.Diagnostics[0];

        // innermost-for-tie: for `Foo(string.Empty)` the ArgumentSyntax and the member access
        // share the same span; we want the expression, not the argument.
        if (root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) is not ExpressionSyntax expression)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                title: Title,
                createChangedDocument: ct => ReplaceWithLiteralAsync(context.Document, expression, ct),
                equivalenceKey: Title),
            diagnostic);
    }

    private static async Task<Document> ReplaceWithLiteralAsync(
        Document document,
        ExpressionSyntax expression,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return document;
        }

        var emptyLiteral = SyntaxFactory
            .LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(""))
            .WithTriviaFrom(expression); // keep comments / whitespace

        return document.WithSyntaxRoot(root.ReplaceNode(expression, emptyLiteral));
    }
}
