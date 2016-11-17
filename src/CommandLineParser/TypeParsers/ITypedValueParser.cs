using CommandLineParser.ParsedArguments;

namespace CommandLineParser.TypeParsers
{
    internal interface ITypedValueParser<TParsed> : ITypedParser
    {
        new TParsed Parse(ParsedArgument argument);
    }
}