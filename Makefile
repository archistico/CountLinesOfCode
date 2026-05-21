.PHONY: run build test check clean zip z

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

clean:
	@echo "Removing bin and obj folders..."
ifeq ($(OS),Windows_NT)
	@if exist clsoc for /d /r clsoc %%d in (bin,obj) do @if exist "%%d" rmdir /s /q "%%d"
	@if exist tests for /d /r tests %%d in (bin,obj) do @if exist "%%d" rmdir /s /q "%%d"
else
	find src tests -type d \( -name bin -o -name obj \) -prune -exec rm -rf {} +
endif
	@echo "Clean completed."

