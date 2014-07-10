using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CaptureSnippets
{
    public class DocumentFileProcessor
    {
        string docsFolder;

        public DocumentFileProcessor(string docsFolder)
        {
            this.docsFolder = docsFolder;
        }

        public DocumentProcessResult Apply(List<CodeSnippet> snippets)
        {
            var result = new DocumentProcessResult();

            var inputFiles = new[] { "*.md", "*.mdown", "*.markdown" }.SelectMany(
              extension => Directory.GetFiles(docsFolder, extension, SearchOption.AllDirectories))
              .ToArray();

            result.Count = inputFiles.Count();

            foreach (var inputFile in inputFiles)
            {
                var fileResult = Apply(snippets, inputFile);

                if (fileResult.RequiredSnippets.Any())
                {
                    // give up if we can't continue
                    result.Include(fileResult.RequiredSnippets);
                    return result;
                }

                result.Include(fileResult.Snippets);

                File.WriteAllText(inputFile, fileResult.Text);
            }

            return result;
        }

        public static FileProcessResult Apply(List<CodeSnippet> snippets, string inputFile)
        {
            var baselineText = File.ReadAllText(inputFile);

            var result = ApplyToText(snippets, baselineText);

            foreach (var missingKey in result.RequiredSnippets)
            {
                missingKey.File = inputFile;
            }
            return result;
        }

        public static FileProcessResult ApplyToText(List<CodeSnippet> snippets, string baselineText)
        {
            var result = new FileProcessResult();

            var missingKeys = CheckMissingKeys(snippets, baselineText).ToList();
            if (missingKeys.Any())
            {
                result.RequiredSnippets = missingKeys;
                result.Text = baselineText;
                return result;
            }
            foreach (var snippet in snippets)
            {
                // TODO: this won't change the text 
                // if a snippet is unchanged
                // so we need more context
                var output = ProcessMatch(snippet.Key, snippet.Value, baselineText);

                if (!string.Equals(output, baselineText))
                {
                    // we may have added in a snippet
                    result.Snippets.Add(snippet);
                }

                baselineText = output;
            }

            result.Text = baselineText;
            return result;
        }

        public static IEnumerable<CodeSnippetReference> CheckMissingKeys(IEnumerable<CodeSnippet> snippets, string baselineText)
        {
            var foundKeys = snippets.Select(m => m.Key);
            var expectedKeys = FindExpectedKeys(baselineText);
            return expectedKeys.Where(k => foundKeys.All(x => x != k.Key));
        }

        static IEnumerable<CodeSnippetReference> FindExpectedKeys(string baselineText)
        {
            var stringReader = new StringReader(baselineText);

            string line;
            var lineNumber = 0;
            while ((line = stringReader.ReadLine()) != null)
            {
                lineNumber++;
                var indexOfImportStart = line.IndexOf("<!-- import ");

                if (indexOfImportStart > -1)
                {
                    var indexOfImportEnd = line.IndexOf(" -->");
                    if (indexOfImportEnd > -1)
                    {
                        var startIndex = indexOfImportStart + 12;
                        var key = line.Substring(startIndex, indexOfImportEnd - startIndex);
                        yield return new CodeSnippetReference
                        {
                            LineNumber = lineNumber,
                            Key = key
                        };
                    }
                }
            }
        }

        static string ProcessMatch(string key, string value, string baseLineText)
        {
            var lookup = string.Format("<!-- import {0} -->", key);

            var codeSnippet = string.Format(
@"```
{0}
```", value);

            var builder = new StringBuilder();
            using (var reader = new StringReader(baseLineText))
            {
                string line;
                var eatingCode = false;
                var eatingCodePending = false;
                while ((line = reader.ReadLine()) != null)
                {
                    if (eatingCodePending)
                    {
                        eatingCodePending = false;
                        if (line.IsMdCodeDelimiter())
                        {
                            eatingCode = true;
                            continue;
                        }
                    }
                    if (eatingCode)
                    {
                        if (line.IsMdCodeDelimiter())
                        {
                            eatingCode = false;
                        }
                        continue;
                    }
                    builder.AppendLine(line);
                    if (line.Contains(lookup))
                    {
                        builder.AppendLine(codeSnippet);
                        eatingCodePending = true;
                    }
                }
            }

            return builder.ToString().TrimTrailingNewLine();
        }

    }
}