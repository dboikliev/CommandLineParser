using System.Runtime.InteropServices.ComTypes;
using CommandLineParser.Attributes;
using Xunit;

namespace CommandLineParser.Tests
{
    class ArgumentsWithPositionalValue
    {

        [Value(0, "min")]
        public int Min { get; set; }
    }

    class ArgumentsWithMultiplePositionalValues
    {

        [Value(0, "min")]
        public int Min { get; set; }

        [Value(1, "min")]
        public int Max { get; set; }
    }

    public class ValueParserTests
    {
        [Fact]
        public void ParseSinglePositionalValue()
        {
            var args = new[] {"4"};
            var parser = new Parser();
            parser.Register<ArgumentsWithPositionalValue>(arguments =>
            {
                Assert.Equal(4, arguments.Min);
            }).Parse(args);
        }

        [Fact]
        public void ParseMultiplePositionalValues()
        {
            var args = new[] { "4", "5" };
            var parser = new Parser();
            parser.Register<ArgumentsWithMultiplePositionalValues>(arguments =>
            {
                Assert.Equal(4, arguments.Min);
                Assert.Equal(5, arguments.Max);
            }).Parse(args);
        }
    }
}
