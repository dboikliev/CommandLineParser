using CommandLineParser.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace CommandLineParser.TestClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            args = new[] { "test"};
            var parser = new Parser();
            parser.Register<Arguments>(arguments =>
            {
                Console.WriteLine(arguments.Input);
            }).Register<Arguments2>(arguments =>
            {
                Console.WriteLine(arguments.Lines);
            })
            .Parse(args);
        }
    }

    [Command("test")]

    class Arguments
    {
        [Option('i', "input", IsRequired = true, DefaultValue = 10)]
        public int Input { get; set; }
    }

    [Command("bla")]

    class Arguments2
    {
        [Option('n', "lines")]
        public int Lines { get; set; }
    }
}
