using CommandLineParser.Attributes;
using static System.Console;

namespace CommandLineParser.TestClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            args = new[] { "-ld" };
            var parser = new Parser();
            parser.Register<Arguments>(arguments =>
            {
                WriteLine($"Log: {arguments.Log}, Delete: {arguments.Delete}");
            }).Parse(args);
        }
    }

    class Arguments
    {
        [Flag('d', "delete")]
        public bool Delete { get; set; }
        [Flag('l', "log")]
        public bool Log{ get; set; }
    }
}
