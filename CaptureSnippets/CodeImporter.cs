using System.Diagnostics;
using System.Linq;

namespace CaptureSnippets
{
    public static class CodeImporter
    {
        public static UpdateResult Update(string codeFolder, string[] extensionsToSearch, string docsFolder)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = new UpdateResult();

            var codeParser = new CodeFileParser(codeFolder);
            var snippets = codeParser.Parse(extensionsToSearch);

            var incompleteSnippets = snippets.Where(s => string.IsNullOrWhiteSpace(s.Value)).ToArray();
            if (incompleteSnippets.Any())
            {
                var messages = incompleteSnippets.FormatIncomplete();
                result.Errors.AddRange(messages);
                return result;
            }

            result.Snippets = snippets.Count;

            var processor = new DocumentFileProcessor(docsFolder);
            var processResult = processor.Apply(snippets);

            var snippetsNotUsed = snippets.Except(processResult.SnippetsUsed).ToArray();

            var snippetsMissed = processResult.SnippetReferences;

            if (snippetsMissed.Any())
            {
                var messages = snippetsMissed.FormatNotFound();
                result.Errors.AddRange(messages);
            }

            if (snippetsNotUsed.Any())
            {
                var messages = snippetsNotUsed.FormatUnused();
                result.Warnings.AddRange(messages);
            }

            result.Files = processResult.Count;
            result.Completed = !result.Errors.Any();
            result.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            return result;
        }
    }
}