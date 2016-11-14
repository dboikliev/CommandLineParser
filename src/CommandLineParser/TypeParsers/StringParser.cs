using System;

namespace CommandLineParser.TypeParsers
{
    public class StringParser : BaseValueParser<string>
    {
        public override string Parse(string value)
        {
            return value;
        }
    }
}
