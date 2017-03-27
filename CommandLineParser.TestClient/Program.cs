using CommandLineParser.Attributes;
using System;

namespace CommandLineParser.TestClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            args = new[] { "test", "--help" };
            var parser = new Parser();
            parser.Register<Arguments>(arguments =>
            {
                Console.WriteLine(arguments.Input);
                Console.WriteLine(arguments.Value);
            })
            .Parse(args);
        }
    }

    [Command("test", Description = "Command used for testing my command line parser.")]
    internal class Arguments
    {
        [Option('i', "input", IsRequired = true, DefaultValue = 10, Description = "Input option")]
        public int Input { get; set; }
        [Option('n', "lines", IsRequired = true, Description = "Lines option")]
        public int Lines { get; set; }
        [Value(0, "min", IsRequired = true, Description = "Min value")]
        public int Value { get; set; }
    }
}
