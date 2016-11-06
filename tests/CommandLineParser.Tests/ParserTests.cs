using System;
using Xunit;

namespace CommandLineParser.Tests
{
    public class ParserTests
    {
        class Arguments
        {
            [Option('i', "integer")]
            public int Test { get; set; }

            [Option('t', "test")]
            public int AnotherTest { get; set; }
        }

        [Fact]
        public void ParserShouldParseShortOptionToTestProperty()
        {
            var args = new[] {"-i", "4"};
            var parser = new Parser();
            parser.Register<Arguments>()
                .On<Arguments>(arguments => Assert.Equal(arguments.Test, int.Parse(args[1])))
                .Parse(args);
        }

        [Fact]
        public void ParserShouldParseLongOptionToTestProperty()
        {
            var args = new[] { "--integer", "4" };
            var parser = new Parser();
            parser.Register<Arguments>()
                .On<Arguments>(arguments => Assert.Equal(arguments.Test, int.Parse(args[1])))
                .Parse(args);
        }

        [Fact]
        public void ParserShouldParserMultipleProperties()
        {
            var args = new[] { "-i", "4", "-t", "10" };
            var parser = new Parser();
            parser.Register<Arguments>()
                .On<Arguments>(arguments =>
                {
                    Assert.Equal(arguments.Test, int.Parse(args[1]));
                    Assert.Equal(arguments.AnotherTest, int.Parse(args[3]));
                })
                .Parse(args);
        }
    }
}
