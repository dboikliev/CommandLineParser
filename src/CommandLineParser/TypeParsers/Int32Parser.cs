using System.Linq;
using CommandLineParser.ParsedArguments;

namespace CommandLineParser.TypeParsers
{
    public sealed class Int32Parser : BaseValueParser<int>
    {
        public override int Parse(ParsedArgument argument)
        {
            return int.Parse(argument.Values.First());
        }
    }
}
