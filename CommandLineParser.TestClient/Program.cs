using System;
using CommandLineParser.Attributes;

namespace CommandLineParser.TestClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            args = new[] { "test", "3", "--input", "5", "-n", "3" };
            var parser = new Parser();
            parser
                .Register<Arguments>(arguments =>
                {
                    Console.WriteLine(arguments.Input);
                    Console.WriteLine(arguments.Value);
                })
                .Register<Arguments2>(a => {})
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
    
    [Command("pesho", Description = "Command used for testing my command line parser.")]
    internal class Arguments2
    {
        [Option('i', "input", IsRequired = true, DefaultValue = 10, Description = "Input option")]
        public int Input { get; set; }
        [Option('n', "lines", IsRequired = true, Description = "Lines option")]
        public int Lines { get; set; }
        [Value(0, "min", IsRequired = true, Description = "Min value")]
        public int Value { get; set; }
    }
}
