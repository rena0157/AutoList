﻿// AutoList
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
            if (pattern == null)
                throw new ArgumentNullException(nameof(pattern));

            var returnList = new List<double>();
            foreach ( Match match in Regex.Matches(inputText, pattern) )
                if ( double.TryParse(match.Groups["number"].Value, out var tempNum) )
                    returnList.Add(tempNum);

            return returnList;
        }

        /// <summary>
        /// Returns a CSV file that is formatted "Block ID, Frontage Length, Area"
        /// </summary>
        /// <param name="inputText">Input text from the AutoCAD List command</param>
        /// <returns>A formatted response that has information from the List Command</returns>
        public static string GetBlocks(string inputText)
        {
            var textObjects = GetText(inputText, AutoListPatterns.TextPattern);
            var lengths = GetDouble(inputText, AutoListPatterns.LinesLengthPattern);
            var areas = GetDouble(inputText, AutoListPatterns.HatchAreaPattern);

            const string orderValidationPattern = @"(LINE|LWPOLYLINE|HATCH|TEXT|MTEXT)";
            var matches = Regex.Matches(inputText, orderValidationPattern);
            var requiresZero = new BitArray(textObjects.Capacity);
            requiresZero.SetAll(false);
            var blockNumber = 0;

            for ( var matchIndex = 0; matchIndex < matches.Count - 1; ++matchIndex )
            {
                var currentMatch = matches[matchIndex];
                var nextMatch = matches[matchIndex + 1];

                if ( currentMatch.Value == "TEXT" || currentMatch.Value == "MTEXT" )
                    blockNumber++;

                if ( ( currentMatch.Value == "TEXT" || currentMatch.Value == "MTEXT" )
                     && nextMatch.Value == "HATCH" )
                    requiresZero[blockNumber - 1] = true;
            }

            var adjustedLengths = new List<double>(textObjects.Capacity);
            var linkedLengths = new LinkedList<double>(lengths);
            var first = linkedLengths.First;
            for ( var index = 0; index < textObjects.Count; ++index )
                if ( requiresZero[index] )
                {
                    adjustedLengths.Add(0);
                }
                else
                {
                    if (index == 0)
                        adjustedLengths.Add(first.Value);
                    else if ( first.Next != null ) 
                        adjustedLengths.Add(first.Next.Value);
                }

            return ExportCsv("Block ID,Frontage,Area", textObjects, adjustedLengths, areas);
        }

        public static string ExportCsv(string headers, params IList[] dataLists)
        {
            var itemsPerList = dataLists[0].Count;
            if ( dataLists.Any(l => l.Count != itemsPerList) )
                throw new ArgumentException("Lists Must be all the same size");

            var sb = new StringBuilder();
            sb.Append(headers + ",\n");
            // Write Lines
            for ( var index = 0; index < itemsPerList; ++index )
            {
                // Write Data into lines
                foreach ( var dataList in dataLists )
                    sb.Append(dataList[index] + ",");
                sb.Append("\n");
            }

            return sb.ToString();
        }
    }
}