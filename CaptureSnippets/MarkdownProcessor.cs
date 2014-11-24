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
            var lookup = new Dictionary<string, List<Snippet>>(StringComparer.OrdinalIgnoreCase);
            foreach (var snippet in availableSnippets)
            {
                if (!lookup.ContainsKey(snippet.Key))
                {
                    lookup[snippet.Key] = new List<Snippet> {snippet};
                }
                else
                {
                    lookup[snippet.Key].Add(snippet);
                }
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
                List<Snippet> codeSnippets;
                if (!lookup.TryGetValue(key, out codeSnippets))
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

                if (codeSnippets.Count == 1)
                {
                    var codeSnippet = codeSnippets[0];
                    AppendCodeSnippet(codeSnippet, stringBuilder);
                    result.UsedSnippets.Add(codeSnippet);
                }
                else
                {
                    codeSnippets.Sort((s1, s2) => s1.Version.CompareTo(s2.Version));
                    codeSnippets.Reverse();

                    bool first = true;
                    stringBuilder.AppendLine("<ul class=\"nav nav-tabs\" role=\"tablist\">");
                    foreach (var codeSnippet in codeSnippets)
                    {
                        if (first)
                        {
                            stringBuilder.Append("<li role=\"presentation\" class=\"active\">");
                            first = false;
                        }
                        else
                        {
                            stringBuilder.Append("<li role=\"presentation\">");
                        }
                        stringBuilder.AppendFormat(
                            "<a href=\"#{0}\" aria-controls=\"{0}\" role=\"tab\" data-toggle=\"tab\">{1}</a></li>"
                            , MakeTabId(codeSnippet), codeSnippet.Version.ToString(2));
                    }
                    stringBuilder.AppendLine("</ul>");

                    first = true;
                    stringBuilder.AppendLine("<div class=\"tab-content\">");
                    foreach (var codeSnippet in codeSnippets)
                    {
                        stringBuilder.AppendLine(
                            String.Format("<div role=\"tabpanel\" class=\"tab-pane {1}\" id=\"{0}\">",
                            MakeTabId(codeSnippet), first ? "active" : String.Empty));
                        AppendCodeSnippet(codeSnippet, stringBuilder);
                        stringBuilder.AppendLine("</div>");
                        first = false;
                        result.UsedSnippets.Add(codeSnippet);
                    }
                    stringBuilder.AppendLine("</div>");
                }

                eatingCodePending = true;

            }
            result.Text = stringBuilder.ToString().TrimTrailingNewLine();
            return result;
        }

        private static string MakeTabId(Snippet codeSnippet)
        {
            if (codeSnippet.Version.Minor > 0)
            {
                return string.Format("{0}{1}_{2}", codeSnippet.Key, codeSnippet.Version.Major, codeSnippet.Version.Minor);
            }

            return codeSnippet.Key + codeSnippet.Version.Major;
        }

        private static void AppendCodeSnippet(Snippet codeSnippet, StringBuilder stringBuilder)
        {
            var value = codeSnippet.Value;

            if (codeSnippet.Language == null)
            {
                stringBuilder.AppendLine("<pre><code>");
            }
            else
            {
                stringBuilder.AppendLine("<pre><code class=\"" + codeSnippet.Language + "\">");
            }
            stringBuilder.AppendLine(value);
            stringBuilder.AppendLine("</code></pre>");
        }

        public static bool TryExtractKeyFromLine(string line, out string key)
        {
            line = line.Replace("  ", " ");
            var indexOfImport = line.IndexOf("<!-- import ");
            var charsToTrim = 12;
            if (indexOfImport == -1)
            {
                charsToTrim = 11;
                indexOfImport = line.IndexOf("<!--import ");
                if (indexOfImport == -1)
                {
                    key = null;
                    return false;
                }
            }
            var substring = line.Substring(indexOfImport + charsToTrim);
            var indexOfFinish = substring.IndexOf("-->");
            if (indexOfFinish == -1)
            {
                key = null;
                return false;
            }
            key = substring.Substring(0, indexOfFinish)
                .TrimNonCharacters();
            return true;
        }


    }
}