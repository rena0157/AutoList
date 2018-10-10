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
        /// <summary>
        /// Returns a list of text objects that are in a string
        /// with the "text" group name within their <see cref="pattern"/>
        /// </summary>
        /// <param name="inputText">The input string</param>
        /// <param name="pattern">The <see cref="Regex"/> Pattern</param>
        /// <returns>A list of strings that match the patterns and are decorated with
        /// the "text" group names</returns>
        public static List<string> GetText(string inputText, string pattern)
        {
            var returnList = new List<string>();

            // Separate the matches in the list into their groups and
            // add them to the return list
            foreach (Match match in Regex.Matches(inputText, pattern))
                returnList.Add(match.Groups["text"].Value.TrimEnd());

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
