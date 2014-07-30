using System.Diagnostics;
using System.IO;
using CaptureSnippets;
using NUnit.Framework;
using ObjectApproval;

[TestFixture]
public class SnippetExtractorTests
{

    [Test]
    [Explicit]
    public void AdHock()
    {
        var input = File.ReadAllText(@"C:\Code\CaptureSnippets\Tests\bin\Debug\data\get-code-snippets\nested-code.cs");
        var snippets = SnippetExtractor.GetSnippetsFromText(input, "x.xml");
        Debug.WriteLine(snippets);
    }
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
    public void CanExtractWithNoTrailingCharacters()
    {
        var input = @"
  // start code CodeKey
  the code
  // end code CodeKey";
        var snippets = SnippetExtractor.GetSnippetsFromText(input,null);
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