using System.Collections.Generic;

namespace CommandLineParser.TypeParsers
{
    public abstract class BaseEnumerableParser<T> : ITypedEnumerableParser<T>
    {
        public abstract IEnumerable<T> Parse(IEnumerable<string> values);

        object ITypedParser.Parse(object value)
        {
            return Parse((IEnumerable<string>) value);
        }
    }
}
