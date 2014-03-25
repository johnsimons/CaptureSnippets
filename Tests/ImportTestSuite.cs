using System.IO;
using CaptureSnippets;
using NUnit.Framework;

[TestFixture]
public class ImportTestSuite
{
    [Test]
    public void RunScenarios()
    {
        var directory = @"scenarios\".ToCurrentDirectory();
        var folders = Directory.GetDirectories(directory);

        foreach (var folder in folders)
        {
            Run(folder, Path.Combine(folder, "input.md"), Path.Combine(folder, "output.md"));
        }
    }

    void Run(string folder, string input, string expectedOutput)
    {
        var parser = new CodeFileParser(folder);
        var snippets = parser.Parse(new[] {".*code[.]cs"});

        var result = DocumentFileProcessor.Apply(snippets, input);

        var expected = File.ReadAllText(expectedOutput).FixNewLines();
        var fixNewLines = result.Text.FixNewLines();
        Assert.AreEqual(expected, fixNewLines,folder);
    }

}
