
using CommandLineParser.Attributes;
using Xunit;

namespace CommandLineParser.Tests
{
    public class BooleanParserTests
    {
        class ArgumentsWithBoolean
        {
            [Option('a', "active")]
            public bool IsActive { get; set; }
        }

        class ArgumentsWithBooleanAndDefaultValue
        {
            [Option('a', "active", DefaultValue = false)]
            public bool IsActive { get; set; }
        }

        [Fact]
        public void ParseStringAsTrue()
        {
            var args = new[] { "-a", "True" };
            var parser = new Parser();

            parser.Register<ArgumentsWithBoolean>(arguments =>
            {
                Assert.True(arguments.IsActive);
            })
            .Parse(args);
        }

        [Fact]
        public void ParseBooleanOptionWithoutValueAsTrue()
        {
            var args = new[] { "-a" };
            var parser = new Parser();

            parser.Register<ArgumentsWithBoolean>(arguments =>
            {
                Assert.True(arguments.IsActive);
            })
            .Parse(args);
        }

        [Fact]
        public void ParseBooleanOptionWidthDefaultValueAsFalse()
        {
            var args = new[] { "-a" };
            var parser = new Parser();

            parser.Register<ArgumentsWithBooleanAndDefaultValue>(arguments =>
            {
                Assert.False(arguments.IsActive);
            })
            .Parse(args);
        }
    }
}
