namespace CommandLineParser.Tokens
{
    public class Token
    {
        public TokenType Type { get; }
        public string Value { get; }
        public int Position { get; }

        public Token(TokenType type, int position, string value = "")
        {
            Type = type;
            Position = position;
            Value = value;
        }
    }
}
