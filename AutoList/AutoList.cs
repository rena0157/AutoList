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
using Newtonsoft.Json;

namespace AutoList
{
    public static class AutoList
    {
        /// <summary>
        /// A simple block class
        /// </summary>
        private class Block
        {

            public Block(string id, double frontage, double area)
            {
                Id = id;
                Frontage = frontage;
                Area = area;
            }

            /// <summary>
            /// The ID/name of the block
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// The Frontage of the block
            /// </summary>
            public double Frontage { get; set; }

            /// <summary>
            /// The Area of the block
            /// </summary>
            public double Area { get; set; }
        }

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
        /// <param name="exportOption"></param>
        /// <returns>A formatted response that has information from the List Command</returns>
        public static string GetBlocks(string inputText, ExportOptions exportOption=ExportOptions.csv)
        {
            var textObjects = GetText(inputText, AutoListPatterns.TextPattern);
            var lengths = GetDouble(inputText, AutoListPatterns.LinesLengthPattern);
            var areas = GetDouble(inputText, AutoListPatterns.HatchAreaPattern);

            // If the count of text is not the same as areas throw an exception
            if (textObjects.Count != areas.Count)
                throw new ArgumentException("The number of text objects must be equal to the" +
                                            "number of areas");

            /*
             * Before we can use the data in the lists above we need to make sure that
             * they are the same lengths and that data lines up. Every block has a block ID
             * and area, but not all blocks have to have a frontage. To fix this we need
             * to get a pattern of selection from the list file and then compare that
             * to the expected pattern. If the current item is a text object and the
             * next item is a hatch then we know that we must at a zero in place of the missing
             * data.
             */

            const string orderValidationPattern = @"(LINE|LWPOLYLINE|HATCH|TEXT|MTEXT)";
            var matches = Regex.Matches(inputText, orderValidationPattern);

            // A bit array that is initially all set to zero
            var requiresZero = new BitArray(textObjects.Capacity);
            requiresZero.SetAll(false);

            // The block number relates to the text objects. They are the anchor 
            // that determines when a new block begins
            var blockNumber = 0;

            /*
             * Loop through all of the matches and if the pattern "Text object - Hatch" is
             * found then we know that a zero for the frontage must be placed at that block
             * we then change that blocks require zero position to true in the bit array.
             */
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

            // The list that will contain the adjusted frontage doubles
            var adjustedLengths = new List<double>(textObjects.Capacity);
            var linkedLengths = new LinkedList<double>(lengths);
            var first = linkedLengths.First;

            /*
             * Iterate through the requires zero bit array, if that position requires
             * a zero place it there, otherwise grab the next available data from the
             * frontage line.
             */
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

            // Select appropriate option for the export type
            switch (exportOption)
            {
                case ExportOptions.csv:
                    return ExportCsv("Block ID,Frontage,Area", textObjects, adjustedLengths, areas);
                case ExportOptions.json:
                    return ExportJson(textObjects, adjustedLengths, areas);
                default:
                    return ExportCsv("Block ID,Frontage,Area", textObjects, adjustedLengths, areas);
            }
        }

        /// <summary>
        /// Exporting a series of lists to 
        /// </summary>
        /// <param name="blockIdsList"></param>
        /// <param name="lengthsList"></param>
        /// <param name="areasList"></param>
        /// <returns>A Json String</returns>
        public static string ExportJson(List<string> blockIdsList, List<double> lengthsList, List<double> areasList)
        {
            if (blockIdsList.Count != lengthsList.Count || blockIdsList.Count != areasList.Count)
                throw new JsonSerializationException("Arrays must be the same length");

            var tempList = new List<Block>();
            tempList.AddRange(blockIdsList.Select((t, index) => new Block(t, lengthsList[index], areasList[index])));

            return JsonConvert.SerializeObject(tempList, Formatting.None);
        }

        /// <summary>
        /// Function that takes in headers and a series of data lists then converts this to a
        /// CSV string
        /// </summary>
        /// <param name="headers">The Headers of the CSV file</param>
        /// <param name="dataLists">The data rows of the CSV file</param>
        /// <returns>A string that is the CSV File</returns>
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

    public enum ExportOptions
    {
        csv,
        json
    }
}