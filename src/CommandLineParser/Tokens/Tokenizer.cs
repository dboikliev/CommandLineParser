using System.Collections.Generic;
using System.Linq;
using CommandLineParser.Extensions;

namespace CommandLineParser.Tokens
{
    public class Tokenizer
    {
        private readonly Options _options;
        private readonly HashSet<string> _optionNames;
        private readonly HashSet<string> _flagNames;
        private readonly HashSet<string> _commandNames;

        public Tokenizer(Options options,
            HashSet<string> optionNames,
            HashSet<string> flagNames,
            HashSet<string> commandNames)
        {
            _options = options;
            _optionNames = optionNames;
            _flagNames = flagNames;
            _commandNames = commandNames;
        }

        public IEnumerable<Token> Tokenize(string[] args)
        {
            //yield return new Token(TokenType.Command, -1);
            foreach (var argument in args)
            {
                if (IsEndOfList(argument))
                {
                    yield return new Token(TokenType.EndOfList);
                }
                else if (IsOption(argument))
                {
                    yield return new Token(TokenType.Option, Normalize(argument));
                }
                else if (IsSingleFlag(argument))
                {
                    yield return new Token(TokenType.Flag, Normalize(argument));
                }
                else if (IsMultiFlag(argument))
                {
                    var normalized = Normalize(argument);
                    if (normalized.Length > 1)
                    {
                        foreach (char flag in normalized)
                        {
                            yield return new Token(TokenType.Flag, flag.ToString());
                        }
                    }
                }
                else if (IsCommand(argument))
                {
                    yield return new Token(TokenType.Command, argument);
                }
                else if (IsHelp(argument))
                {
                    yield return new Token(TokenType.Help, argument);
                }
                else
                {
                    yield return new Token(TokenType.Value, Normalize(argument));
                }
            }
        }

        private bool IsHelp(string argument) =>
            argument == _options.ShortPrefix + _options.ShortHelpOption ||
            argument == _options.LongPrefix + _options.LongHelpOption;

        private bool IsEndOfList(string argument) =>
            argument.Length == 2 &&
            argument.StartsWith(_options.EndOfList);

        private bool IsOption(string argument) =>
            (argument.Length == 2 && argument.StartsWith(_options.ShortPrefix) ||
             argument.Length > _options.LongPrefix.Length && argument.StartsWith(_options.LongPrefix)) &&
            _optionNames.Contains(Normalize(argument));

        private bool IsSingleFlag(string argument) =>
            (argument.Length == 2 && argument.StartsWith(_options.ShortPrefix) ||
             argument.Length == _options.LongPrefix.Length + 1 && argument.StartsWith(_options.LongPrefix)) &&
            _flagNames.Contains(Normalize(argument));

        private bool IsMultiFlag(string argument)
        {
            return argument.Length > 2
                 && argument.StartsWith(_options.ShortPrefix)
                 && argument.Skip(1).All(f => _flagNames.Contains(f.ToString()));
        }

        private bool IsCommand(string argument) => _commandNames.Contains(argument);


        private string Normalize(string value)
        {
            return value.TrimStart(_options.ShortPrefix);
        }
    }
}
