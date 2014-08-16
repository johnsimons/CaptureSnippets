using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CaptureSnippets
{
    public static class MarkdownProcessor
    {
        public static ProcessResult Apply(List<Snippet> snippets, string inputFile)
        {
            using (var reader = File.OpenText(inputFile))
            {
                return Apply(snippets, reader);
            }
        }

        public static ProcessResult ApplyToText(List<Snippet> availableSnippets, string markdownContent)
        {
            using (var reader = new StringReader(markdownContent))
            {
                return Apply(availableSnippets, reader);
            }
        }

        static ProcessResult Apply(List<Snippet> availableSnippets, TextReader reader)
        {
            var stringBuilder = new StringBuilder();
            var lookup = new Dictionary<string, Snippet>(StringComparer.OrdinalIgnoreCase);
            foreach (var snippet in availableSnippets)
            {
                lookup[snippet.Key] = snippet;
            }
            var result = new ProcessResult();

            string line;
            var eatingCode = false;
            var eatingCodePending = false;
            var index = 0;
            while ((line = reader.ReadLine()) != null)
            {
                index++;
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
                stringBuilder.AppendLine(line);

                string key;
                if (!TryExtractKeyFromLine(line, out key))
                {
                    continue;
                }
                Snippet codeSnippet;
                if (!lookup.TryGetValue(key, out codeSnippet))
                {
                    var missingSnippet = new MissingSnippet
                    {
                        Key = key,
                        Line = index
                    };
                    result.MissingSnippet.Add(missingSnippet);
                    stringBuilder.AppendLine(string.Format("** Could not find key '{0}' **", key));
                    continue;
                }
                var value = codeSnippet.Value;

                stringBuilder.AppendLine("```");
                stringBuilder.AppendLine(value);
                stringBuilder.AppendLine("```");
                eatingCodePending = true;
                result.UsedSnippets.Add(codeSnippet);
            }
            result.Text = stringBuilder.ToString().TrimTrailingNewLine();
            return result;
        }

        public static bool TryExtractKeyFromLine(string line, out string key)
        {
            var indexOfImport = line.IndexOf("<!-- import ");
            if (indexOfImport == -1)
            {
                key = null;
                return false;
            }
            var substring = line.Substring(indexOfImport + 12);
            var indexOfFinish = substring.IndexOf(" -->");
            if (indexOfFinish == -1)
            {
                key = null;
                return false;
            }
            key = substring.Substring(0, indexOfFinish);
            return true;
        }


    }
}