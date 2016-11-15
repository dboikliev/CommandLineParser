using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CommandLineParser.Tests
{
    public class EnumParserTests
    {
        private class ArgumentsWithEnum
        {
            public enum Color { Red, Green, Blue }

            [Option('c', "color")]
            public Color Rgb { get; set; }
        }

        private class ArgumentsWithIEnumerableOfEnum
        {
            public enum Color { Red, Green, Blue }

            [Option('c', "color")]
            public IEnumerable<Color> RgbColors { get; set; }
        }

        [Fact]
        public void ParserShouldParseTextAsRedEnum()
        {
            var args = new[] { "-c", "RED" };
            var parser = new Parser();
            parser.Register<ArgumentsWithEnum>(arguments =>
            {
                Assert.Equal(arguments.Rgb, ArgumentsWithEnum.Color.Red);
            })
            .Parse(args);
        }

        [Fact]
        public void ParserShouldParseTextAsGreenEnum()
        {
            var args = new[] { "-c", "GREEN" };
            var parser = new Parser();
            parser.Register<ArgumentsWithEnum>(arguments =>
            {
                Assert.Equal(arguments.Rgb, ArgumentsWithEnum.Color.Green);
            })
            .Parse(args);
        }

        [Fact]
        public void ParserShouldParseTextAsBlueEnum()
        {
            var args = new[] { "-c", "BLUE" };
            var parser = new Parser();
            parser.Register<ArgumentsWithEnum>(arguments =>
            {
                Assert.Equal(arguments.Rgb, ArgumentsWithEnum.Color.Blue);
            })
            .Parse(args);
        }

        [Fact]
        public void ParserShouldParseTextAsIEnumerableOfColorEnum()
        {
            var args = new[] { "-c", "BLUE", "RED", "GREEN" };
            var parser = new Parser();
            parser.Register<ArgumentsWithIEnumerableOfEnum>(arguments =>
            {
                Assert.Equal(arguments.RgbColors, new []
                {
                    ArgumentsWithIEnumerableOfEnum.Color.Blue,
                    ArgumentsWithIEnumerableOfEnum.Color.Red,
                    ArgumentsWithIEnumerableOfEnum.Color.Green, 
                });
            })
            .Parse(args);
        }
    }
}
