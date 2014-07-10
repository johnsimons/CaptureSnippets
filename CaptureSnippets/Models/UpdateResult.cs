using System.Collections.Generic;

namespace CaptureSnippets
{
    public class UpdateResult
    {
        public int Snippets;
        public int Files;
        public List<ErrorMessage> Warnings = new List<ErrorMessage>();
    }
}