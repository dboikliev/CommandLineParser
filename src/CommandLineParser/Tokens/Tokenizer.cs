using System.Collections.Generic;

namespace CommandLineParser.Tokens
{
    public class Tokenizer
    {
        private HashSet<string> _optionNames;
        private HashSet<string> _flagNames;

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
                else if (IsFlag(argument))
                {
                    yield return new Token(TokenType.Flag, i, Normalize(argument));
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

        public bool IsFlag(string argument)
        {
            return (argument.Length == 2 && argument.StartsWith("-")
                 || argument.Length > 2 && argument.StartsWith("--"))
                 && _flagNames.Contains(Normalize(argument));
        }
        
        private static string Normalize(string value)
        {
            return value.Trim('-');
        }
    }
}
