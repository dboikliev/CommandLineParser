using CommandLineParser.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CommandLineParser.TestClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            args = new[] { "--files", @"D:\Fake_CV.txt" };
            var parser = new Parser();
            parser.Register<Arguments>(arguments =>
            {
                Console.WriteLine(string.Join(", ", arguments.Files.Select(f => f.FullName)));
            }).Parse(args);
        }
    }

    class Arguments
    {
        [Option('f', "files")]
        public IEnumerable<FileInfo> Files { get; set; }

    }
}
