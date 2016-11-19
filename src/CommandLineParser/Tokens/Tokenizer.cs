using System.Collections.Generic;
using System.Linq;

namespace CommandLineParser.Tokens
{
    public class Tokenizer
    {
        private readonly HashSet<string> _optionNames;
        private readonly HashSet<string> _flagNames;

        public Tokenizer(HashSet<string> _optionNames, HashSet<string> _flagNames)
        {
            this._optionNames = _optionNames;
            this._flagNames = _flagNames;
        }

        public IEnumerable<Token> Tokenize(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                var argument = args[i];
                if (IsEndOfList(argument))
                {
                    yield return new Token(TokenType.EndOfList, i);
                }
                else if (IsOption(argument))
                {
                    yield return new Token(TokenType.Option, i, Normalize(argument));
                }
                else if (IsSingleFlag(argument))
                {
                    yield return new Token(TokenType.Flag, i, Normalize(argument));
                }
                else if (IsMultiFlag(argument))
                {
                    var normalized = Normalize(args[i]);
                    if (normalized.Length > 1)
                    {
                        foreach (char flag in normalized)
                        {
                            yield return new Token(TokenType.Flag, i, flag.ToString());
                        }
                    }
                }
                else 
                {
                    yield return new Token(TokenType.Value, i, Normalize(argument));
                }
            }
        }

        private bool IsEndOfList(string argument)
        {
            return argument.Length == 2 
                && argument.StartsWith("--");
        }
        public bool IsOption(string argument)
        {
            return (argument.Length == 2 && argument.StartsWith("-")
                   || argument.Length > 2 && argument.StartsWith("--"))
                   && _optionNames.Contains(Normalize(argument));
        }

        public bool IsSingleFlag(string argument)
        {
            return (argument.Length == 2 && argument.StartsWith("-")
                 || argument.Length == 3 && argument.StartsWith("--"))
                 && _flagNames.Contains(Normalize(argument));
        }

        public bool IsMultiFlag(string argument)
        {
            return argument.Length > 2 
                 && argument.StartsWith("-")
                 && argument.Substring(1).All(f => _flagNames.Contains(f.ToString()));
        }
        
        private static string Normalize(string value)
        {
            return value.Trim('-');
        }
    }
}
