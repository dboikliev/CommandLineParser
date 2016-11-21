namespace CommandLineParser.Attributes
{
    public sealed class ValueAttribute : ArgumentAttribute
    {
        public string Name { get; }
        public int Position { get; }
        public bool IsRequired { get; set; } = false;
        public object DefaultValue { get; set; }

        public ValueAttribute(int position, string name)
        {
            Position = position;
            Name = name;
        }
    }
}