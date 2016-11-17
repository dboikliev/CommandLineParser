using CommandLineParser.ParsedArguments;

namespace CommandLineParser.TypeParsers
{
    internal interface ITypedParser
    {
        object Parse(ParsedArgument argument);
    }
}
