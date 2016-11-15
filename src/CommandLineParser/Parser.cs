using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandLineParser.Attributes;
using CommandLineParser.Exceptions;
using CommandLineParser.ParsedArguments;
using CommandLineParser.Tokens;

namespace CommandLineParser
{
    public class Parser
    {
        private readonly ParserFactory _parserFactory = new ParserFactory();
        private readonly Tokenizer _tokenizer = new Tokenizer();

        private readonly HashSet<string> _optionNames = new HashSet<string>();
        private readonly HashSet<string> _valueNames = new HashSet<string>();

        private readonly Dictionary<Type, MulticastDelegate> _argumentCallbacks = new Dictionary<Type, MulticastDelegate>();
        private object _topLevelArguments;

        private readonly SortedList<int, PropertyInfo> _valueProperties =
            new SortedList<int, PropertyInfo>();

        private readonly Dictionary<string, PropertyInfo> _argumentProperties =
            new Dictionary<string, PropertyInfo>();

        private int _lastValuePosition;
        private int _currentPosition;


        public Parser Register<T>(Action<T> callback) where T : class, new()
        {
            if (_topLevelArguments != null)
            {
                throw new MultipleTopLevelArgumentsNotAllowedException();
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
                    throw new MultipleArgumentAttributesNotAllowedException();
                }

                if (argumentAttributes.Length == 1)
                {
                    var attribute = argumentAttributes.First();

                    if (attribute is OptionAttribute)
                    {
                        var option = (OptionAttribute)attribute;
                        var argumentProperty = property;
                        _optionNames.Add(option.ShortName);
                        _optionNames.Add(option.LongName);
                        _argumentProperties[option.ShortName] = argumentProperty;
                        _argumentProperties[option.LongName] = argumentProperty;
                    }
                    else if (attribute is ValueAttribute)
                    {
                        var value = (ValueAttribute)attribute;
                        _valueNames.Add(value.Name);

                        _valueProperties[value.Position] = property;
                    }
                }
            }
            _argumentCallbacks[typeof(T)] = callback;
            _topLevelArguments = new T();
            return this;
        }

        public void Parse(string[] args)
        {
            var tokens = _tokenizer.Tokenize(args).ToArray();
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
            valueArgument.Type = valueProperty.PropertyType;
            var parser = _parserFactory.GetParser(valueArgument.Type);
            var value = parser.Parse(valueArgument);
            valueProperty.SetValue(_topLevelArguments, value);
        }

        private void EvaluateOption(ParsedArgument optionArgument)
        {
            var argumentProperty = _argumentProperties[optionArgument.Name];
            optionArgument.Type = argumentProperty.PropertyType;
            var parser = _parserFactory.GetParser(optionArgument.Type);
            var value = parser.Parse(optionArgument);
            argumentProperty.SetValue(_topLevelArguments, value);
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