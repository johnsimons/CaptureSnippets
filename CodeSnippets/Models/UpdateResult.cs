using System.Collections.Generic;

namespace CodeSnippets
{
    public class UpdateResult
    {
     
        public bool Completed;

        public int Snippets;

        public int Files;

        public List<ScribbleMessage> Warnings = new List<ScribbleMessage>();

        public List<ScribbleMessage> Errors = new List<ScribbleMessage>();

        public long ElapsedMilliseconds;
    }
}