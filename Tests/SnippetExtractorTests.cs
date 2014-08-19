using CaptureSnippets;
using NUnit.Framework;
using ObjectApproval;

[TestFixture]
public class SnippetExtractorTests
{

    [Test]
    public void CanExtractFromXml()
    {
        var input = @"
  <!-- startcode CodeKey -->
  <configSections/>
  <!-- endcode -->";
        var snippets = SnippetExtractor.GetSnippetsFromText(input, null);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public void CanExtractFromRegion()
    {
        var input = @"
  #region CodeKey
  The Code
  #endregion";
        var snippets = SnippetExtractor.GetSnippetsFromText(input,null);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public void CanExtractWithNoTrailingCharacters()
    {
        var input = @"
  // startcode CodeKey
  the code
  // endcode ";
        var snippets = SnippetExtractor.GetSnippetsFromText(input,null);
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public void CanExtractWithMissingSpaces()
    {
        var input = @"
  <!--startcode CodeKey-->
  <configSections/>
  <!--endcode-->";
        var snippets = SnippetExtractor.GetSnippetsFromText(input, null);
        ObjectApprover.VerifyWithJson(snippets);
    }
    [Test]
    public void CanExtractWithTrailingWhitespace()
    {
        var input = @"
  // startcode CodeKey
  the code
  // endcode   ";
        var snippets = SnippetExtractor.GetSnippetsFromText(input, null);
        ObjectApprover.VerifyWithJson(snippets);
    }
}