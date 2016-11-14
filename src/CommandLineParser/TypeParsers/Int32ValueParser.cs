using System;

namespace CommandLineParser.TypeParsers
{
    public sealed class Int32ValueParser : BaseValueParser<int>
    {
        public override int Parse(string value)
        {
            return int.Parse(value);
        }
    }
}
