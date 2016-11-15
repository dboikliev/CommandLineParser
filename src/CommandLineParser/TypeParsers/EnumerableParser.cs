using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommandLineParser.ParsedArguments;

namespace CommandLineParser.TypeParsers
{
    public sealed class EnumerableValueParser<T> : ITypedEnumerableParser<T>
    {
        private readonly ParserFactory _parserFactory = new ParserFactory();

        public IEnumerable<T> Parse(ParsedArgument argument)
        {
            foreach (var value in argument.Values)
            {
                yield return (T)_parserFactory[typeof(T)].Parse(new ParsedArgument
                {
                    Type = argument.Type.GenericTypeArguments.First(),
                    Values = new [] { value }
                });
            }
        }

        object ITypedParser.Parse(ParsedArgument argument)
        {
            return Parse(argument);
        }
    }
}
