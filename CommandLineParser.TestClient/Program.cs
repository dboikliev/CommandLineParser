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
            var parser = new Parser();
            parser.Register<Arguments>()
                .On<Arguments>(a => Console.WriteLine(a.Value + " has been parsed."));
        }
    }

    class Arguments
    {
        [Option('v', "value", IsRequired = true)]
        public string Value { get; set; }

    }
}
