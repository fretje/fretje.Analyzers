using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace fretje.Analyzers.Tests;

public class AvoidStringEmptyTests
{
    [Theory]
    [InlineData("string.Empty")]
    [InlineData("String.Empty")]
    [InlineData("System.String.Empty")]
    public Task Reports_and_fixes_every_qualification(string expression)
    {
        var source = $$"""
            using System;

            class C
            {
                string M() => {|FRE0001:{{expression}}|};
            }
            """;

        const string fixedSource = """
            using System;

            class C
            {
                string M() => "";
            }
            """;

        return VerifyFixAsync(source, fixedSource);
    }

    [Fact]
    public Task Reports_and_fixes_using_static_simple_name()
    {
        const string source = """
            using static System.String;

            class C
            {
                string M() => {|FRE0001:Empty|};
            }
            """;

        const string fixedSource = """
            using static System.String;

            class C
            {
                string M() => "";
            }
            """;

        return VerifyFixAsync(source, fixedSource);
    }

    [Fact]
    public Task Fixes_method_argument()
    {
        const string source = """
            class C
            {
                void M()
                {
                    Use({|FRE0001:string.Empty|});
                }

                void Use(string value) { }
            }
            """;

        const string fixedSource = """
            class C
            {
                void M()
                {
                    Use("");
                }

                void Use(string value) { }
            }
            """;

        return VerifyFixAsync(source, fixedSource);
    }

    [Fact]
    public Task Preserves_trivia()
    {
        const string source = """
            class C
            {
                string M()
                {
                    return /* keep me */ {|FRE0001:string.Empty|};
                }
            }
            """;

        const string fixedSource = """
            class C
            {
                string M()
                {
                    return /* keep me */ "";
                }
            }
            """;

        return VerifyFixAsync(source, fixedSource);
    }

    [Fact]
    public Task Fixes_multiple_occurrences()
    {
        const string source = """
            class C
            {
                string A = {|FRE0001:string.Empty|};

                string B() => {|FRE0001:string.Empty|} + {|FRE0001:string.Empty|};
            }
            """;

        const string fixedSource = """
            class C
            {
                string A = "";

                string B() => "" + "";
            }
            """;

        return VerifyFixAsync(source, fixedSource);
    }

    [Fact]
    public Task Ignores_empty_string_literal()
    {
        const string source = """
            class C
            {
                string M() => "";
            }
            """;

        return VerifyNoDiagnosticsAsync(source);
    }

    [Fact]
    public Task Ignores_nameof()
    {
        const string source = """
            class C
            {
                string M() => nameof(string.Empty);
            }
            """;

        return VerifyNoDiagnosticsAsync(source);
    }

    [Fact]
    public Task Ignores_empty_fields_on_other_types()
    {
        const string source = """
            class Custom
            {
                public static readonly string Empty = "";
            }

            class C
            {
                string M() => Custom.Empty;
            }
            """;

        return VerifyNoDiagnosticsAsync(source);
    }

    private static Task VerifyFixAsync(string source, string fixedSource) =>
        CreateTest(source, fixedSource).RunAsync(TestContext.Current.CancellationToken);

    // No markup + no fixed code: asserts the analyzer reports nothing.
    private static Task VerifyNoDiagnosticsAsync(string source) =>
        CreateTest(source, fixedSource: null).RunAsync(TestContext.Current.CancellationToken);

    private static CSharpCodeFixTest<AvoidStringEmptyAnalyzer, AvoidStringEmptyCodeFixProvider, DefaultVerifier> CreateTest(
        string source,
        string? fixedSource)
    {
        var test = new CSharpCodeFixTest<AvoidStringEmptyAnalyzer, AvoidStringEmptyCodeFixProvider, DefaultVerifier>
        {
            TestCode = source,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
        };

        if (fixedSource is not null)
        {
            test.FixedCode = fixedSource;
        }

        return test;
    }
}
