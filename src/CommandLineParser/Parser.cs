using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandLineParser.ParsedArguments;
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

        private int _lastValuePosition = 0;
        private int _currentPosition = 0;

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
            //_enumerator = ((IEnumerable<Token>)tokens).GetEnumerator();
            Parse(tokens);
            _argumentCallbacks[_topLevelArguments.GetType()].DynamicInvoke(_topLevelArguments);
        }

        private void Parse(Token[] tokens)
        {
            while (_currentPosition < tokens.Length)
            {
                if (tokens[_currentPosition].Type == TokenType.Option)
                {
                    var option = ParseOption(tokens);
                    EvaluateOption(option);
                }
                else if (tokens[_currentPosition].Type == TokenType.Value)
                {
                    var value = ParseValue(tokens);
                    EvaluateValue(value);
                }
                _currentPosition++;
            }
        }

        private void EvaluateValue(ParsedArgument valueArgument)
        {
            var valueProperty = _valueProperties[valueArgument.Position];
            var parser = _parserFactory.GetParser(valueProperty.Property.PropertyType);
            var value = parser.Parse(valueArgument.Values);
            valueProperty.Property.SetValue(_topLevelArguments, value);
        }

        private void EvaluateOption(ParsedArgument optionArgument)
        {
            var argumentProperty = _argumentProperties[optionArgument.Name];
            var parser = _parserFactory.GetParser(argumentProperty.Property.PropertyType);
            var value = parser.Parse(optionArgument.Values);
            argumentProperty.Property.SetValue(_topLevelArguments, value);
        }

        private ParsedArgument ParseValue(Token[] tokens)
        {
            var token = tokens[_currentPosition];
            var parsedValue = new ParsedArgument
            {
                Values = new[] { token.Value },
                Position = _lastValuePosition++
            };
            return parsedValue;
        }

        private ParsedArgument ParseOption(Token[] tokens)
        {
            var token = tokens[_currentPosition++];
            var parsedOption = new ParsedArgument { Name = token.Value };
            var values = new List<string>();

            while (tokens[_currentPosition].Type == TokenType.Value)
            {
                token = tokens[_currentPosition];
                values.Add(token.Value);

                if (_currentPosition < tokens.Length - 1 && tokens[_currentPosition + 1].Type == TokenType.Value)
                {
                    _currentPosition++;
                }
                else
                {
                    break;
                }
            }

            parsedOption.Values = values;
            return parsedOption;
        }
    }
}
;