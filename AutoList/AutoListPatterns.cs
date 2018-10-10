using System;
using System.Collections.Generic;
using System.Text;

namespace AutoList
{
    public static class AutoListPatterns
    {
        public static string LinesLengthPattern = "";
        public static string PolylinesLengthPattern = "";
        public static string HatchAreaPattern = "";
        public static readonly string TextPattern = 
            @"text\s*(?<text>.*)";
    }

    public enum ExportType
    {
        CSV
    }
}
