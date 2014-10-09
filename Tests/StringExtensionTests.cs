using System.Collections.Generic;
using CaptureSnippets;
using NUnit.Framework;
using ObjectApproval;

[TestFixture]
public class StringExtensionTests
{
    [Test]
    public void TrimNonCharacters()
    {
        Assert.AreEqual("Foo2", "Foo2".TrimNonCharacters());
        Assert.AreEqual("Foo2", "Foo2 ".TrimNonCharacters());
        Assert.AreEqual("Foo2", "Foo2-".TrimNonCharacters());
        Assert.AreEqual("Foo2", "-Foo2-".TrimNonCharacters());
        Assert.AreEqual("Fo_o2", "-Fo_o2-".TrimNonCharacters());
        Assert.AreEqual("Fo-o2", "_Fo-o2_".TrimNonCharacters());
    }

    [Test]
    public void TrimIndentation()
    {
        var input= new List<string>
        {
            "   Line1",
            "    Line2",
            "   Line2",
        };
        ObjectApprover.VerifyWithJson(input.TrimIndentation());
    }

    [Test]
    public void ExcludeEmptyPaddingLines()
    {
        var input= new List<string>
        {
            "   ",
            "    Line2",
            "   ",
        };
        ObjectApprover.VerifyWithJson(input.ExcludeEmptyPaddingLines());
    }

    [Test]
    public void TrimIndentation_with_mis_match()
    {
        var input= new List<string>
        {
            "      Line2",
            "   ",
            "     Line4",
        };
        ObjectApprover.VerifyWithJson(input.TrimIndentation());
    }

    [Test]
    public void ExcludeEmptyPaddingLines_empty_list()
    {
        var input= new List<string>();
        ObjectApprover.VerifyWithJson(input.ExcludeEmptyPaddingLines());
    }

    [Test]
    public void ExcludeEmptyPaddingLines_whitespace_list()
    {
        var input= new List<string>
        {"",  "  "
        };
        ObjectApprover.VerifyWithJson(input.ExcludeEmptyPaddingLines());
    }

    [Test]
    public void TrimIndentation_no_initial_padding()
    {
        var input= new List<string>
        {
            "Line1",
            "    Line2",
            "   Line2",
        };
        ObjectApprover.VerifyWithJson(input.TrimIndentation());
    }
}