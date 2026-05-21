# CountLinesOfCode

Small command line utility that counts lines of code for files with a selected extension.

This repository is being modernized incrementally. The current version targets .NET 8, has baseline tests, separates the core counting logic from the console executable, and includes a more robust C-like comment parser.

## Requirements

- .NET 8 SDK

## Build

```bash
dotnet build clsoc.sln
```

## Test

```bash
dotnet test clsoc.sln
```

## Use

From the folder you want to analyze:

```bash
dotnet run --project clsoc -- cs
```

The argument is the file extension to analyze. Both `cs` and `.cs` are accepted.

Current output:

```text
Numero file     : 0
Linee totali    : 0
-----------------------
Linee di codice : 0
Linee vuote     : 0
Linee commenti  : 0
```

## Project structure

```text
clsoc.sln
clsoc/
  clsoc.csproj                 Console executable
  Program.cs                   CLI entry point
src/
  clsoc.Core/
    clsoc.Core.csproj          Core library
    Counting/
      Contatore.cs             Backward-compatible facade
      FileScanner.cs           File discovery
      LineCounter.cs           Single-file line counter
      LineCountResult.cs       Count result model
tests/
  clsoc.Tests/
    ContatoreTests.cs
    LineCounterTests.cs
```

`Contatore` is still available for compatibility with the original code and tests, but the real work is now delegated to smaller classes. This prepares the next phases: parser correction, language definitions, exclusions, and richer output formats.

## Current parser behavior

The counter now uses a character-by-character parser for C-like comments. It handles:

- single-line comments with `//`
- legacy VB-style single-line comments with `'` when the line starts as a comment
- block comments with `/* ... */`
- block comments that open and close on the same line
- inline block comments inside code lines
- comment markers inside normal string literals
- simple C# verbatim strings such as `@"..."`

The current reporting model still assigns each physical line to one primary category. A line containing both code and a comment is counted as a code line, preserving the original public counters.

## Current limitations

Some planned features are still intentionally missing:

- language-specific comment definitions
- separate reporting for mixed code/comment lines
- raw C# string literal support
- generated/build folder exclusions such as `bin`, `obj`, `.git`, `node_modules`
- richer output formats and configuration

## Roadmap

1. Modernize project and add baseline tests — done
2. Separate core counting logic from CLI output — done
3. Improve comment parsing — done for C-like baseline cases
4. Add language definitions
5. Add directory exclusions and configuration
6. Add table, JSON, Markdown and CSV output
7. Package as a .NET global tool
