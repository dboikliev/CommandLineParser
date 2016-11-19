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
        private object _arguments;

        private readonly ParserFactory _parserFactory = new ParserFactory();
        private readonly HashSet<string> _optionNames = new HashSet<string>();
        private readonly HashSet<string> _flagNames = new HashSet<string>();
        private readonly HashSet<string> _valueNames = new HashSet<string>();

        private readonly Dictionary<Type, MulticastDelegate> _argumentCallbacks =
            new Dictionary<Type, MulticastDelegate>();

        private readonly SortedList<int, ArgumentProperty> _valueProperties =
            new SortedList<int, ArgumentProperty>();

        private readonly Dictionary<string, ArgumentProperty> _argumentProperties =
            new Dictionary<string, ArgumentProperty>();

        private int _currentPosition;
        private int _lastValuePosition;


        public Parser Register<T>(Action<T> callback) where T : class, new()
        {
            if (_arguments != null)
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

                    var argumentProperty = new ArgumentProperty
                    {
                        Argument = attribute,
                        Property = property
                    };

                    if (attribute is OptionAttribute)
                    {
                        var option = (OptionAttribute) attribute;
                        
                        _optionNames.Add(option.ShortName);
                        _optionNames.Add(option.LongName);
                        _argumentProperties[option.ShortName] = argumentProperty;
                        _argumentProperties[option.LongName] = argumentProperty;
                    }
                    else if (attribute is FlagAttribute)
                    {
                        if (property.PropertyType != typeof(bool))
                        {
                            throw new InvalidAttributeUsageException($"{nameof(FlagAttribute)} can only be used on boolean properties.");
                        }
                        var flag = (FlagAttribute) attribute;
                        _flagNames.Add(flag.ShortName);
                        _flagNames.Add(flag.LongName);
                        _argumentProperties[flag.ShortName] = argumentProperty;
                        _argumentProperties[flag.LongName] = argumentProperty;
                    }
                    else if (attribute is ValueAttribute)
                    {
                        var value = (ValueAttribute) attribute;
                        _valueNames.Add(value.Name);

                        _valueProperties[value.Position] = argumentProperty;
                    }
                }
            }

            _argumentCallbacks[typeof(T)] = callback;
            _arguments = new T();
            return this;
        }

        public void Parse(string[] args)
        {
            var tokenizer = new Tokenizer(_optionNames, _flagNames);
            var tokens = tokenizer.Tokenize(args).ToArray();
            Parse(tokens);
            _argumentCallbacks[_arguments.GetType()].DynamicInvoke(_arguments);
        }

        private void Parse(Token[] tokens)
        {
            while (_currentPosition < tokens.Length)
            {
                if (tokens[_currentPosition].Type == TokenType.Option)
                {
                    var option = ParseOption(tokens);
                    var argumentProperty = _argumentProperties[option.Name];
                    if (argumentProperty.Evaluated)
                    {
                        throw new RepeatingArgumentsException($"Argument {option.Name} has already been evaluated.");
                    }
                    EvaluateOption(option);
                    argumentProperty.Evaluated = true;
                }
                else if (tokens[_currentPosition].Type == TokenType.Flag)
                {
                    var flag = ParseFlag(tokens);
                    var argumentProperty = _argumentProperties[flag.Name];
                    if (argumentProperty.Evaluated)
                    {
                        throw new RepeatingArgumentsException($"Argument {flag.Name} has already been evaluated.");
                    }
                    EvaluateFlag(flag);
                    argumentProperty.Evaluated = true;
                }
                else if (tokens[_currentPosition].Type == TokenType.Value)
                {
                    var value = ParseValue(tokens);
                    EvaluateValue(value);
                    _valueProperties.Remove(_currentPosition);
                }
                _currentPosition++;
            }
            
            
        }

        private void EvaluateFlag(ParsedArgument flagArgument)
        {
            var argumentProperty = _argumentProperties[flagArgument.Name];
            argumentProperty.Property.SetValue(_arguments, true);
        }

        private void EvaluateValue(ParsedArgument valueArgument)
        {
            var argumentProperty = _valueProperties[valueArgument.Position];
            valueArgument.Type = argumentProperty.Property.PropertyType;
            var parser = _parserFactory.GetParser(valueArgument.Type);
            var value = parser.Parse(valueArgument);
            argumentProperty.Property.SetValue(_arguments, value);
        }

        private void EvaluateOption(ParsedArgument optionArgument)
        {
            var argumentProperty = _argumentProperties[optionArgument.Name];
            var attribute = (OptionAttribute) argumentProperty.Argument;
            optionArgument.Type = argumentProperty.Property.PropertyType;
            object value = null;
            if (optionArgument.Values.Any())
            {
                var parser = _parserFactory.GetParser(optionArgument.Type);
                value = parser.Parse(optionArgument);
            }
            else if (attribute.IsRequired)
            {
                throw new MissingValueForRequiredOption(optionArgument.Name);
            }

            argumentProperty.Property.SetValue(_arguments, value);
        }

        private ParsedArgument ParseValue(Token[] tokens)
        {
            var token = tokens[_currentPosition];
            var parsedValue = new ParsedArgument
            {
                Values = new[] {token.Value},
                Position = _lastValuePosition++
            };
            return parsedValue;
        }

        private ParsedArgument ParseOption(Token[] tokens)
        {
            var token = tokens[_currentPosition];
            var parsedOption = new ParsedArgument {Name = token.Value};
            var values = new List<string>();

            while (_currentPosition + 1 < tokens.Length && tokens[_currentPosition + 1].Type == TokenType.Value)
            {
                _currentPosition++;
                token = tokens[_currentPosition];
                values.Add(token.Value);
            }

            parsedOption.Values = values;
            return parsedOption;
        }

        private ParsedArgument ParseFlag(Token[] tokens)
        {
            var token = tokens[_currentPosition];
            var parsedOption = new ParsedArgument { Name = token.Value };
            return parsedOption;
        }
    }
}