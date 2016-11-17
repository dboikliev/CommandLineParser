using System.Linq;
using CommandLineParser.ParsedArguments;

namespace CommandLineParser.TypeParsers
{
    public sealed class BooleanParser : BaseValueParser<bool>
    {
        public override bool Parse(ParsedArgument argument)
        {
            return bool.Parse(argument.Values.FirstOrDefault());
        }
    }
}
