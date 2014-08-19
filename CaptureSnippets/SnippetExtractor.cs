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
        public List<Snippet> Parse(params string[] filterOnExpressions)
        {
            var filesMatchingExtensions = new List<string>();

            foreach (var expression in filterOnExpressions)
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
                stringBuilder.AppendFormat("Code snippet reference '{0}' was not closed (specify 'endcode {0}'). File:{1} LineNumber: {2}", incompleteSnippet.Key, incompleteSnippet.File, incompleteSnippet.StartRow);
                stringBuilder.AppendLine();
            }
            throw new Exception(stringBuilder.ToString());
        }

        IEnumerable<string> FindFromExpression(string expression)
        {
            Regex regex;
            if (RegexParser.TryGetRegex(expression, out regex))
            {
                return Directory.EnumerateFiles(codeFolder, "*.*", SearchOption.AllDirectories)
                    .Where(f => regex.IsMatch(f));
            }
            return Directory.EnumerateFiles(codeFolder, expression, SearchOption.AllDirectories);
        }


        [Time]
        public static List<Snippet> GetSnippets(IEnumerable<string> codeFiles)
        {
            var snippets = new List<Snippet>();

            foreach (var file in codeFiles)
            {
                using (var stringReader = File.OpenText(file))
                {
                    snippets.AddRange(GetSnippetsFromText(file, stringReader));
                }
            }

            return snippets;
        }

        public static IEnumerable<Snippet> GetSnippetsFromText(string contents, string file)
        {
            using (var stringReader = new StringReader(contents))
            {
                return GetSnippetsFromText(file, stringReader);
            }
        }

        static IEnumerable<Snippet> GetSnippetsFromText(string file, TextReader stringReader)
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
                    break;
                }
                if (isInSnippet)
                {
                    if (endFunc(line))
                    {
                        isInSnippet = false;
                        snippetLines = snippetLines
                            .ExcludeEmptyPaddingLines()
                            .TrimIndentation()
                            .ToList();
                        yield return new Snippet
                        {
                            StartRow = startLine+1,
                            EndRow = lineNumber+2,
                            Key = currentKey,
                            Value = string.Join(LineEnding, snippetLines)
                        };
                        snippetLines = null;
                        currentKey = null;
                        continue;
                    }
                    snippetLines.Add(line);
                }
                else
                {
                    if (IsStartCode(line, out currentKey))
                    {
                        endFunc = IsEndCode;
                        isInSnippet = true;
                        startLine = lineNumber;
                        snippetLines = new List<string>();
                        continue;
                    }
                    if (IsStartRegion(line, out currentKey))
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

        static bool IsStartCode(string line, out string key)
        {
            var startCodeIndex = line.IndexOf("startcode ");
            if (startCodeIndex != -1)
            {
                var startIndex = startCodeIndex + 10;
                key = line.RemoveStart(startIndex)
                    .ReadUntilNotCharacter();
                if (string.IsNullOrWhiteSpace(key))
                {
                    var message = string.Format("Could not extract key from '{0}'.", line);
                    throw new Exception(message);
                }
                return true;
            }
            key = null;
            return false;
        }
        
        static bool IsEndRegion(string line)
        {
            return line.Contains("#endregion");
        }

        static bool IsEndCode(string line)
        {
            return line.Contains("endcode");
        }

        static bool IsStartRegion(string line, out string key)
        {
            var startCodeIndex = line.IndexOf("#region ");
            if (startCodeIndex != -1)
            {
                var startIndex = startCodeIndex + 8;
                key = line.RemoveStart(startIndex);
                key = key.ReadUntilNotCharacter();
                if (string.IsNullOrWhiteSpace(key))
                {
                    var message = string.Format("Could not extract key from '{0}'.", line);
                    throw new Exception(message);
                }
                return true;
            }
            key = null;
            return false;
        }


    }
}
