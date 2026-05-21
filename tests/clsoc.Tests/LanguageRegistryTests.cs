using clsoc;
using Xunit;

namespace clsoc.Tests;

public sealed class LanguageRegistryTests
{
    [Fact]
    public void FindById_WhenLanguageExists_ShouldReturnDefinition()
    {
        LanguageDefinition? language = LanguageRegistry.FindById("csharp");

        Assert.NotNull(language);
        Assert.Equal("C#", language.DisplayName);
    }

    [Fact]
    public void FindByExtensions_WhenExtensionMatches_ShouldReturnLanguage()
    {
        IReadOnlyList<LanguageDefinition> languages = LanguageRegistry.FindByExtensions(new[] { ".xaml" });

        Assert.Single(languages);
        Assert.Equal("xml", languages[0].Id);
    }
}
