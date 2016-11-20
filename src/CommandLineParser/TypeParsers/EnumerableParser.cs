using System.Collections.Generic;
using System.Linq;
using CommandLineParser.ParsedArguments;

namespace CommandLineParser.TypeParsers
{
    public sealed class EnumerableValueParser<T> : ITypedEnumerableParser<T>
    {
        private readonly ParserFactory _parserFactory = new ParserFactory();
        private readonly ITypedParser _parser;
        public EnumerableValueParser()
        {
            _parser = _parserFactory[typeof(T)];
        }

        public IEnumerable<T> Parse(ParsedArgument argument)
        {
            return argument.Values.Select(value => (T)_parser.Parse(new ParsedArgument
            {
                Type = argument.Type.GenericTypeArguments.First(),
                Values = new [] { value }
            }));
        }

        object ITypedParser.Parse(ParsedArgument argument)
        {
            return Parse(argument);
        }
    }
}
