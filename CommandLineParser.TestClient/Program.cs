using System;
using CommandLineParser.Attributes;

namespace CommandLineParser.TestClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            args = new[] { "-v", "a" };
            var parser = new Parser();
            parser.Register<Arguments>(arguments =>
                {
                    Console.WriteLine(arguments.Value);
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
