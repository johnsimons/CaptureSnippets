namespace CaptureSnippets
{
    public class ErrorMessage
    {
        public string Message;
        public string File;
        public int LineNumber;

        public override string ToString()
        {
            return string.Format("{0}, File: {1}, Line: {2}", Message, File, LineNumber);
        }
    }
}