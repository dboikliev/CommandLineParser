namespace CommandLineParser.Attributes
{
    public sealed class OptionAttribute : ArgumentAttribute
    {
        public string ShortName { get; }
        public string LongName { get; }
        public object DefaultValue { get; set; }

        public OptionAttribute(char shortName, string longName)
        {
            ShortName = shortName.ToString();
            LongName = longName;
        }
    }
}
