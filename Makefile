.PHONY: run build test check clean pack publish install-local uninstall-local tool z

run:
	dotnet run --project clsoc -- count .

r:
	dotnet run --project clsoc -- count "E:\sviluppo\2026 OpenCad2D"
	
json:
	dotnet run --project clsoc -- count . --format json
	
md:
	dotnet run --project clsoc -- count . --format markdown
	
csv:
	dotnet run --project clsoc -- count . --format csv

build:
	dotnet build clsoc.sln

test:
	dotnet test clsoc.sln

check: build test

pack:
	dotnet pack clsoc/clsoc.csproj -c Release -o artifacts/packages

publish:
	dotnet publish clsoc/clsoc.csproj -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true -o artifacts/publish/win-x64 /p:DebugType=None /p:DebugSymbols=false

install-local: pack
	dotnet tool install --global clsoc --add-source artifacts/packages

uninstall-local:
	dotnet tool uninstall --global clsoc

tool:
	clsoc count .

clean:
	@echo "Removing bin, obj and generated artifact folders..."
ifeq ($(OS),Windows_NT)
	@if exist clsoc for /d /r clsoc %%d in (bin,obj) do @if exist "%%d" rmdir /s /q "%%d"
	@if exist src for /d /r src %%d in (bin,obj) do @if exist "%%d" rmdir /s /q "%%d"
	@if exist tests for /d /r tests %%d in (bin,obj) do @if exist "%%d" rmdir /s /q "%%d"
	@if exist artifacts rmdir /s /q artifacts
else
	find clsoc src tests -type d \( -name bin -o -name obj \) -prune -exec rm -rf {} +
	rm -rf artifacts
endif
	@echo "Clean completed."
