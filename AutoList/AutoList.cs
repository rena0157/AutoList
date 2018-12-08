// AutoList
// AutoList.cs
// 
// ============================================================
// 
// Created: 2018-10-10
// Last Updated: 2018-12-07-03:57 PM
// By: Adam Renaud
// 
// ============================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace AutoList
{
    /// <summary>
    ///     AutoList Main Class
    /// </summary>
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
            if ( pattern == null )
                throw new ArgumentNullException(nameof(pattern));

            var returnList = new List<double>();
            foreach ( Match match in Regex.Matches(inputText, pattern) )
                if ( double.TryParse(match.Groups["number"].Value, out var tempNum) )
                    returnList.Add(tempNum);

            return returnList;
        }

        /// <summary>
        ///     Returns a CSV file that is formatted "Block ID, Frontage Length, Area"
        /// </summary>
        /// <param name="inputText">Input text from the AutoCAD List command</param>
        /// <param name="exportOption"></param>
        /// <returns>A formatted response that has information from the List Command</returns>
        public static string GetBlocks(string inputText, ExportOptions exportOption = ExportOptions.Csv)
        {
            var textObjects = GetText(inputText, AutoListPatterns.TextPattern);
            var lengths = GetDouble(inputText, AutoListPatterns.LinesLengthPattern);
            var areas = GetDouble(inputText, AutoListPatterns.HatchAreaPattern);

            /*
                This validation pattern is used to determine the order of
                objects in the List Output
             */
            const string orderValidationPattern = @"(LINE|LWPOLYLINE|HATCH|TEXT|MTEXT)";
            var matches = Regex.Matches(inputText, orderValidationPattern);

            var textIndex = 0;
            var lineIndex = 0;
            var areaIndex = 0;

            string currentText = null;
            double currentLength = 0;
            double currentArea = 0;
            var blocks = new List<Block>(textObjects.Capacity);

            /*
                Loop through the validation pattern. Use the pattern to determine the order of
                objects that were selected. Use this information to build the current block 
                and place it into the list of blocks.
             */
            for ( var matchIndex = 0; matchIndex < matches.Count; ++matchIndex )
            {
                var currentMatch = matches[matchIndex];

                // Get the initial block ID
                if ( currentText == null && ( currentMatch.Value == "TEXT" || currentMatch.Value == "MTEXT" ) )
                {
                    currentText = textObjects[textIndex++];
                    continue;
                }

                // Add length of a line and polyline to the total length
                if ( currentMatch.Value == "LWPOLYLINE" || currentMatch.Value == "LINE" )
                {
                    currentLength += lengths[lineIndex++];
                    continue;
                }

                // If the current item is a hatch then add the area of the hatch
                // to the list
                if ( currentMatch.Value == "HATCH" )
                {
                    currentArea += areas[areaIndex++];
                    continue;
                }

                // Build the block and place it into the list
                if ( currentText != null && ( currentMatch.Value == "TEXT" || currentMatch.Value == "MTEXT" ) )
                {
                    blocks.Add(new Block(currentText, currentLength, currentArea));
                    currentText = textObjects[textIndex++];
                    currentLength = 0;
                    currentArea = 0;
                }
            }

            // Build the final block
            if ( blocks.Count < textObjects.Count )
                blocks.Add(new Block(currentText, currentLength, currentArea));

            // Select appropriate option for the export type
            switch ( exportOption )
            {
                case ExportOptions.Csv:
                    return ExportCsv("Block ID,Frontage,Area", blocks);
                case ExportOptions.Json:
                    return ExportJson(blocks);
                default:
                    return ExportCsv("Block ID,Frontage,Area", blocks);
            }
        }

        /// <summary>
        ///     Exporting a series of lists to JSON
        /// </summary>
        /// <param name="blocks">Blocks that are to be serialized</param>
        /// <returns>A Json String</returns>
        public static string ExportJson(List<Block> blocks)
        {
            return JsonConvert.SerializeObject(blocks, Formatting.None);
        }

        /// <summary>
        ///     Function that takes in headers and a series of data lists then converts this to a
        ///     CSV string
        /// </summary>
        /// <param name="headers">The Headers of the CSV file</param>
        /// <param name="blocks">The blocks that will be added to the CSV</param>
        /// <returns>A string that is the CSV File</returns>
        public static string ExportCsv(string headers, IEnumerable<Block> blocks)
        {
            var sb = new StringBuilder();
            sb.Append(headers + ",\n");

            foreach ( var block in blocks )
            {
                var line = $"{block.Id},{block.Frontage},{block.Area},\n";
                sb.Append(line);
            }

            return sb.ToString();
        }

        /// <summary>
        ///     A simple block class
        /// </summary>
        public class Block
        {
            /// <summary>
            ///     Values backend of the Block type
            /// </summary>
            private readonly double[] _values;

            public Block(string id, double frontage = 0, double area = 0)
            {
                Id = id;
                _values = new[] {frontage, area};
            }

            /// <summary>
            ///     The ID/name of the block
            /// </summary>
            public string Id { get; }

            /// <summary>
            ///     The Frontage of the block
            /// </summary>
            public double Frontage => _values[0];

            /// <summary>
            ///     The Area of the block
            /// </summary>
            public double Area => _values[1];
        }
    }

    public enum ExportOptions
    {
        Csv,
        Json
    }
}