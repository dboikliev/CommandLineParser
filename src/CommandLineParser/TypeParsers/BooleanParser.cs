using System.Linq;
using CommandLineParser.ParsedArguments;

namespace CommandLineParser.TypeParsers
{
    public sealed class BooleanParser : BaseValueParser<bool>
    {
        public override bool Parse(ParsedArgument argument)
        {
            var value = argument.Values.FirstOrDefault();
            return bool.Parse(value);
        }
    }
}
