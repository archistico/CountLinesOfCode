# clsoc - Count Lines Of Code

`clsoc` is a small command line tool for counting lines of code.

The project has been modernized to .NET 8 and split into a console frontend plus a testable core library.

## Build

```bash
dotnet build clsoc.sln
```

## Test

```bash
dotnet test clsoc.sln
```

## Legacy usage

The original command style is still supported:

```bash
clsoc cs
```

This scans the current directory recursively and counts files with the selected extension.

## New usage

The new command style is closer to the final CLI target:

```bash
clsoc count .
clsoc count . --lang csharp
clsoc count . --lang csharp,xml
clsoc count . --ext cs,xaml,xml
```

The output is grouped by language:

```text
Language                 Files      Code  Comments     Blank     Total
-----------------------------------------------------------------------
C#                           4       120        12        20       152
XML/XAML                     2        35         3         6        44
-----------------------------------------------------------------------
Total                        6       155        15        26       196
```

## Supported languages in this phase

| Language | Extensions | Line comments | Block comments |
|---|---|---|---|
| C# | `.cs` | `//` | `/* */` |
| VB.NET | `.vb` | `'` | - |
| JavaScript/TypeScript | `.js`, `.jsx`, `.ts`, `.tsx` | `//` | `/* */` |
| CSS | `.css`, `.scss`, `.sass`, `.less` | - | `/* */` |
| Python | `.py` | `#` | - |
| SQL | `.sql` | `--` | `/* */` |
| XML/XAML | `.xml`, `.xaml`, `.csproj`, `.props`, `.targets`, `.config` | - | `<!-- -->` |
| HTML | `.html`, `.htm` | - | `<!-- -->` |

## Current architecture

```text
clsoc/
  Program.cs
  clsoc.csproj

src/
  clsoc.Core/
    Counting/
      CommentBlockDefinition.cs
      Contatore.cs
      FileScanner.cs
      LanguageCountResult.cs
      LanguageDefinition.cs
      LanguageRegistry.cs
      LineCounter.cs
      LineCountResult.cs
      ProjectCounter.cs

tests/
  clsoc.Tests/
    ContatoreTests.cs
    LanguageRegistryTests.cs
    LineCounterTests.cs
    ProjectCounterTests.cs
```

## Notes

The parser is now language-aware, but it is still intentionally lightweight. It is not a full compiler parser.

Mixed code/comment lines are still counted as code lines, preserving the current counting model.

## Roadmap

- [x] Migrate to .NET 8
- [x] Add baseline tests
- [x] Split Core and CLI
- [x] Improve C-like comment parsing
- [x] Add language definitions and grouped language counting
- [ ] Add default excluded directories such as `bin`, `obj`, `.git`, `.vs`, `node_modules`
- [ ] Add output formats: table, JSON, Markdown, CSV
- [ ] Add optional configuration file `clsoc.json`
- [ ] Package as a .NET global tool
