using System.Collections.Generic;
using System.Linq;

namespace CaptureSnippets
{
    public class DocumentProcessResult
    {
        public int Count;
        public List<CodeSnippet> SnippetsUsed = new List<CodeSnippet>();
        public List<CodeSnippetReference> SnippetReferences = new List<CodeSnippetReference>();

        public void Include(List<CodeSnippet> snippets)
        {
            foreach (var snippet in snippets)
            {
                if (SnippetsUsed.Any(s => s.Key == snippet.Key))
                {
                    continue;
                }
               
                SnippetsUsed.Add(snippet);
            }
        }

        public void Include(IEnumerable<CodeSnippetReference> references)
        {
            foreach (var reference in references)
            {
                if (SnippetsUsed.Any(s => s.Key == reference.Key))
                {
                    continue;
                }

                SnippetReferences.Add(reference);
            }
            
        }
    }
}