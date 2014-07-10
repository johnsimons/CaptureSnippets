using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CaptureSnippets
{
    public static class MarkdownProcessor
    {
        public static ProcessResult Apply(List<Snippet> snippets, string inputFile)
        {
            var baselineText = File.ReadAllText(inputFile);

            return ApplyToText(snippets, baselineText);
        }

        public static ProcessResult ApplyToText(List<Snippet> availableSnippets, string markdownContent)
        {
            CheckMissingKeys(availableSnippets, markdownContent);
            var result = new ProcessResult();
            foreach (var snippet in availableSnippets)
            {
                // TODO: this won't change the text 
                // if a snippet is unchanged
                // so we need more context
                var output = ProcessMatch(snippet.Key, snippet.Value, markdownContent);

                if (!string.Equals(output, markdownContent))
                {
                    // we may have added in a snippet
                    result.UsedSnippets.Add(snippet);
                }

                markdownContent = output;
            }

            result.Text = markdownContent;
            return result;
        }

        public static void CheckMissingKeys(List<Snippet> availableSnippets, string baselineText)
        {
            var stringReader = new StringReader(baselineText);
            var stringBuilder = new StringBuilder();
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
                        if (availableSnippets.Any(x => x.Key == key))
                        {
                            continue;
                        }
                        stringBuilder.AppendFormat("Could not find a CodeSnippet for the key '{0}'. LineNumber:{1}", key, lineNumber);
                        stringBuilder.AppendLine();
                    }
                }
            }
            if (stringBuilder.Length > 0)
            {
                throw new Exception(stringBuilder.ToString());
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