using System;
using System.Collections.Generic;

namespace CaptureSnippets
{
    public class ParseException: Exception
    {
        public List<ErrorMessage> Errors { get; set; }
    }
}