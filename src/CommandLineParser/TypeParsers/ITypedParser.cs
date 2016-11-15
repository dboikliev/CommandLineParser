using System.Collections.Generic;
using CommandLineParser.ParsedArguments;

namespace CommandLineParser.TypeParsers
{
    public interface ITypedValueParser<TParsed> : ITypedParser
    {
        TParsed Parse(ParsedArgument argument);
    }

    public interface ITypedEnumerableParser<TParsed> : ITypedParser
    {
        IEnumerable<TParsed> Parse(ParsedArgument argument);
    }

    public interface ITypedParser
    {
        object Parse(ParsedArgument argument);
    }
}
