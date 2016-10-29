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
        }
    }

    class Arguments
    {
        [Option("-o", "--output")]
        public string OutputDir { get; set; }
    }
}
