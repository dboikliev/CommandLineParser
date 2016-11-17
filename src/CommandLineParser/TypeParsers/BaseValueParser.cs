using System.Collections.Generic;
using System.Linq;
using CommandLineParser.ParsedArguments;

namespace CommandLineParser.TypeParsers
{
    public abstract class BaseValueParser<T> : ITypedValueParser<T>
    {
        public abstract T Parse(ParsedArgument argument);

        object ITypedParser.Parse(ParsedArgument argument)
        {
            return Parse(argument);
        }
    }
}
