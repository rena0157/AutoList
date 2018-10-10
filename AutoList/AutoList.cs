using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace AutoList
{
    public static class AutoList
    {
        public static List<string> GetText(string inputText, string pattern)
        {
            var returnList = new List<string>();
            foreach (Match match in Regex.Matches(inputText, pattern))
            {
                returnList.Add(match.Groups["text"].Value.TrimEnd());
            }

            return returnList;
        }

        public static List<double> GetLengths(string inputText, string pattern)
        {
            return new List<double>();
        }

        public static List<double> GetAreas(string inputText, string pattern)
        {
            return new List<double>();
        }

        public static List<string> Export(params IList[] list)
        {
            return new List<string>();
        }
    }
}
