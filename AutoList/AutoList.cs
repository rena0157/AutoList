// AutoList
// AutoList.cs
// 
// ============================================================
// 
// Created: 2018-10-10
// Last Updated: 2018-10-10-8:16 PM
// By: Adam Renaud
// 
// ============================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AutoList
{
    public static class AutoList
    {
        /// <summary>
        ///     Returns a dataLists of text objects that are in a string
        ///     with the "text" group name within their <see cref="pattern" />
        /// </summary>
        /// <param name="inputText">The input string</param>
        /// <param name="pattern">The <see cref="Regex" /> Pattern</param>
        /// <returns>
        ///     A dataLists of strings that match the patterns and are decorated with
        ///     the "text" group names
        /// </returns>
        public static List<string> GetText(string inputText, string pattern)
        {
            var returnList = new List<string>();

            // Separate the matches in the dataLists into their groups and
            // add them to the return dataLists
            foreach ( Match match in Regex.Matches(inputText, pattern) )
                returnList.Add(match.Groups["text"].Value.TrimEnd());

            return returnList;
        }

        /// <summary>
        ///     Returns a double dataLists of numbers that are from a text string.
        ///     These numbers are converted to doubles and are within the "number" group
        ///     of a regular expression
        /// </summary>
        /// <param name="inputText">The input string</param>
        /// <param name="pattern">The pattern</param>
        /// <returns>A dataLists of doubles</returns>
        public static List<double> GetDouble(string inputText, string pattern)
        {
            // The return list
            var returnList = new List<double>();
            foreach ( Match match in Regex.Matches(inputText, pattern) )
                if ( double.TryParse(match.Groups["number"].Value, out var tempNum) )
                    returnList.Add(tempNum);

            return returnList;
        }

        /// <summary>
        /// Uses the <see cref="GetDouble"/> and <see cref="GetText"/> functions
        /// to grab lines, polylines, hatches and text objects. This function will return
        /// a CSV formatted string with the following headers "BlockID,Frontage,Area"
        /// </summary>
        /// <param name="inputText">Input text from AutoCAD List command</param>
        /// <returns>A CSV formatted String</returns>
        public static string GetBlocks(string inputText)
        {
            var textObjects = GetText(inputText, AutoListPatterns.TextPattern);
            var lengths = GetDouble(inputText, AutoListPatterns.LinesLengthPattern);
            var areas = GetDouble(inputText, AutoListPatterns.HatchAreaPattern);

            const string orderValidationPattern = @"(LINE|LWPOLYLINE|HATCH|TEXT|MTEXT)";
            var matches = Regex.Matches(inputText, orderValidationPattern);
            var requiresZero = new BitArray(matches.Count);
            requiresZero.SetAll(false);

            for ( var matchIndex = 0; matchIndex < matches.Count - 1; ++matchIndex )
            {
                var currentMatch = matches[matchIndex];
                var nextMatch = matches[matchIndex + 1];

                if ( ( currentMatch.Value == "TEXT" || currentMatch.Value == "MTEXT" )
                     && ( nextMatch.Value != "LWPOLYLINE" || nextMatch.Value != "LINE" ) )
                    requiresZero[matchIndex + 1] = true;
            }

            var adjustedLengths = new List<double>(textObjects.Capacity);
            for ( var index = 0; index < textObjects.Count; ++index )
                if ( requiresZero[index] )
                    adjustedLengths.Add(0);
                else
                    adjustedLengths.Add(lengths[index]);

            return ExportCsv("Block ID,Frontage,Area", textObjects, adjustedLengths, areas);
        }

        /// <summary>
        /// Generic Function for writing data to a CSV file. This function
        /// takes a string for headers and a list of lists for the data. Each of the
        /// data lists are the columns in table.
        /// </summary>
        /// <param name="headers">The headers of the CSV file</param>
        /// <param name="dataLists">A list of lists that are the columns in the CSV File</param>
        /// <returns>A string formatted as a CSV file</returns>
        public static string ExportCsv(string headers, params IList[] dataLists)
        {
            var itemsPerList = dataLists[0].Count;
            if ( dataLists.Any(l => l.Count != itemsPerList) )
                throw new ArgumentException("Lists Must be all the same size");

            var sb = new StringBuilder();
            sb.Append(headers + ",\n");
            // Each run through the loop will write a full row in the string
            for ( var index = 0; index < itemsPerList; ++index )
            {
                // Each run through the loop will write each data to the string
                foreach ( var dataList in dataLists )
                    sb.Append(dataList[index] + ",");
                sb.Append("\n");
            }

            return sb.ToString();
        }
    }
}