using System;
using CommandLineParser.Exceptions;
using Xunit;

namespace CommandLineParser.Tests
{
    public class ParserFactoryTests
    {
        [Fact]
        public void GetParserInt32ReturnsInt32Parser() 
        {
            var factory = new ParserFactory();
            var int32Parser = factory.GetParser<Int32>();
            Assert.IsType<Int32Parser>(int32Parser);
        }

        [Fact]
        public void GetParserObjectThrowsTypeNotSupportedException()
        {
            var factory = new ParserFactory();
            Assert.Throws<TypeNotSupportedException>(() => factory.GetParser<object>());
        }
    }
}
