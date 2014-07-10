using System.Linq;
using MethodTimer;

namespace CaptureSnippets
{
    public static class CodeImporter
    {
        [Time]
        public static UpdateResult UpdateDirectory(string codeFolder, string[] extensionsToSearch, string docsFolder)
        {
            var result = new UpdateResult();

            var codeParser = new CodeFileParser(codeFolder);
            var snippets = codeParser.Parse(extensionsToSearch);

            var incompleteSnippets = snippets.Where(s => string.IsNullOrWhiteSpace(s.Value)).ToArray();
            if (incompleteSnippets.Any())
            {
                throw new ParseException
                {
                    Errors = incompleteSnippets.FormatIncomplete().ToList()
                };
            }

            result.Snippets = snippets.Count;

            var processor = new DocumentFileProcessor(docsFolder);
            var processResult = processor.Apply(snippets);

            var snippetsNotUsed = snippets.Except(processResult.SnippetsUsed).ToArray();

            var snippetsMissed = processResult.SnippetReferences;

            if (snippetsMissed.Any())
            {
                throw new ParseException
                {
                    Errors = snippetsMissed.FormatNotFound().ToList()
                };
            }

            if (snippetsNotUsed.Any())
            {
                var messages = snippetsNotUsed.FormatUnused();
                result.Warnings.AddRange(messages);
            }

            result.Files = processResult.Count;
            return result;
        }
    }
}