using System.IO;
using CodeSnippets;
using NUnit.Framework;

public class ImporterTests
{
    [Test]
    public void Update_UsingSourceAndDocsFolder_WillReturnCodeSnippetCount()
    {
        var directory = @"data\test-site\".ToCurrentDirectory();

        var codeFolder = Path.Combine(directory, @"source\");
        var docsFolder = Path.Combine(directory, @"docs\");
        var result = CodeImporter.Update(codeFolder, new[] {"*.cs"}, docsFolder);

        Assert.AreEqual(14, result.Snippets);
    }

    [Test]
    public void Update_UsingSourceAndDocsFolder_WillReturnCodeFileCount()
    {
        var directory = @"data\test-site\".ToCurrentDirectory();

        var codeFolder = Path.Combine(directory, @"source\");
        var docsFolder = Path.Combine(directory, @"docs\");
        var result = CodeImporter.Update(codeFolder, new[] {"*.cs"}, docsFolder);

        Assert.AreEqual(1, result.Files);
    }

    [Test]
    public void Update_UsingSourceAndDocsFolder_ReturnsTrue()
    {
        var directory = @"data\test-site\".ToCurrentDirectory();

        var codeFolder = Path.Combine(directory, @"source\");
        var docsFolder = Path.Combine(directory, @"docs\");
        var result = CodeImporter.Update(codeFolder, new[] {"*.cs"}, docsFolder);

        Assert.AreEqual(14, result.Snippets);
    }

    [Test]
    public void Update_UsingSourceAndDocsFolder_WillFormatWithCodeSnippet()
    {
        var directory = @"data\test-site\".ToCurrentDirectory();

        var codeFolder = Path.Combine(directory, @"source\");
        var docsFolder = Path.Combine(directory, @"docs\");
        CodeImporter.Update(codeFolder, new[] {"*.cs"}, docsFolder);

        var indexFile = Path.Combine(directory, @"docs\index.md");
        var actual = File.ReadAllText(indexFile).FixNewLines();

        var outputFile = Path.Combine(directory, @"output.md");
        var expected = File.ReadAllText(outputFile).FixNewLines();

        Assert.AreEqual(expected, actual);
    }
}