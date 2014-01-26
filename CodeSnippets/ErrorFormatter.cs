using System.Collections.Generic;
using System.Linq;

namespace CaptureSnippets
{
    public static class ErrorFormatter
    {
        public static IEnumerable<ErrorMessage> FormatIncomplete(this IEnumerable<CodeSnippet> snippets)
        {
            return snippets.Where(s => string.IsNullOrWhiteSpace(s.Value))
                           .Select(ToNotFoundMessage);
        }

        static ErrorMessage ToNotFoundMessage(this CodeSnippet snippet)
        {
            return new ErrorMessage
            {
                File = snippet.File,
                LineNumber = snippet.StartRow,
                Message = string.Format("Code snippet reference '{0}' was not closed (specify 'end code {0}').", snippet.Key)
            };
        }

        public static IEnumerable<ErrorMessage> FormatUnused(this IEnumerable<CodeSnippet> snippets)
        {
            return snippets.Select(ToUnusedMessage);
        }

        static ErrorMessage ToUnusedMessage(CodeSnippet snippet)
        {
            return new ErrorMessage
            {
                File = snippet.File,
                LineNumber = snippet.StartRow,
                Message = string.Format("Code snippet reference '{0}' is not used in any pages and can be removed", snippet.Key)
            };
        }

        public static IEnumerable<ErrorMessage> FormatNotFound(this List<CodeSnippetReference> snippets)
        {
            return snippets.Select(ToRequiredMessage);
        }

        static ErrorMessage ToRequiredMessage(this CodeSnippetReference snippet)
        {
            return new ErrorMessage
            {
                File = snippet.File,
                LineNumber = snippet.LineNumber,
                Message = string.Format("Could not find a code snippet for reference '{0}'", snippet.Key)
            };
        }

    }
}