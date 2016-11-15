using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandLineParser.ParsedArguments;

namespace CommandLineParser.TypeParsers
{
    public class EnumParser : BaseValueParser<Enum>
    {
        public override Enum Parse(ParsedArgument argument)
        {
            return (Enum)Enum.Parse(argument.Type, argument.Values.FirstOrDefault(), true);
        }
    }
}
