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
  <!-- start code CodeKey -->
  <configSections/>
  <!-- end code CodeKey -->";
        var snippets = SnippetExtractor.GetSnippetsFromText(input,"x.xml");
        ObjectApprover.VerifyWithJson(snippets);
    }

    [Test]
    public void CanExtractFromXmlMissingSpaces()
    {
        var input = @"
  <!--start code CodeKey-->
  <configSections/>
  <!--end code CodeKey-->";
        var snippets = SnippetExtractor.GetSnippetsFromText(input, "x.xml");
        ObjectApprover.VerifyWithJson(snippets);
    }
}