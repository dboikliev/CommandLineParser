using System;
using System.Collections.Generic;
using CommandLineParser.Exceptions;
using CommandLineParser.TypeParsers;
using Xunit;

namespace CommandLineParser.Tests
{
    public class ParserFactoryTests
    {
        [Fact]
        public void GetInt32Parser() 
        {
            var factory = new ParserFactory();
            var int32Parser = factory.GetParser(typeof(int));
            Assert.IsType<Int32Parser>(int32Parser);
        }

        [Fact]
        public void GetStringParser()
        {
            var factory = new ParserFactory();
            var enumberableInt32Parser = factory.GetParser(typeof(string));
            Assert.IsType<StringParser>(enumberableInt32Parser);
        }

        [Fact]
        public void GetIEnumberableInt32Parser()
        {
            var factory = new ParserFactory();
            var enumberableInt32Parser = factory.GetParser(typeof(IEnumerable<int>));
            Assert.IsType<EnumerableValueParser<int>>(enumberableInt32Parser);
        }

        [Fact]
        public void GetIEnumberableStringParser()
        {
            var factory = new ParserFactory();
            var enumberableInt32Parser = factory.GetParser(typeof(IEnumerable<string>));
            Assert.IsType<EnumerableValueParser<string>>(enumberableInt32Parser);
        }

        [Fact]
        public void GetObjectParserThrowsTypeNotSupportedException()
        {
            var factory = new ParserFactory();
            Assert.Throws<TypeNotSupportedException>(() => factory.GetParser(typeof(object)));
        }
    }
}
