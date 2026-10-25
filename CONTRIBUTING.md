# Contributing

Thanks for your interest in improving fretje.Analyzers!

## Getting started

You need the .NET SDK version from [`global.json`](global.json) (newer feature bands are fine).

```sh
dotnet build
dotnet test
```

Tests use [xunit.v3](https://xunit.net/) on the [Microsoft Testing Platform](https://learn.microsoft.com/dotnet/core/testing/microsoft-testing-platform-intro)
(enabled for `dotnet test` through `global.json`). The test project is a normal executable, so you can also run it
directly, with filters: `dotnet run --project tests/fretje.Analyzers.Tests -- --filter-method "*Nameof*"`.

## Adding a new rule

1. Add an ID to `src/fretje.Analyzers/DiagnosticIds.cs` (next free `FRExxxx` number).
2. Add `<Name>Analyzer.cs` (and `<Name>CodeFixProvider.cs` if there is a code fix) in `src/fretje.Analyzers/`.
3. Add tests in `tests/fretje.Analyzers.Tests/`: cases where the rule fires, the fix output,
   and cases where it must stay silent.
4. Add a docs page `docs/rules/FRExxxx.md` (the descriptor's help link points there).
5. Add a row to `src/fretje.Analyzers/AnalyzerReleases.Unshipped.md`, the rule table in `README.md`,
   and an entry under **Unreleased** in `CHANGELOG.md`.

## Roslyn version policy

The analyzer references the **oldest** Roslyn version it supports (`Microsoft.CodeAnalysis.CSharp.Workspaces` in
`Directory.Packages.props`, currently 4.14 = VS 2022 17.14 / .NET SDK 9.0.300), not the newest, because an analyzer
compiled against a newer Roslyn fails to load in older compilers. Raise it deliberately, and mention it in the changelog
and the README requirements. The version is written as a range (`[4.14.0,5.0.0)`) so the NuGet UI doesn't offer 5.x as an update.

## Releasing (maintainers)

**Preview releases** (`v1.0.0-preview.1`): no release-file changes needed, leave the rules in
`AnalyzerReleases.Unshipped.md` and just tag. Do the steps below for the first stable release.

1. Move the rows from `AnalyzerReleases.Unshipped.md` to `AnalyzerReleases.Shipped.md` under a
   `## Release x.y.z` heading, and turn **Unreleased** in `CHANGELOG.md` into the new version.
2. Merge to `main`, then tag and push:
   ```sh
   git tag v1.0.0
   git push origin v1.0.0
   ```
3. The **Publish** workflow builds, tests, packs, pushes to nuget.org and creates the GitHub release.
   Tags with a suffix (`v1.1.0-preview.1`) are published as pre-releases.

The package version comes from the git tag (via [MinVer](https://github.com/adamralph/minver)); there is no
version number to edit in the project files.

One-time setup (nuget.org [Trusted Publishing](https://aka.ms/nuget/trusted-publishing), no API key needed):

1. On nuget.org: user menu → **Trusted Publishing** → add a policy:
   - Package owner: yourself (or your organization)
   - Repository owner: `fretje`, Repository: `fretje.Analyzers`
   - Workflow file: `publish.yml` (file name only)
   - Environment: leave empty
   - Scopes: allow publishing **new packages** and **new versions**, glob pattern `fretje.Analyzers`
2. In the GitHub repo: Settings → Secrets and variables → Actions → add the secret `NUGET_USER` with your
   nuget.org **profile name** (not your email address).

A policy for a private GitHub repo is only temporarily active for 7 days until the first successful publish
(restart the window on the Trusted Publishing page if it lapses). Public repos don't have this limitation.
