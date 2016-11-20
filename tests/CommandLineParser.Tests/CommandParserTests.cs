using CommandLineParser.Attributes;
using Xunit;

namespace CommandLineParser.Tests
{
    class Arguments
    {
        [Option('t', "test")]
        public string Test { get; set; }
        [Value(0, "max")]
        public int Max { get; set; }
    }
    [Command("command")]
    class CommandArguments
    {
        [Option('i', "inner")]
        public string InnerOption { get; set; }

        [Value(0, "foo")]
        public int Foo { get; set; }

        [Value(1, "foo")]
        public int Bar { get; set; }

        [Value(2, "moo")]
        public int Moo { get; set; }
    }


    public class CommandParserTests
    {
        [Fact]
        public void ParseCommandArguments()
        {
            var args = new[] { "-t", "test", "command", "-i", "innerTest" };
            var parser = new Parser();
            parser.Register<Arguments>(arguments =>
            {
                Assert.Equal(arguments.Test, "test");
            });
            parser.Register<CommandArguments>(commandArgs =>
            {
                Assert.Equal(commandArgs.InnerOption, "innerTest");
            });
            parser.Parse(args);
        }

        [Fact]
        public void ParseCommandWithPositionalValueARgument()
        {
            var args = new[] { "10", "-t", "test", "command", "1", "2", "-i", "innerTest", "--", "3" };
            var parser = new Parser();
            parser.Register<Arguments>(arguments =>
            {
                Assert.Equal(arguments.Max, 10);
                Assert.Equal(arguments.Test, "test");
            });
            parser.Register<CommandArguments>(commandArgs =>
            {
                Assert.Equal(commandArgs.Foo, 1);
                Assert.Equal(commandArgs.Bar, 2);
                Assert.Equal(commandArgs.Moo, 3);
                Assert.Equal(commandArgs.InnerOption, "innerTest");
            });
            parser.Parse(args);
        }
    }
}
