using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CommandLineParser.Attributes;

namespace CommandLineParser.TestClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            args = new[] { "head", "sda"};
            var sw  = new Stopwatch();
            sw.Start();
            var parser = new Parser();
            parser.Register<IOptions>(arguments =>
            {
                Console.WriteLine(arguments.FileName);
            }).Parse(args);
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
        }
    }

    [Command("head")]

    class IOptions
    {
        [Option('n', "lines")]
        public int Lines { get; set; }

        [Option('c', "bytes")]
        public int Bytes { get; set; }

        [Option('q', "quiet")]
        public bool Quiet { get; set; }

        [Value(0, "dsa")]
        public string FileName { get; set; }
    }

}
