using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Xunit;
using AutoList;
using Xunit.Abstractions;

namespace AutoList.Tests
{
    public class AutoListTests
    {
        private static string ReadFile(string fileName) { return File.ReadAllText(fileName); }
        private static string Filename = @".\TestFiles\GenericListText.txt";
        private readonly ITestOutputHelper _output;
        private readonly string[] _inputStrings;

        public AutoListTests(ITestOutputHelper output)
        {
            _output = output;
            _inputStrings = new[] {File.ReadAllText(Filename)};
        }

        [Fact]
        public void GetText_ExtractTextObjects()
        {
            // Arrange
            var expectedStrings = new[] {"Text Object 4", 
                "Text Object 3", "Text Object 2", "Text Object 1"};
            var inputString = ReadFile(Filename);

            // Act
            var result = AutoList.GetText(inputString, AutoListPatterns.TextPattern);

            // Assert
            Assert.Equal(expectedStrings, result);

            // Print out the strings to the console
            foreach ( var s in result )
            {
                _output.WriteLine(s);
            }
        }

        [Theory]
        [InlineData(new[]{ 2.4312, 1.2566, 5.4836 }, AutoListPatterns.LinesLengthPattern)]
        [InlineData(new[]{1.6050, 2.5373}, AutoListPatterns.HatchAreaPattern)]
        public void GetDoubles(double[] expectedDoubles, string pattern)
        {
            // Arrange
            var inputString = File.ReadAllText(Filename);
            // Act
            var result = AutoList.GetDouble(inputString, pattern);

            // Assert
            Assert.Equal(expectedDoubles, result);

            foreach( var n in result)
                _output.WriteLine(n.ToString(CultureInfo.InvariantCulture));
        }

    }
}
