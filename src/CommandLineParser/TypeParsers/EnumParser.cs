using System;
using System.Linq;
using CommandLineParser.ParsedArguments;

namespace CommandLineParser.TypeParsers
{
    public sealed class EnumParser : BaseValueParser<Enum>
    {
        public override Enum Parse(ParsedArgument argument)
        {
            return (Enum)Enum.Parse(argument.Type, argument.Values.First(), true);
        }
    }
}
