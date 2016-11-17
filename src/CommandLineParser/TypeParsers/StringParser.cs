using System.Linq;
using CommandLineParser.ParsedArguments;

namespace CommandLineParser.TypeParsers
{
    public sealed class StringParser : BaseValueParser<string>
    {
        public override string Parse(ParsedArgument argument)
        {
            return argument.Values.FirstOrDefault();
        }
    }
}
