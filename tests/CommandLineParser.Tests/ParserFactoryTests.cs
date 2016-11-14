using System;
using CommandLineParser.Exceptions;
using CommandLineParser.TypeParsers;
using Xunit;

namespace CommandLineParser.Tests
{
    public class ParserFactoryTests
    {
        [Fact]
        public void GetParserInt32ReturnsInt32Parser() 
        {
            var factory = new ParserFactory();
            var int32Parser = factory.GetParser(typeof(Int32));
            Assert.IsType<Int32ValueParser>(int32Parser);
        }

        [Fact]
        public void GetParserObjectThrowsTypeNotSupportedException()
        {
            var factory = new ParserFactory();
            Assert.Throws<TypeNotSupportedException>(() => factory.GetParser(typeof(object)));
        }
    }
}
