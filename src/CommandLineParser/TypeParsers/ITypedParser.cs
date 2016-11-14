using System.Collections.Generic;

namespace CommandLineParser.TypeParsers
{
    public interface ITypedValueParser<TParsed> : ITypedParser
    {
        TParsed Parse(string value);
    }

    public interface ITypedEnumerableParser<TParsed> : ITypedParser
    {
        IEnumerable<TParsed> Parse(IEnumerable<string> values);
    }

    public interface ITypedParser
    {
        object Parse(object value);
    }
}
