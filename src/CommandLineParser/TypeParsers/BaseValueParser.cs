using System.Collections.Generic;
using System.Linq;
using CommandLineParser.ParsedArguments;

namespace CommandLineParser.TypeParsers
{
    public abstract class BaseValueParser<T> : ITypedValueParser<T>
    {
        //public abstract T Parse(string value);
        //object ITypedParser.Parse(object value)
        //{
        //    var enumerable = value as IEnumerable<string>;
        //    if (enumerable != null)
        //    {
        //        return Parse(enumerable.FirstOrDefault());
        //    }
        //    return Parse((string) value);
        //}

        public abstract T Parse(ParsedArgument argument);

        object ITypedParser.Parse(ParsedArgument argument)
        {
            return Parse(argument);
        }
    }
}
