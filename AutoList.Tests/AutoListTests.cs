// AutoList.Tests
// AutoListTests.cs
// 
// ============================================================
// 
// Created: 2018-10-10
// Last Updated: 2018-12-07-03:56 PM
// By: Adam Renaud
// 
// ============================================================

using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace AutoList.Tests
{
    /// <summary>
    ///     Testing class for AutoList
    /// </summary>
    public class AutoListTests
    {
        /// <summary>
        ///     Get the ITestOutputHelper using DI
        /// </summary>
        /// <param name="output"></param>
        public AutoListTests(ITestOutputHelper output) { _output = output; }

        /// <summary>
        ///     Simple ReadAllText wrapper
        /// </summary>
        /// <param name="fileName">The file name that will be read</param>
        /// <returns>All of the text from the file</returns>
        private static string ReadFile(string fileName) { return File.ReadAllText(fileName); }

        /// <summary>
        ///     A List of filenames that will be tested
        /// </summary>
        /// <value></value>
        private static readonly string[] Filenames =
        {
            @".\TestFiles\GenericListText.txt",
            @".\TestFiles\BlocksTest.txt",
            @".\TestFiles\BlocksTest_1.txt"
        };

        /// <inheritdoc />
        /// <summary>
        ///     Block Data class for testing
        /// </summary>
        private class BlocksTestData : IEnumerable<object[]>
        {
            // Information that the testing class uses for data
            IEnumerator<object[]> IEnumerable<object[]>.GetEnumerator()
            {
                // CSV Test - Block Data 1
                yield return new object[]
                {
                    @".\TestFiles\BlocksTest.txt",
                    "Block ID,Frontage,Area,\nBlock 1,100,2900,\nBlock 1,0,2900,\n",
                    ExportOptions.Csv
                };

                // CSV Test
                yield return new object[]
                {
                    @".\TestFiles\BlocksTest_1.txt",
                    "Block ID,Frontage,Area,\nBlock 1 - " +
                    "Frontage,50,1450,\nBlock 2 -  No Frontage,0,1450,\nBlock 3 - " +
                    "Frontage,50,1450,\nBlock 4 -  No Frontage,0,1450,\n",
                    ExportOptions.Csv
                };

                // Json Test
                yield return new object[]
                {
                    @".\TestFiles\BlocksTest_1.txt",
                    "[{\"Id\":\"Block 1 - Frontage\",\"Frontage\":50.0,\"Area\":1450.0},{\"Id\":\"Block 2 -  " +
                    "No Frontage\",\"Frontage\":0.0,\"Area\":1450.0},{\"Id\":\"Block 3 - Frontage\",\"Frontage\":50.0," +
                    "\"Area\":1450.0},{\"Id\":\"Block 4 -  No Frontage\",\"Frontage\":0.0,\"Area\":1450.0}]",
                    ExportOptions.Json
                };
            }

            public IEnumerator GetEnumerator() { yield return new object(); }
        }

        /// <summary>
        ///     Output Helper that links tests to the standard output
        /// </summary>
        private readonly ITestOutputHelper _output;

        /// <summary>
        ///     Test Theory that tests the length and area
        ///     from the first filename
        /// </summary>
        /// <param name="expectedDoubles">A list of expected values</param>
        /// <param name="pattern">The extraction pattern</param>
        [Theory]
        [InlineData(new[] {2.4312, 1.2566, 5.4836}, AutoListPatterns.LinesLengthPattern)]
        [InlineData(new[] {1.6050, 2.5373}, AutoListPatterns.HatchAreaPattern)]
        public void GetDoubles(double[] expectedDoubles, string pattern)
        {
            // Arrange
            var inputString = File.ReadAllText(Filenames[0]);
            // Act
            var result = AutoList.GetDouble(inputString, pattern);

            // Assert
            Assert.Equal(expectedDoubles, result);

            foreach ( var n in result )
                _output.WriteLine(n.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///     Main GetBlocks testing function
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="expected"></param>
        /// <param name="option"></param>
        [Theory]
        [ClassData(typeof(BlocksTestData))]
        public void GetBlocks_Export(string filename, string expected, ExportOptions option)
        {
            // Arrange
            var inputText = File.ReadAllText(filename);
            // Act
            var result = AutoList.GetBlocks(inputText, option);
            _output.WriteLine($"Result: {result}\n Expected: {expected}");
            // Assert
            Assert.Equal(expected, result);
        }

        /// <summary>
        ///     Testing the export csv functionality
        /// </summary>
        [Fact]
        public void ExportToCsv()
        {
            // Arrange
            const string headers = "Block ID,Frontage,Area";
            var dataList1 = new AutoList.Block("Block 1", 100, 101);
            var dataList2 = new AutoList.Block("Block 2", 200, 201);
            var dataList3 = new AutoList.Block("Block 3", 300, 301);
            var blocks = new List<AutoList.Block> {dataList1, dataList2, dataList3};
            const string expectedString =
                "Block ID,Frontage,Area,\nBlock 1,100,101,\nBlock 2,200,201,\nBlock 3,300,301,\n";

            // Act
            var result = AutoList.ExportCsv(headers, blocks);

            // Assert
            Assert.Equal(expectedString, result);
        }

        /// <summary>
        ///     Extracting text objects test
        /// </summary>
        [Fact]
        public void GetText_ExtractTextObjects()
        {
            // Arrange
            var expectedStrings = new[]
            {
                "Text Object 4",
                "Text Object 3", "Text Object 2", "Text Object 1"
            };
            var inputString = ReadFile(Filenames[0]);

            // Act
            var result = AutoList.GetText(inputString, AutoListPatterns.TextPattern);

            // Assert
            Assert.Equal(expectedStrings, result);

            // Print out the strings to the console
            foreach ( var s in result ) _output.WriteLine(s);
        }
    }
}