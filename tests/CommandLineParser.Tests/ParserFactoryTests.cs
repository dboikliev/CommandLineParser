using System;
using CommandLineParser;
using Xunit;

namespace Tests
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
    }
}
