using System.Diagnostics;
using System.IO;
using System.Linq;
using CaptureSnippets;
using NUnit.Framework;

[TestFixture]
public class IntegrationTests
{
    [Test]
    [Explicit]
    public void Foo()
    {
        var strings = Directory.GetFiles(@"C:\Code\Particular\docs.particular.net\Content", "*.md", SearchOption.AllDirectories).Select(File.ReadAllText);

        var startNew = Stopwatch.StartNew();
        var parser = new SnippetExtractor(@"C:\Code\Particular\docs.particular.net\Snippets");
        var snippets = parser.Parse("*.cs").ToList();

        foreach (var file in strings)
        {

            var result = MarkdownProcessor.ApplyToText(snippets, file);   
        }
        Debug.WriteLine(startNew.ElapsedMilliseconds);
    }
}