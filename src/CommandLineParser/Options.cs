namespace CommandLineParser
{
    public class Options
    {
        public string ShortPrefix { get; }
        public string LongPrefix { get; }
        public string LongHelpOption { get; }
        public string ShortHelpOption { get; }
        public string EndOfList = "--";

        public static readonly Options Default = new Options();

        public Options(char shortPrefix = '-', 
            string longPrefix = "--", 
            string longHelpOption = "help", 
            string shortHelpOption = "h")
        {
            ShortPrefix = shortPrefix.ToString();
            LongPrefix = longPrefix;
            LongHelpOption = longHelpOption;
            ShortHelpOption = shortHelpOption;
        }
    }
}
