using System.Collections.Generic;

namespace CaptureSnippets
{
    public class UpdateResult
    {
     
        public bool Completed;

        public int Snippets;

        public int Files;

        public List<ErrorMessage> Warnings = new List<ErrorMessage>();

        public List<ErrorMessage> Errors = new List<ErrorMessage>();

        public long ElapsedMilliseconds;
    }
}