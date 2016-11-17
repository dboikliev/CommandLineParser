using System.IO;
using System.Linq;
using CommandLineParser.ParsedArguments;

namespace CommandLineParser.TypeParsers
{
    public sealed class FileInfoParser : BaseValueParser<FileInfo>
    {
        public override FileInfo Parse(ParsedArgument argument)
        {
           return new FileInfo(argument.Values.FirstOrDefault());
        }
    }
}
