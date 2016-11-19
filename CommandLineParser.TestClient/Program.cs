using CommandLineParser.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using static System.Console;

namespace CommandLineParser.TestClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            args = new[] { "-l", "-d" };
            var parser = new Parser();
            parser.Register<Arguments>(arguments =>
            {
                WriteLine($"Log: {arguments.Log}, Delete: {arguments.Delete}");
            }).Parse(args);
        }
    }

    class Arguments
    {
        [Option('d', "delete")]
        public bool Delete { get; set; }
        [Option('l', "log")]
        public bool Log{ get; set; }
    }
}
