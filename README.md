# fretje.Analyzers

[![NuGet](https://img.shields.io/nuget/v/fretje.Analyzers.svg)](https://www.nuget.org/packages/fretje.Analyzers)
[![NuGet downloads](https://img.shields.io/nuget/dt/fretje.Analyzers.svg)](https://www.nuget.org/packages/fretje.Analyzers)
[![CI](https://github.com/fretje/fretje.Analyzers/actions/workflows/ci.yml/badge.svg)](https://github.com/fretje/fretje.Analyzers/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/fretje/fretje.Analyzers/blob/main/LICENSE)

A collection of Roslyn analyzers and code fixes for C#.

## Installation

```sh
dotnet add package fretje.Analyzers
```

or add it to your project / `Directory.Build.props` manually:

```xml
<PackageReference Include="fretje.Analyzers" Version="x.y.z" PrivateAssets="all" />
```

The package is a development dependency: it adds no assemblies to your output and doesn't flow to
projects that reference yours. Requires .NET SDK 9.0.300 / Visual Studio 2022 17.14 or newer
(Roslyn 4.14), including .NET SDK 10 and Visual Studio 2026.

## Rules

| ID | Title | Severity | Code fix |
|---|---|---|---|
| [FRE0001](https://github.com/fretje/fretje.Analyzers/blob/main/docs/rules/FRE0001.md) | Use `""` instead of `string.Empty` | Warning | ✔️ |

## Configuration

Change the severity of a rule (or turn it off) in your `.editorconfig`:

```ini
[*.cs]
dotnet_diagnostic.FRE0001.severity = suggestion   # none | silent | suggestion | warning | error
```

## Building from source

```sh
git clone https://github.com/fretje/fretje.Analyzers.git
cd fretje.Analyzers
dotnet build
dotnet test
dotnet pack src/fretje.Analyzers -c Release -o artifacts
```

## Contributing

Bug reports, rule ideas and pull requests are welcome, see [CONTRIBUTING.md](https://github.com/fretje/fretje.Analyzers/blob/main/CONTRIBUTING.md).
Notable changes are listed in the [changelog](https://github.com/fretje/fretje.Analyzers/blob/main/CHANGELOG.md).

## License

[MIT](https://github.com/fretje/fretje.Analyzers/blob/main/LICENSE)
