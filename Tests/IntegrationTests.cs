using System.Diagnostics;
using CaptureSnippets;
using NUnit.Framework;

[TestFixture]
public class IntegrationTests
{
    [Test]
    [Explicit]
    public void Foo()
    {
        var parser = new SnippetExtractor(@"C:\Code\Particular\docs.particular.net\Snippets");
        var snippets = parser.Parse("*.cs");
        Debug.WriteLine(snippets.Count);
    }
}