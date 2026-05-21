# Packaging clsoc

`clsoc` can be packaged as a .NET global tool.

## Build and test first

```bash
dotnet build clsoc.sln
dotnet test clsoc.sln
```

## Create the NuGet package

```bash
dotnet pack clsoc/clsoc.csproj -c Release -o artifacts/packages
```

This creates a `.nupkg` file under:

```text
artifacts/packages
```

## Install from the local package folder

```bash
dotnet tool install --global clsoc --add-source artifacts/packages
```

After installation, the command should be available as:

```bash
clsoc count .
```

## Update an already installed local version

When the tool is already installed globally, uninstall and install it again from the local package folder:

```bash
dotnet tool uninstall --global clsoc
dotnet tool install --global clsoc --add-source artifacts/packages
```

## Uninstall

```bash
dotnet tool uninstall --global clsoc
```

## NuGet metadata

The package metadata is defined in:

```text
clsoc/clsoc.csproj
```

Important properties:

| Property | Meaning |
|---|---|
| `PackAsTool` | Enables packaging as a .NET tool. |
| `ToolCommandName` | Defines the installed command name, currently `clsoc`. |
| `PackageId` | Defines the NuGet package id, currently `clsoc`. |
| `PackageVersion` | Defines the package version, currently `0.1.0`. |

Before a public NuGet release, verify that the selected `PackageId` is available and decide whether the project should use a more unique id.

## Publish as a single-file executable

For a simple distributable executable under `artifacts`, use:

```bash
make publish
```

Current target:

```text
artifacts/publish/win-x64/clsoc.exe
```

The publish command uses:

```bash
dotnet publish clsoc/clsoc.csproj -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true -o artifacts/publish/win-x64
```

This is a framework-dependent single-file publish: it produces one executable file, but the target machine must have the .NET 8 runtime installed.

