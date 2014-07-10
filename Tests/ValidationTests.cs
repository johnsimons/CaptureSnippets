using System.IO;
using System.Linq;
using CaptureSnippets;
using NUnit.Framework;

[TestFixture]
public class ValidationTests
{
    [Test]
    public void When_Tag_Found_In_Docs_But_Not_Found_In_Code_Returns_False()
    {
        var directory = @"data\validation\no-snippets\".ToCurrentDirectory();

        var codeFolder = Path.Combine(directory, @"source\");
        var docsFolder = Path.Combine(directory, @"docs\");

        Assert.Throws<ParseException>(() => CodeImporter.UpdateDirectory(codeFolder, new[] {"*.cs"}, docsFolder));
    }

    [Test]
    public void When_Tag_Found_In_Docs_But_Not_Found_In_Code_Display_Error()
    {
        var directory = @"data\validation\no-snippets\".ToCurrentDirectory();

        var codeFolder = Path.Combine(directory, @"source\");
        var docsFolder = Path.Combine(directory, @"docs\");

        var expectedFile = Path.Combine(docsFolder, "index.md");
        var exception = Assert.Throws<ParseException>(() => CodeImporter.UpdateDirectory(codeFolder, new[] { "*.cs" }, docsFolder));

        var error = exception.Errors.First();
        // message explains error
        Assert.AreEqual(error.Message, "Could not find a code snippet for reference 'LinqToJsonCreateParse'");

        // file is as we expected
        Assert.AreEqual(error.File, expectedFile);

        // and we have the right line number to look at
        Assert.AreEqual(15, error.LineNumber);
    }

    [Test]
    public void When_Code_Snippet_Defined_But_Not_Used_Does_Not_Throw()
    {
        var directory = @"data\validation\no-reference\".ToCurrentDirectory();

        var codeFolder = Path.Combine(directory, @"source\");
        var docsFolder = Path.Combine(directory, @"docs\");

        CodeImporter.UpdateDirectory(codeFolder, new[] { "*.cs" }, docsFolder);
    }

    [Test]
    [Explicit]
    public void When_Code_Snippet_Defined_But_Not_Used_In_Docs_Displays_Warning_Message()
    {
        var directory = @"data\validation\no-reference\".ToCurrentDirectory();

        var codeFolder = Path.Combine(directory, @"source\");
        var docsFolder = Path.Combine(directory, @"docs\");

        var expectedFile = Path.Combine(codeFolder, "code.cs");

        var result = CodeImporter.UpdateDirectory(codeFolder, new[] { "*.cs" }, docsFolder);

        var warning = result.Warnings.First();
        // message explains error
        Assert.AreEqual(warning.Message, "Code snippet reference 'LinqToJsonCreateParse' is not used in any pages and can be removed");

        // file is as we expected
        Assert.AreEqual(warning.File, expectedFile);

        // and we have the right line number to look at
        Assert.AreEqual(32, warning.LineNumber);
    }

    [Test]
    public void When_Incomplete_Snippet_Found_Displays_Error_Message()
    {
        var directory = @"data\validation\bad-snippet\".ToCurrentDirectory();

        var codeFolder = Path.Combine(directory, @"source\");
        var docsFolder = Path.Combine(directory, @"docs\");

        var expectedFile = Path.Combine(codeFolder, "code.cs");

        var exception = Assert.Throws<ParseException>(() => CodeImporter.UpdateDirectory(codeFolder, new[] { "*.cs" }, docsFolder));

        var error = exception.Errors.First();

        // message explains error
        Assert.AreEqual(error.Message, "Code snippet reference 'ThisIsAInvalidCodeSnippet' was not closed (specify 'end code ThisIsAInvalidCodeSnippet').");

        // file is as we expected
        Assert.AreEqual(error.File, expectedFile);

        // and we have the right line number to look at
        Assert.AreEqual(30, error.LineNumber);
    }

    [Test]
    public void When_Incomplete_Snippet_Found_Does_Not_Import()
    {
        var directory = @"data\validation\bad-snippet\".ToCurrentDirectory();

        var codeFolder = Path.Combine(directory, @"source\");
        var docsFolder = Path.Combine(directory, @"docs\");

        Assert.Throws<ParseException>(() => CodeImporter.UpdateDirectory(codeFolder, new[] { "*.cs" }, docsFolder));

    }
}