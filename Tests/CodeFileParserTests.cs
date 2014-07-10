using System.IO;
using System.Linq;
using CaptureSnippets;
using NUnit.Framework;

[TestFixture]
public class CodeFileParserTests
{
    [Test]
    public void GetCodeSnippets_ReturnsMultipleResults_AllHaveValues()
    {
        var directory = @"data\get-code-snippets\".ToCurrentDirectory();

        var parser = new CodeFileParser(directory);
        var actual = parser.Parse(new[] {".*code[.]cs"});

        Assert.IsTrue(actual.Count > 1);
        Assert.IsTrue(actual.All(c => !string.IsNullOrWhiteSpace(c.Value)));
    }

    [Test]
    public void GetCodeSnippets_ProvidingARegex_ChoosesAllFiles()
    {
        var directory = @"data\use-regexes\".ToCurrentDirectory();

        var parser = new CodeFileParser(directory);
        var actual = parser.Parse(new[] {"[.]cs"});

        Assert.IsTrue(actual.Count == 2);
    }

    [Test]
    public void GetCodeSnippets_ProvidingARegexWithFolder_ChoosesOneFile()
    {
        var directory = @"data\use-regexes\".ToCurrentDirectory();

        var parser = new CodeFileParser(directory);
        var actual = parser.Parse(new[] {@".*want.*[.]cs"});

        Assert.IsTrue(actual.Count == 1);
    }


    [Test]
    public void GetCodeSnippets_WithNestedSnippets_ReturnsTwoValues()
    {
        var directory = @"data\get-code-snippets\".ToCurrentDirectory();

        var parser = new CodeFileParser(directory);
        var actual = parser.Parse(new[] {".*nested-code[.]cs"});

        Assert.AreEqual(2, actual.Count);
        Assert.IsTrue(actual.All(c => !string.IsNullOrWhiteSpace(c.Value)));
    }

    [Test]
    public void ApplySnippets_UsingFile_MatchesExpectedResult()
    {
        var directory = @"data\apply-snippets\".ToCurrentDirectory();
        var inputFile = Path.Combine(directory, @"input.md");
        var outputFile = Path.Combine(directory, @"output.md");

        var parser = new CodeFileParser(directory);
        var snippets = parser.Parse(new[] {".*code[.]cs"});

        var result = DocumentFileProcessor.Apply(snippets, inputFile);

        var expected = File.ReadAllText(outputFile).FixNewLines();
        var actual = result.Text.FixNewLines();
        Assert.AreEqual(expected, actual);
    }
}