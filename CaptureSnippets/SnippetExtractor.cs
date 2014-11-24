using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MethodTimer;

namespace CaptureSnippets
{
    public static class SnippetExtractor
    {
        const string LineEnding = "\r\n";

        [Time]
        public static IEnumerable<Snippet> FromFiles(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                using (var stringReader = File.OpenText(file))
                {
                    foreach (var snippet in GetSnippetsFromTextReader(stringReader, file))
                    {
                        yield return snippet;
                    }
                }
            }
        }

        public static IEnumerable<Snippet> FromText(string contents, string file = null)
        {
            using (var reader = new StringReader(contents))
            {
                return GetSnippetsFromTextReader(reader, file);
            }
        }

        static IEnumerable<Snippet> GetSnippetsFromTextReader(TextReader stringReader, string file)
        {
            var innerList = GetSnippetsFromFile(stringReader).ToList();

            foreach (var snippet in innerList)
            {
                snippet.File = file;
                snippet.Language = GetLanguageFromFile(file);
            }
            return innerList;
        }

        static string GetLanguageFromFile(string file)
        {
            var extension = Path.GetExtension(file);
            if (extension != null)
            {
                return extension.TrimStart('.');
            }
            return String.Empty;
        }

        static IEnumerable<Snippet> GetSnippetsFromFile(TextReader stringReader)
        {
            string currentKey = null;
            string version = null;
            var startLine = 0;
            var isInSnippet = false;
            List<string> snippetLines = null;
            Func<string, bool> endFunc = null;
            var lineNumber = 0;
            while (true)
            {
                var line = stringReader.ReadLine();
                if (line == null)
                {
                    if (isInSnippet)
                    {
                        yield return new Snippet
                        {
                            StartRow = startLine + 1,
                            Key = currentKey,
                            Value = "SNIPPET WAS NOT CLOSED",
                            IsUnclosed = true
                        };
                    }
                    break;
                }

                var trimmedLine = line.Trim().Replace("  ", " ").ToLowerInvariant();
                if (isInSnippet)
                {
                    if (endFunc(trimmedLine))
                    {
                        isInSnippet = false;
                        var snippetValue = snippetLines
                            .ExcludeEmptyPaddingLines()
                            .TrimIndentation();
                        yield return new Snippet
                        {
                            StartRow = startLine+1,
                            EndRow = lineNumber+2,
                            Key = currentKey,
                            Version = version != null ? Version.Parse(version) : null,
                            Value = string.Join(LineEnding, snippetValue)
                        };
                        snippetLines = null;
                        currentKey = null;
                        version = null;
                        continue;
                    }
                    snippetLines.Add(line);
                }
                else
                {
                    if (IsStartCode(trimmedLine, out currentKey))
                    {
                        endFunc = IsEndCode;
                        isInSnippet = true;
                        startLine = lineNumber;
                        snippetLines = new List<string>();
                        continue;
                    }
                    if (IsStartRegion(trimmedLine, out currentKey, out version))
                    {
                        endFunc = IsEndRegion;
                        isInSnippet = true;
                        startLine = lineNumber;
                        snippetLines = new List<string>();
                    }
                }
                lineNumber++;
            }
        }


        private static bool IsEndRegion(string line)
        {
            return line.IndexOf("#endregion", StringComparison.Ordinal) >= 0;
        }

        private static bool IsEndCode(string line)
        {
            return line.IndexOf("endcode", StringComparison.Ordinal) >= 0;
        }

        public static bool IsStartCode(string line, out string key)
        {
            var startCodeIndex = line.IndexOf("startcode ", StringComparison.Ordinal);
            if (startCodeIndex != -1)
            {
                var startIndex = startCodeIndex + 10;
                var splitBySpace = line.Substring(startIndex)
                    .Split(new[]{' '},StringSplitOptions.RemoveEmptyEntries);
                if (splitBySpace.Any())
                {
                    key = splitBySpace
                        .First()
                        .TrimNonCharacters();
                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        return true;
                    }
                }
            }
            key = null;
            return false;
        }

        static bool IsStartRegion(string line, out string key, out string version)
        {
            var startCodeIndex = line.IndexOf("#region ", StringComparison.Ordinal);

            version = null;

            if (startCodeIndex != -1)
            {
                var startIndex = startCodeIndex + 8;
                var splitBySpace = line.Substring(startIndex)
                    .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (splitBySpace.Any())
                {
                    key = splitBySpace.First();

                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        if (splitBySpace.Length > 1)
                        {
                            version = splitBySpace.Last();
                        }

                        return true;
                    }
                }
            }

            key = null;
            return false;
        }


    }
}
