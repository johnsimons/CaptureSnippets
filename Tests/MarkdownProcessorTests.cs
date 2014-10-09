using CaptureSnippets;
using NUnit.Framework;

[TestFixture]
public class MarkdownProcessor_TryExtractKeyFromTests
{

    [Test]
    public void WithDashes()
    {
        string key;
        MarkdownProcessor.TryExtractKeyFromLine("<!-- import my-code-snippet -->", out key);
        Assert.AreEqual("my-code-snippet",key);
    }
    [Test]
    public void Simple()
    {
        string key;
        MarkdownProcessor.TryExtractKeyFromLine("<!-- import mycodesnippet -->", out key);
        Assert.AreEqual("mycodesnippet", key);
    }
    [Test]
    public void ExtraSpace()
    {
        string key;
        MarkdownProcessor.TryExtractKeyFromLine("<!--  import  mycodesnippet  -->", out key);
        Assert.AreEqual("mycodesnippet", key);
    }
}