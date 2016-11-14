using System.Collections.Generic;
using System.Linq;

namespace CommandLineParser.TypeParsers
{
    public abstract class BaseValueParser<T> : ITypedValueParser<T>
    {
        public abstract T Parse(string value);
        object ITypedParser.Parse(object value)
        {
            var enumerable = value as IEnumerable<string>;
            if (enumerable != null)
            {
                return Parse(enumerable.FirstOrDefault());
            }
            return Parse((string) value);
        }
    }
}
