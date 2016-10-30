namespace CommandLineParser
{
    public class OptionAttribute : ArgumentAttribute
    {
        public OptionAttribute(string shortName, string longName) : base(shortName, longName)
        {
        }
    }

    public class ValueAttribute : ArgumentAttribute
    {
        public ValueAttribute() : base(null, null)
        {
        }
    }
}
