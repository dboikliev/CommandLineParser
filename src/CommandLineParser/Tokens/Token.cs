namespace CommandLineParser.Tokens
{
    public class Token
    {
        public TokenType Type { get; }
        public string Symbol { get; }

        public Token(TokenType type, string symbol = "")
        {
            Type = type;
            Symbol = symbol;
        }

        public static Token EndOfList()
        {
            return new Token(TokenType.EndOfList);
        }

        public static Token Command(string symbol)
        {
            return new Token(TokenType.Command, symbol);
        }

        public static Token Flag(string symbol)
        {
            return new Token(TokenType.Flag, symbol);
        }

        public static Token Value(string symbol)
        {
            return new Token(TokenType.Value, symbol);
        }

        public static Token Option(string symbol)
        {
            return new Token(TokenType.Option, symbol);
        }

        public static Token Help()
        {
            return new Token(TokenType.Help);
        }
    }
}
