using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandLineParser.TestClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(string.Join(",", args));
            Parser.Parse<Arguments>(args);
            ITypedParser<int> parser = new ParserFactory().GetParser<int>();
        }
    }

    class Arguments
    {
        [Option("o", "output", IsRequired = true)]
        public string OutputDir { get; set; }

        [Option("i", "input")]
        public string InputDir { get; set; }

        [Value(IsRequired = true)]
        public int Max { get; set; }
    }
}
