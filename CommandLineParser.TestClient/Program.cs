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
            var parser = new Parser();
            parser.Register<Arguments>(arguments =>
                {
                    Console.WriteLine(arguments);
                })
                .Parse(args);
        }
    }

    class Arguments
    {

        [Option('v', "value", IsRequired = true)]
        public string Value { get; set; }

    }
}
