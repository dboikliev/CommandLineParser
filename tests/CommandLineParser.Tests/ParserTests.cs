using System;
using System.Collections.Generic;
using System.Linq;
using CommandLineParser.Attributes;
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

            [Value(0, "min")]
            public int Min { get; set; }
        }

        class ArgumentsWithList
        {
            [Option('n', "numbers")]
            public IEnumerable<int> Numbers { get; set; }
        }

        class ArgumentsWithMultipleLists
        {
            [Option('i', "numbers")]
            public IEnumerable<int> Numbers { get; set; }
            [Option('n', "names")]
            public IEnumerable<string> Names { get; set; }
        }

        [Fact]
        public void ParseShortOptionToTestProperty()
        {
            var args = new[] { "-i", "4" };
            var parser = new Parser();
            parser.Register<Arguments>(arguments => Assert.Equal(arguments.Test, int.Parse(args[1])))
                .Parse(args);
        }

        [Fact]
        public void ParseLongOptionToTestProperty()
        {
            var args = new[] { "--integer", "4" };
            var parser = new Parser();
            parser.Register<Arguments>(arguments => Assert.Equal(arguments.Test, int.Parse(args[1])))
                .Parse(args);
        }

        [Fact]
        public void ParseMultipleProperties()
        {
            var args = new[] { "4", "-i", "4", "-t", "10" };
            var parser = new Parser();
            parser.Register<Arguments>(arguments =>
                {
                    Assert.Equal(arguments.Test, 4);
                    Assert.Equal(arguments.AnotherTest, 10);
                    Assert.Equal(arguments.Min, 4);
                })
                .Parse(args);
        }

        [Fact]
        public void ParseSingleListOfValues()
        {
            var args = new[] { "-n", "1", "2", "3", "4" };
            var parser = new Parser();
            parser.Register<ArgumentsWithList>(arguments =>
                {
                    Assert.Equal(arguments.Numbers, Enumerable.Range(1, 4));
                })
                .Parse(args);
        }

        [Fact]
        public void ParseMultipleListsOfValues()
        {
            var args = new[] { "--names", "aa", "bb", "cc", "--numbers", "12", "13", "14" };
            var parser = new Parser();
            parser.Register<ArgumentsWithMultipleLists>(arguments =>
                {
                    Assert.Equal(arguments.Numbers, new[] { 12, 13, 14 });
                    Assert.Equal(arguments.Names, new[] { "aa", "bb", "cc" });
                })
                .Parse(args);
        }
    }
}
