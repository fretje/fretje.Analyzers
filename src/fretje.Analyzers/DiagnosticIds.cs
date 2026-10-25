namespace fretje.Analyzers;

internal static class DiagnosticIds
{
    public const string AvoidStringEmpty = "FRE0001";

    public static string HelpLink(string id) =>
        $"https://github.com/fretje/fretje.Analyzers/blob/main/docs/rules/{id}.md";
}

internal static class DiagnosticCategories
{
    public const string Style = "Style";
}
