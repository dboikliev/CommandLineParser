using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandLineParser.Tokens;

namespace CommandLineParser
{

    public class Parser
    {
        private readonly ParserFactory _parserFactory = new ParserFactory();
        private readonly Tokenizer _tokenizer;

        private readonly HashSet<string> _optionNames = new HashSet<string>();
        private readonly HashSet<string> _valueNames = new HashSet<string>();

        private readonly Dictionary<Type, MulticastDelegate> _argumentCallbacks = new Dictionary<Type, MulticastDelegate>();
        private object _topLevelArguments;
        private readonly SortedList<int, ArgumentProperty> _valueProperties =
            new SortedList<int, ArgumentProperty>();


        private readonly Dictionary<string, ArgumentProperty> _argumentProperties =
            new Dictionary<string, ArgumentProperty>();

        private IEnumerator<Token> _enumerator;

        public Parser()
        {
            _tokenizer = new Tokenizer();
        }

        public Parser On<T>(Action<T> callback) where T : class, new()
        {
            _argumentCallbacks[typeof(T)] = callback;
            return this;
        }

        public Parser Register<T>() where T : class, new()
        {
            if (_topLevelArguments != null)
            {
                throw new Exception("Cannot register multiple top level arguments.");
            }

            var argumentsType = typeof(T);

            var properties = argumentsType
                .GetRuntimeProperties()
                .ToArray();

            foreach (var property in properties)
            {
                var argumentAttributes = property.GetCustomAttributes<ArgumentAttribute>()
                    .ToArray();
                if (argumentAttributes.Length > 1)
                {
                    throw new Exception("Multiple argument attributes on the same target are not allowed.");
                }

                if (argumentAttributes.Length == 1)
                {
                    var attribute = argumentAttributes.First();

                    if (attribute is OptionAttribute)
                    {
                        var option = (OptionAttribute)attribute;
                        var argumentProperty = new ArgumentProperty()
                        {
                            Argument = option,
                            Property = property
                        };
                        _optionNames.Add(option.ShortName);
                        _optionNames.Add(option.LongName);
                        _argumentProperties[option.ShortName] = argumentProperty;
                        _argumentProperties[option.LongName] = argumentProperty;
                    }
                    else if (attribute is ValueAttribute)
                    {
                        var value = (ValueAttribute)attribute;
                        _valueNames.Add(value.Name);

                        _valueProperties[value.Position] = new ArgumentProperty()
                        {
                            Argument = value,
                            Property = property
                        };
                    }
                }
            }
            _topLevelArguments = new T();
            return this;
        }

        public void Parse(string[] args)
        {
            var tokens = _tokenizer.Tokenize(args).ToArray();
            _enumerator = ((IEnumerable<Token>)tokens).GetEnumerator();
            Parse(tokens);
            _argumentCallbacks[_topLevelArguments.GetType()].DynamicInvoke(_topLevelArguments);
        }

        private void Parse(IEnumerable<Token> tokens)
        {
            while (_enumerator.MoveNext())
            {
                var token = _enumerator.Current;
                switch (token.Type)
                {
                    case TokenType.Option:
                        ParserOption(tokens);
                        continue;
                    case TokenType.Value:
                        ParsePositionalValue(token);
                        continue;
                    default:
                        return;
                }
            }
        }

        private void ParsePositionalValue(Token token)
        {
            var property = _valueProperties[token.Position].Property;
            var parser = _parserFactory.GetParser(property.PropertyType);
            var parsedValue = parser.Parse(token.Value);
            property.SetValue(_topLevelArguments, parsedValue);
        }

        private void ParserOption(IEnumerable<Token> tokens)
        {
            var optionToken = _enumerator.Current;
            _enumerator.MoveNext();
            var valueToken = _enumerator.Current;
            if (valueToken.Type == TokenType.Value)
            {
                var property = _argumentProperties[optionToken.Value].Property;
                var parser = _parserFactory.GetParser(property.PropertyType);
                var parsedValue = parser.Parse(valueToken.Value);
                property.SetValue(_topLevelArguments, parsedValue);
            }
            Parse(tokens);
        }
    }
}
