using System.Diagnostics;
using System.Reflection;

namespace CaptureSnippets
{
    static class MethodTimeLogger
    {
        public static void Log(MethodBase methodBase, long milliseconds)
        {
#if DEBUG
            Trace.WriteLine(string.Format("{0} took {1}ms", methodBase.Name, milliseconds));
#endif
        }
    }
}