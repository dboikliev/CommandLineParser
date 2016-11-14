using System;

namespace CommandLineParser.TypeParsers
{
    public sealed class Int32Parser : BaseValueParser<int>
    {
        public override int Parse(string value)
        {
            return int.Parse(value);
        }
    }
}
