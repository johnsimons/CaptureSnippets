using CaptureSnippets;
using NUnit.Framework;

[TestFixture]
public class SnippetExtractor_IsStartCodeTests
{

    [Test]
    public void CanExtractFromXml()
    {
        string key;
        SnippetExtractor.IsStartCode("<!-- startcode CodeKey -->", out key);
        Assert.AreEqual("CodeKey",key);
    }

    [Test]
    public void CanExtractFromXmlWithMissingSpaces()
    {
        string key;
        SnippetExtractor.IsStartCode("<!--startcode CodeKey-->", out key);
        Assert.AreEqual("CodeKey",key);
    }

    [Test]
    public void CanExtractFromXmlWithExtraSpaces()
    {
        string key;
        SnippetExtractor.IsStartCode("<!--  startcode  CodeKey  -->", out key);
        Assert.AreEqual("CodeKey",key);
    }
    [Test]
    public void CanExtractWithNoTrailingCharacters()
    {
        string key;
        SnippetExtractor.IsStartCode("<!-- startcode CodeKey", out key);
        Assert.AreEqual("CodeKey",key);
    }
    [Test]
    public void CanExtractWithUnderScores()
    {
        string key;
        SnippetExtractor.IsStartCode("<!-- startcode Code_Key -->", out key);
        Assert.AreEqual("Code_Key", key);
    }
    [Test]
    public void CanExtractWithUnderScoresOutside()
    {
        string key;
        SnippetExtractor.IsStartCode("<!-- startcode _CodeKey_ -->", out key);
        Assert.AreEqual("CodeKey", key);
    }
    [Test]
    public void CanExtractWithDashes()
    {
        string key;
        SnippetExtractor.IsStartCode("<!-- startcode Code-Key -->", out key);
        Assert.AreEqual("Code-Key", key);
    }
    [Test]
    public void CanExtractWithDashesOutside()
    {
        string key;
        SnippetExtractor.IsStartCode("<!-- startcode -CodeKey- -->", out key);
        Assert.AreEqual("CodeKey", key);
    }

}