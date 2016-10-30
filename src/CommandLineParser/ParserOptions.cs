using System;

namespace CommandLineParser
{
    public class ParserOptions
    {
        private string _shortOptionPrefix = "-";
        private string _longOptionPrefix = "--";

        public string ShortOptionPrefix
        {
            get { return _shortOptionPrefix; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new Exception("The short option prefix cannot be null or empty.");
                }
                _shortOptionPrefix = value;
            }
        }

        public string LongOptionPrefix
        {
            get { return _longOptionPrefix; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new Exception("The long option prefix cannot be null or empty.");
                }
                _longOptionPrefix = value;
            }
        }
    }
}
