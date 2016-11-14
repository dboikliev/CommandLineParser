using System.Collections.Generic;

namespace CommandLineParser.TypeParsers
{
    public sealed class EnumerableValueParser<T> : ITypedEnumerableParser<T>
    {
        private readonly ParserFactory _parserFactory = new ParserFactory();

        public IEnumerable<T> Parse(IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                yield return (T)_parserFactory[typeof(T)].Parse(value);
            }
        }
        object ITypedParser.Parse(object value)
        {
            return Parse((IEnumerable<string>)value);
        }
    }
}
