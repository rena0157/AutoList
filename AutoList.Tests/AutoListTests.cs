using System;
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
        public static string ReadFile(string fileName) { return File.ReadAllText(fileName); }
        public static string Filename = @".\TestFiles\GenericListText.txt";
        private readonly ITestOutputHelper _output;

        public AutoListTests(ITestOutputHelper output) { _output = output; }

        [Fact]
        public void GetText_ExtractTextObjects()
        {
            // Arrange
            var expectedStrings = new[] {"Text Object 3", "Text Object 2", "Text Object 1"};
            var inputString = ReadFile(Filename);

            // Act
            var result = AutoList.GetText(inputString, AutoListPatterns.TextPattern);

            // Assert
            Assert.Equal(3, result.Count);
            foreach ( var res in result )
            {
                _output.WriteLine(res);
                Assert.Equal(expectedStrings, result);
            }
        }


    }
}
