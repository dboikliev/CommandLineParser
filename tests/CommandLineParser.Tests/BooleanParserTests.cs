
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

        [Fact]
        public void ParserShouldParserStringAsBoolean()
        {
            var args = new[] { "-a", "True" };
            var parser = new Parser();

            parser.Register<ArgumentsWithBoolean>(arguments =>
            {
                Assert.True(arguments.IsActive);
            })
            .Parse(args);
        }
    }
}
