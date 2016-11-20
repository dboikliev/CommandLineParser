using System;
using System.IO;
using CommandLineParser.Attributes;

namespace CommandLineParser.TestClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //args = new[] { "--input", @"D:\in.txt", "--output", @"D:\out.txt" };
            var parser = new Parser();
            parser.Register<Arguments>(arguments =>
            {
                Console.WriteLine(arguments.Input);
                Console.WriteLine(arguments.Output);
            }).Parse(args);
        }
    }

    class Arguments
    {
        [Option('i', "input")]
        public FileInfo Input { get; set; }

        [Option('o', "output")]
        public FileInfo Output { get; set; }
    }
}
