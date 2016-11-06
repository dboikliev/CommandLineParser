using System;
using System.Collections.Generic;

namespace CommandLineParser.Tokens
{
    public class Tokenizer
    {
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
                    yield return new Token(TokenType.Option, i, argument);
                }
                else if (IsValue(argument))
                {
                    yield return new Token(TokenType.Value, i, argument);
                }
            }
        }

        private bool IsEndOfList(string argument)
        {
            return argument.Length == 2 && argument.StartsWith("--");
        }
        
        public bool IsOption(string argument)
        {
            return argument.Length == 2 && argument.StartsWith("-")
                   || argument.Length > 2 && argument.StartsWith("--");
        }

        public bool IsValue(string argument)
        {
            return !IsOption(argument);
        }
    }
}
