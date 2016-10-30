
using System;

namespace CommandLineParser
{
    public class Int32Parser : ITypedParser<Int32>
    {
        public Int32 Parse(string value)
        {
            Int32 parsed = Int32.Parse(value);
            return parsed;
        }
    }
}
