using System.Collections.Generic;

namespace CaptureSnippets
{
    public class FileProcessResult
    {
        public string Text;

        public List<CodeSnippet> Snippets = new List<CodeSnippet>();

        public List<CodeSnippetReference> RequiredSnippets = new List<CodeSnippetReference>();
    }
}