using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLineParser.Tokens
{
    public class Tokenizer
    {
        private readonly HashSet<string> _optionNames;
        private readonly HashSet<string> _flagNames;
        private readonly HashSet<string> _commandNames;

        public Tokenizer(HashSet<string> optionNames, 
            HashSet<string> flagNames, 
            HashSet<string> commandNames)
        {
            _optionNames = optionNames;
            _flagNames = flagNames;
            _commandNames = commandNames;
        }

        public IEnumerable<Token> Tokenize(string[] args)
        {
            //yield return new Token(TokenType.Command, -1);
            for (int i = 0; i < args.Length; i++)
            {
                var argument = args[i];
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
                    var normalized = Normalize(args[i]);
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

        private bool IsHelp(string argument)
        {
            return argument == "--help";
        }

        private bool IsEndOfList(string argument)
        {
            return argument.Length == 2 
                && argument.StartsWith("--");
        }
        private bool IsOption(string argument)
        {
            return (argument.Length == 2 && argument.StartsWith("-")
                   || argument.Length > 2 && argument.StartsWith("--"))
                   && _optionNames.Contains(Normalize(argument));
        }

        private bool IsSingleFlag(string argument)
        {
            return (argument.Length == 2 && argument.StartsWith("-")
                 || argument.Length == 3 && argument.StartsWith("--"))
                 && _flagNames.Contains(Normalize(argument));
        }

        private bool IsMultiFlag(string argument)
        {
            return argument.Length > 2 
                 && argument.StartsWith("-")
                 && argument.Substring(1).All(f => _flagNames.Contains(f.ToString()));
        }

        private bool IsCommand(string argument)
        {
            return _commandNames.Contains(argument);
        }

        
        private static string Normalize(string value)
        {
            return value.Trim('-');
        }
    }
}
