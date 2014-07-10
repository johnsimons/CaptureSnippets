using System;
using System.Text.RegularExpressions;

namespace CaptureSnippets
{
    class RegexParser
    {
        public static bool TryGetRegex(string expression, out Regex regex)
        {
            regex = null;
            if (expression.StartsWith("*"))
            {
                return false;
            }

            try
            {
                regex = new Regex(expression);
                return true;
            }
            catch (ArgumentException)
            {
            }
            return false;
        }
    }
}