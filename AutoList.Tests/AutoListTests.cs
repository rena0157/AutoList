// AutoList.Tests
// AutoListTests.cs
// 
// ============================================================
// 
// Created: 2018-10-10
// Last Updated: 2018-10-10-8:16 PM
// By: Adam Renaud
// 
// ============================================================

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace AutoList.Tests
{
    public class AutoListTests
    {
        public AutoListTests(ITestOutputHelper output)
        {
            _output = output;
            _inputStrings = new[] {File.ReadAllText(Filenames[0])};
        }

        private static string ReadFile(string fileName) { return File.ReadAllText(fileName); }

        private static readonly string[] Filenames =
        {
            @".\TestFiles\GenericListText.txt",
            @".\TestFiles\BlocksTest.txt"
        };

        private readonly ITestOutputHelper _output;
        private readonly string[] _inputStrings;

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

        [Fact]
        public void ExportToCsv()
        {
            // Arrange
            const string headers = "h1,h2,h3";
            var dataList1 = new List<string> {"d1", "d4", "d7"};
            var dataList2 = new List<string> {"d2", "d5", "d8"};
            var dataList3 = new List<string> {"d3", "d6", "d9"};
            const string expectedString = "h1,h2,h3,\nd1,d2,d3,\nd4,d5,d6,\nd7,d8,d9,\n";

            // Act
            var result = AutoList.ExportCsv(headers, dataList1, dataList2, dataList3);

            // Assert
            Assert.Equal(expectedString, result);
        }

        [Fact]
        public void GetBlocks_Export()
        {
            // Arrange
            var inputText = File.ReadAllText(Filenames[1]);

            // Act
            var result = AutoList.GetBlocks(inputText);
            var expected = "Block ID,Frontage,Area,\nBlock 1,100,2900,\nBlock 1,0,2900,\n";
            // Assert
            Assert.Equal(expected, result);
        }

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