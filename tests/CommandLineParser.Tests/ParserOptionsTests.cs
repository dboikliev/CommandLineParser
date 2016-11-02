using System;
using System.Runtime.InteropServices;
using Xunit;

namespace CommandLineParser.Tests
{
    public class ParserOptionsTests
    {
        class Arguments
        {
            [Option('v', "value", IsRequired = true)]
            public string Value { get; set; }
        }


        [Fact]
        public void ParserMapsOptionsToProperties()
        {
            //Arrange
            var args = new[] {"-v", "some_text"};

            //Act
            var parser = new Parser();
            parser.Register<Arguments>();

            var value = string.Empty;
            parser.Register<Arguments>();
            parser.On<Arguments>(a => value = a.Value);
            parser.Parse(args);

            //Assert
            Assert.Equal(value, args[1]);
        }
    }
}
