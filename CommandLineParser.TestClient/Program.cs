using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using CommandLineParser.Attributes;
using static System.Console;

namespace CommandLineParser.TestClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            args = new[] { "1", "-ld", "copy", "2,", "--source", @"D:\test.txt", "--destination", @"D:\test2.txt", "delete", "--path", @"D:\emails.txt" };
            var parser = new Parser();
            parser.Register<Arguments>(arguments =>
            {
                WriteLine(arguments.Min);
                WriteLine($"Log: {arguments.Log}, Delete: {arguments.Delete}");
            })
            .Register<CopyCommand>(commandArgs =>
            {
                WriteLine(commandArgs.Source.FullName);
                WriteLine(commandArgs.Destination.FullName);
            })
            .Register<DeleteCommand>(commandArgs =>
            {
                WriteLine(commandArgs.Path);
            })
            .Parse(args);
        }
    }

    class Arguments
    {
        [Flag('d', "delete")]
        public bool Delete { get; set; }
        [Flag('l', "log")]
        public bool Log { get; set; }

        [Value(0, "min")]
        public int Min { get; set; }
    }

    [Command("copy")]
    class CopyCommand
    {
        [Option('s', "source")]
        public FileInfo Source { get; set; }
        [Option('d', "destination")]
        public FileInfo Destination { get; set; }
    }


    [Command("delete")]
    class DeleteCommand
    {
        [Option('p', "path")]
        public string Path { get; set; }
    }
}
