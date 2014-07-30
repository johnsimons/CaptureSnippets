using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaptureSnippets
{
   public  static class StringExtensions
    {
        public static IEnumerable<string> TrimIndentation(this IEnumerable<string> snippetLines)
        {
            string initialPadding = null;
            foreach (var line in snippetLines)
            {
                if (initialPadding == null)
                {
                    initialPadding = new String(line.TakeWhile(char.IsWhiteSpace).ToArray());
                }
                yield return line.RemoveStart(initialPadding);
            }
        }
        public static string ReadUntilNotCharacter(this string line)
        {
            var stringBuilder = new StringBuilder();
            foreach (var ch in line)
            {
                if (!Char.IsLetterOrDigit(ch))
                {
                    break;
                }
                stringBuilder.Append(ch);
            }
            return stringBuilder.ToString();
        }
        public static IEnumerable<string> ExcludeEmptyPaddingLines(this IEnumerable<string> snippetLines)
        {
            var list = snippetLines.ToList();
            while (list.Count > 0 && list[0].IsWhiteSpace())
            {
                list.RemoveAt(0);
            }
            while (list.Count > 0 && list[list.Count - 1].IsWhiteSpace())
            {
                list.RemoveAt(list.Count - 1);
            }
            return list;
        }

        public static bool IsWhiteSpace(this string target)
        {
            return string.IsNullOrWhiteSpace(target);
        }
        public static string RemoveStart(this string target, int count)
        {
            return target.Substring(count, target.Length - count);
        }
        public static string RemoveStart(this string target, string toRemove)
        {
            if (target.StartsWith(toRemove))
            {
                var count = toRemove.Length;
                return target.RemoveStart(count);
            }
            return target;
        }

        public static string TrimTrailingNewLine(this string target)
        {
            return target.TrimEnd('\r', '\n'); 
        }
        public static bool IsMdCodeDelimiter(this string line)
        {
            return line.StartsWith("```");
        }

    }
}