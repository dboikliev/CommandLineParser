using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandLineParser.Attributes;
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
        public void ParseTextAsRedEnum()
        {
            var args = new[] { "-c", "RED" };
            var parser = new Parser();
            parser.Register<ArgumentsWithEnum>(arguments =>
            {
                Assert.Equal(ArgumentsWithEnum.Color.Red, arguments.Rgb);
            })
            .Parse(args);
        }

        [Fact]
        public void ParseTextAsGreenEnum()
        {
            var args = new[] { "-c", "GREEN" };
            var parser = new Parser();
            parser.Register<ArgumentsWithEnum>(arguments =>
            {
                Assert.Equal(ArgumentsWithEnum.Color.Green, arguments.Rgb);
            })
            .Parse(args);
        }

        [Fact]
        public void ParseTextAsBlueEnum()
        {
            var args = new[] { "-c", "BLUE" };
            var parser = new Parser();
            parser.Register<ArgumentsWithEnum>(arguments =>
            {
                Assert.Equal(ArgumentsWithEnum.Color.Blue, arguments.Rgb);
            })
            .Parse(args);
        }

        [Fact]
        public void ParseTextAsIEnumerableOfColorEnum()
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
