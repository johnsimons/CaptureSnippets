using System.IO;
using System.Linq;
using CaptureSnippets;
using NUnit.Framework;

[TestFixture]
public class CodeFileParserTests
{
    [Test]
    public void GetSnippets_ReturnsMultipleResults_AllHaveValues()
    {
        var directory = @"data\get-code-snippets\".ToCurrentDirectory();

        var parser = new SnippetExtractor(directory);
        var actual = parser.Parse(".*code[.]cs");

        Assert.IsTrue(actual.Count > 1);
        Assert.IsTrue(actual.All(c => !string.IsNullOrWhiteSpace(c.Value)));
    }

    [Test]
    public void GetSnippets_ProvidingARegex_ChoosesAllFiles()
    {
        var directory = @"data\use-regexes\".ToCurrentDirectory();

        var parser = new SnippetExtractor(directory);
        var actual = parser.Parse("[.]cs");

        Assert.AreEqual(2, actual.Count);
    }

    [Test]
    public void GetSnippets_ProvidingARegexWithFolder_ChoosesOneFile()
    {
        var directory = @"data\use-regexes\".ToCurrentDirectory();

        var parser = new SnippetExtractor(directory);
        var actual = parser.Parse(@".*want.*[.]cs");

        Assert.AreEqual(1,actual.Count);
    }


    [Test]
    public void ApplySnippets_UsingFile_MatchesExpectedResult()
    {
        var directory = @"data\apply-snippets\".ToCurrentDirectory();
        var inputFile = Path.Combine(directory, @"input.md");
        var outputFile = Path.Combine(directory, @"output.md");

        var parser = new SnippetExtractor(directory);
        var snippets = parser.Parse(".*code[.]cs");

        var result = MarkdownProcessor.ApplyToText(snippets, File.ReadAllText(inputFile));

        var expected = File.ReadAllText(outputFile).FixNewLines();
        var actual = result.Text.FixNewLines();
        Assert.AreEqual(expected, actual);
    }
}