using System.Collections.Generic;
using System.Linq;
using Scribble.CodeSnippets.Models;

namespace CodeSnippets
{
    public class DocumentProcessResult
    {

        public int Count;

        public IEnumerable<object> Warnings = new object[0];

        public IEnumerable<object> Errors = new object[0];

        public bool HasMessages {
            get { return Warnings.Any() || Errors.Any(); }
        }

        public List<CodeSnippet> SnippetsUsed = new List<CodeSnippet>();
        public List<CodeSnippetReference> SnippetReferences = new List<CodeSnippetReference>();

        public void Include(List<CodeSnippet> snippets)
        {
            foreach (var snippet in snippets)
            {
                if (SnippetsUsed.Any(s => s.Key == snippet.Key))
                    continue;
               
                SnippetsUsed.Add(snippet);
            }
        }

        public void Include(IEnumerable<CodeSnippetReference> references)
        {
            foreach (var reference in references)
            {
                if (SnippetsUsed.Any(s => s.Key == reference.Key))
                    continue;

                SnippetReferences.Add(reference);
            }
            
        }
    }
}