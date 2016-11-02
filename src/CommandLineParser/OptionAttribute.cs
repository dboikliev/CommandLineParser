namespace CommandLineParser
{
    public class OptionAttribute : ArgumentAttribute
    {
        public OptionAttribute(char shortName, string longName) : base(shortName, longName)
        {
        }
    }

    public class ValueAttribute : ArgumentAttribute
    {
    }
}
