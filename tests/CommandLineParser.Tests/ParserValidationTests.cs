using CommandLineParser.Attributes;
using CommandLineParser.Exceptions;
using Xunit;

namespace CommandLineParser.Tests
{
    public class ParserValidationTests
    {
        class ArgumentsWithMutpleAttributesOnSameProperty
        {
            [Option('n', "number")]
            [Value(0, "number")]
            public int Number { get; set; }
        }

        [Fact]
        public void ThrowExceptionIfMultipleArgumentAttributesAreApplied()
        {
            var parser = new Parser();
            Assert.Throws<MultipleArgumentAttributesNotAllowedException>(
                () => parser.Register<ArgumentsWithMutpleAttributesOnSameProperty>(args => { }));
        }
    }
}
