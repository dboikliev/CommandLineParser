using System.Collections.Generic;

namespace CommandLineParser.TypeParsers
{
    public sealed class EnumerableValueParser<T> : BaseEnumerableParser<T>
    {
        private readonly ParserFactory _parserFactory = new ParserFactory();

        public override IEnumerable<T> Parse(IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                yield return (T)_parserFactory[typeof(T)].Parse(value);
            }
        }
    }
}
