using System.Collections.Generic;
using Scribble.CodeSnippets.Models;

namespace CodeSnippets
{
    public class FileProcessResult
    {
        public string Text;

        public List<CodeSnippet> Snippets = new List<CodeSnippet>();

        public IEnumerable<CodeSnippetReference> RequiredSnippets = new List<CodeSnippetReference>();
    }
}