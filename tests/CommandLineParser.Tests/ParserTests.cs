﻿using System;
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
            parser.Register<Arguments>(arguments => Assert.Equal(int.Parse(args[1]), arguments.Test))
                .Parse(args);
        }

        [Fact]
        public void ParseLongOptionToTestProperty()
        {
            var args = new[] { "--integer", "4" };
            var parser = new Parser();
            parser.Register<Arguments>(arguments => Assert.Equal(int.Parse(args[1]), arguments.Test))
                .Parse(args);
        }

        [Fact]
        public void ParseMultipleProperties()
        {
            var args = new[] { "-i", "4", "-t", "10" };
            var parser = new Parser();
            parser.Register<Arguments>(arguments =>
                {
                    Assert.Equal(4, arguments.Test);
                    Assert.Equal(10, arguments.AnotherTest);
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
                    Assert.Equal(Enumerable.Range(1, 4), arguments.Numbers);
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
                    Assert.Equal(new[] { 12, 13, 14 }, arguments.Numbers);
                    Assert.Equal(new[] { "aa", "bb", "cc" }, arguments.Names);
                })
                .Parse(args);
        }
    }
}
