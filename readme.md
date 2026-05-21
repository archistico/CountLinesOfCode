# CountLinesOfCode

Small command line utility that counts lines of code for files with a selected extension.

This repository is being modernized incrementally. The current version keeps the original counting behavior, but the project now targets .NET 8 and includes baseline tests so future parser changes can be made safely.

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

## Current limitations

The parser still reflects the original behavior and is intentionally simple. It does not yet correctly handle all cases, for example:

- inline block comments such as `int x = 1; /* comment */`
- block comments that open and close on the same line
- comment markers inside string literals
- language-specific comment syntax
- generated/build folders such as `bin`, `obj`, `.git`, `node_modules`

These limitations are documented so they can be fixed in the next phases with tests.

## Roadmap

1. Modernize project and add baseline tests — done
2. Separate core counting logic from CLI output
3. Improve comment parsing
4. Add language definitions
5. Add directory exclusions and configuration
6. Add table, JSON, Markdown and CSV output
7. Package as a .NET global tool
