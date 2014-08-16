using System.Collections.Generic;

namespace CaptureSnippets
{
    public class ProcessResult
    {
        public string Text;

        public List<Snippet> UsedSnippets = new List<Snippet>();
        public List<MissingSnippet> MissingSnippet = new List<MissingSnippet>();

    }

    public class MissingSnippet
    {
        public string Key;
        public int Line;
    }
}