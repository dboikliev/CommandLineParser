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
                Assert.Equal("test", arguments.Test);
            });

            parser.Register<CommandArguments>(commandArgs =>
            {
                Assert.Equal("innerTest", commandArgs.InnerOption);
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
                Assert.Equal(10, arguments.Max);
                Assert.Equal("test", arguments.Test);
            });
            parser.Register<CommandArguments>(commandArgs =>
            {
                Assert.Equal(1, commandArgs.Foo);
                Assert.Equal(2, commandArgs.Bar);
                Assert.Equal(3, commandArgs.Moo);
                Assert.Equal("innerTest", commandArgs.InnerOption);
            });
            parser.Parse(args);
        }

        [Fact]
        public void ParseCommand()
        {
            var args = new[] { "command", "1", "2", "-i", "innerTest", "--", "3" };
            var parser = new Parser();
            parser.Register<CommandArguments>(commandArgs =>
            {
                Assert.Equal(1, commandArgs.Foo);
                Assert.Equal(2, commandArgs.Bar);
                Assert.Equal(3, commandArgs.Moo);
                Assert.Equal("innerTest", commandArgs.InnerOption);
            });
            parser.Parse(args);
        }
    }
}
