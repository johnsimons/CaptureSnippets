using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MethodTimer;

namespace CaptureSnippets
{
    public class SnippetExtractor
    {
        const string LineEnding = "\r\n";

        string codeFolder;

        public SnippetExtractor(string codeFolder)
        {
            this.codeFolder = codeFolder;
        }

        [Time]
        public List<Snippet> Parse(string[] filterOnExpression)
        {
            var filesMatchingExtensions = new List<string>();

            foreach (var expression in filterOnExpression)
            {
                var collection = FindFromExpression(expression);
                filesMatchingExtensions.AddRange(collection);
            }
            var snippets = GetSnippets(filesMatchingExtensions.Where(x => !x.Contains(@"\obj\"))
                .Distinct());

            ThrowForIncompleteSnippets(snippets);
            return snippets;
        }

        static void ThrowForIncompleteSnippets(List<Snippet> snippets)
        {
            var incompleteSnippets = snippets.Where(s => string.IsNullOrWhiteSpace(s.Value)).ToList();
            if (!incompleteSnippets.Any())
            {
                return;
            }
            var stringBuilder = new StringBuilder();
            foreach (var incompleteSnippet in incompleteSnippets)
            {
                stringBuilder.AppendFormat("Code snippet reference '{0}' was not closed (specify 'end code {0}'). File:{1} LineNumber: {2}", incompleteSnippet.Key, incompleteSnippet.File, incompleteSnippet.StartRow);
                stringBuilder.AppendLine();
            }
            throw new Exception(stringBuilder.ToString());
        }

        IEnumerable<string> FindFromExpression(string expression)
        {
            Regex regex;
            if (RegexParser.TryGetRegex(expression, out regex))
            {
                var allFiles = Directory.GetFiles(codeFolder, "*.*", SearchOption.AllDirectories);
                return allFiles.Where(f => regex.IsMatch(f));
            }
            return Directory.GetFiles(codeFolder, expression, SearchOption.AllDirectories);
        }


        [Time]
        public static List<Snippet> GetSnippets(IEnumerable<string> codeFiles)
        {
            var snippets = new List<Snippet>();

            foreach (var file in codeFiles)
            {
                //Reading 
                var contents = File.ReadAllText(file);
                if (!contents.Contains("start code "))
                {
                    continue;
                }

                var innerList = GetSnippetsFromText(contents, file);
                snippets.AddRange(innerList);
            }

            return snippets;
        }

        public static IEnumerable<Snippet> GetSnippetsFromText(string contents, string file)
        {
            var lines = contents.Split(new[]
            {
                "\r\n", "\n"
            }, StringSplitOptions.None);
            var innerList = GetSnippetsFromFile(lines).ToList();
            foreach (var snippet in innerList)
            {
                snippet.File = file;
                snippet.Language = GetLanguageFromFile(file);
            }
            return innerList;
        }

        static string GetLanguageFromFile(string file)
        {
            return Path.GetExtension(file);
        }

        static IEnumerable<Snippet> GetSnippetsFromFile(string[] lines)
        {
            var innerList = new List<Snippet>();

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                string startKey;
                if (TryExtractStartKey(line, out startKey))
                {
                    innerList.Add(new Snippet
                    {
                        Key = startKey,
                        StartRow = i + 1,
                    });
                    continue;
                }

                string endKey;
                if (TryExtractEndKey(line, out endKey))
                {
                    var existing = innerList.FirstOrDefault(c => c.Key == endKey);
                    if (existing == null)
                    {
                        // TODO: message about failure
                    }
                    else
                    {
                        existing.EndRow = i;
                        var count = existing.EndRow - existing.StartRow;
                        var snippetLines = lines.Skip(existing.StartRow)
                            .Take(count)
                            .Where(IsNotCodeSnippetTag).ToList();
                        snippetLines = snippetLines
                            .ExcludeEmptyPaddingLines()
                            .TrimIndentation()
                            .ToList();
                        existing.Value = string.Join(LineEnding, snippetLines);
                    }
                }
            }
            return innerList;
        }

        public static bool TryExtractEndKey(string line, out string key)
        {
            return TryExtractKey(line,"end code ", out key);
        }
        public static bool TryExtractStartKey(string line, out string key)
        {
            return TryExtractKey(line,"start code ", out key);
        }

        static bool TryExtractKey(string line, string splitter, out string key)
        {
            var indexOfEndCode = line.IndexOf(splitter);
            if (indexOfEndCode != -1)
            {
                var startIndex = indexOfEndCode + splitter.Length;
                var suffix = line.RemoveStart(startIndex);
                var split = suffix.Split(new char[]
                {
                }, StringSplitOptions.RemoveEmptyEntries);
                key = split.First();
                return true;
            }
            key = null;
            return false;
        }


        static bool IsNotCodeSnippetTag(string line)
        {
            return !line.Contains("end code ") && !line.Contains("start code ");
        }
    }
}
