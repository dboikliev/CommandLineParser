
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

        class ArgumentsWithMultipleBooleanProperties
        {
            [Flag('d', "delete")]
            public bool Delete { get; set; }
            [Flag('l', "log")]
            public bool Log { get; set; }
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
        public void ParseMultipleBooleanArguments()
        {
            var args = new[] { "-l", "-d" };
            var parser = new Parser();

            parser.Register<ArgumentsWithMultipleBooleanProperties>(arguments =>
            {
                Assert.True(arguments.Log);
                Assert.True(arguments.Delete);
            })
                .Parse(args);
        }
    }
}
