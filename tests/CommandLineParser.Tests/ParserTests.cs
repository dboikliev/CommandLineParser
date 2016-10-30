using System;
using Xunit;

namespace CommandLineParser.Tests
{
    public class ParserTests
    {
        [Fact]
        public void ParserThrowsArgumentExceptionWithNullOptions()
        {
            Assert.Throws<ArgumentException>("options", () => new Parser(null));
        }
    }
}
