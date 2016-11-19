namespace CommandLineParser.Attributes
{
    public class FlagAttribute : ArgumentAttribute
    {
        public string ShortName { get; }
        public string LongName { get; }

        public FlagAttribute(char shortName, string longName)
        {
            ShortName = shortName.ToString();
            LongName = longName;
        }
    }
}
