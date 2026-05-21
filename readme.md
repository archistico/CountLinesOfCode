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
clsoc count . --exclude generated,temp
clsoc count . --no-default-excludes
clsoc count . --format table
clsoc count . --format json
clsoc count . --format markdown
clsoc count . --format csv
clsoc count . --format json --output report.json
```

The output is grouped by language and now includes a small metrics section:

```text
Language                 Files      Code  Comments     Blank     Total
-----------------------------------------------------------------------
C#                           4       120        12        20       152
XML/XAML                     2        35         3         6        44
-----------------------------------------------------------------------
Total                        6       155        15        26       196

Metrics
-------
Comment ratio:          7.65 %
Blank ratio:           13.27 %
Code ratio:            79.08 %
Average lines/file:     32.67
Largest file:         src/Program.cs (98 lines)
```



## Output formats

The default output format is `table`.

```bash
clsoc count . --format table
```

You can also generate machine-readable or documentation-friendly reports:

```bash
clsoc count . --format json
clsoc count . --format markdown
clsoc count . --format csv
```

Reports can be written to a file:

```bash
clsoc count . --format json --output report.json
clsoc count . --format markdown --output report.md
clsoc count . --format csv --output report.csv
```

### JSON example

```json
{
  "languages": [
    {
      "id": "csharp",
      "name": "C#",
      "files": 4,
      "code": 120,
      "comments": 12,
      "blank": 20,
      "total": 152,
      "metrics": {
        "codeRatio": 0.7895,
        "commentRatio": 0.0789,
        "blankRatio": 0.1316,
        "averageLinesPerFile": 38,
        "largestFile": {
          "path": "src/Program.cs",
          "total": 98,
          "code": 82,
          "comments": 6,
          "blank": 10
        }
      }
    }
  ],
  "total": {
    "files": 4,
    "code": 120,
    "comments": 12,
    "blank": 20,
    "total": 152,
    "metrics": {
      "codeRatio": 0.7895,
      "commentRatio": 0.0789,
      "blankRatio": 0.1316,
      "averageLinesPerFile": 38,
      "largestFile": {
        "path": "src/Program.cs",
        "total": 98,
        "code": 82,
        "comments": 6,
        "blank": 10
      }
    }
  }
}
```

### Markdown example

```markdown
| Language | Files | Code | Comments | Blank | Total |
|---|---:|---:|---:|---:|---:|
| C# | 4 | 120 | 12 | 20 | 152 |
| **Total** | **4** | **120** | **12** | **20** | **152** |

## Metrics

- Comment ratio: 7.89 %
- Blank ratio: 13.16 %
- Code ratio: 78.95 %
- Average lines/file: 38
- Largest file: src/Program.cs (98 lines)
```

## Default excluded directories

By default, the recursive scan ignores directories that usually contain generated, restored, build or tooling files:

```text
.git
.vs
bin
obj
node_modules
dist
build
packages
vendor
coverage
```

You can add project-specific exclusions:

```bash
clsoc count . --exclude generated,temp
```

You can also disable the default exclusions:

```bash
clsoc count . --no-default-excludes
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
      FileScanOptions.cs
      FileScanner.cs
      FileCountResult.cs
      LanguageCountResult.cs
      LanguageDefinition.cs
      LanguageRegistry.cs
      LineCounter.cs
      LineCountResult.cs
      ProjectCounter.cs
    Reporting/
      CountReportFormatter.cs
      ReportFormat.cs
      ReportFormatParser.cs

tests/
  clsoc.Tests/
    ContatoreTests.cs
    FileScannerTests.cs
    LanguageRegistryTests.cs
    LineCounterTests.cs
    ProjectCounterTests.cs
    CountReportFormatterTests.cs
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
- [x] Add default excluded directories such as `bin`, `obj`, `.git`, `.vs`, `node_modules`
- [x] Add output formats: table, JSON, Markdown, CSV
- [x] Add summary metrics: ratios, average lines per file and largest file
- [ ] Add optional configuration file `clsoc.json`
- [ ] Package as a .NET global tool
