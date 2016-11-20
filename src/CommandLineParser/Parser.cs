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
        private readonly HashSet<string> _commandNames = new HashSet<string>();
        private readonly HashSet<string> _valueNames = new HashSet<string>();

        private readonly Dictionary<Type, MulticastDelegate> _argumentCallbacks =
            new Dictionary<Type, MulticastDelegate>();

        private readonly Dictionary<string, Queue<ArgumentProperty>> _valueProperties =
            new Dictionary<string, Queue<ArgumentProperty>>();

        //private readonly SortedList<Tuple<string, int>, ArgumentProperty> _valueProperties =
        //    new SortedList<Tuple<string, int>, ArgumentProperty>();

        private readonly Dictionary<Tuple<string, string>, ArgumentProperty> _argumentProperties =
            new Dictionary<Tuple<string, string>, ArgumentProperty>();

        private int _currentPosition;
        private int _lastValuePosition;

        private readonly Dictionary<string, object> _commandArguments = 
            new Dictionary<string, object>();

        /// <summary>
        /// Registers a type into the parser.
        /// </summary>
        /// <typeparam name="T">The type being registered.</typeparam>
        /// <param name="callback">A callback which will be executed when the values of the registered type have been parsed.</param>
        /// <returns></returns>
        public Parser Register<T>(Action<T> callback) where T : class, new()
        {
            var argumentsType = typeof(T);
            var commandAttribute = argumentsType.GetTypeInfo()
                .GetCustomAttribute<CommandAttribute>();
            if (_arguments != null && commandAttribute == null)
            {
                throw new MultipleTopLevelArgumentsNotAllowedException();
            }
            var currentCommand = string.Empty;
            if (commandAttribute == null)
            {
                _commandNames.Add(string.Empty);
                _commandArguments[string.Empty] = new T();
            }
            else
            {
                if (_commandArguments.ContainsKey(commandAttribute.Name))
                {
                    throw new Exception($"Command whith name {commandAttribute.Name} has already been registered.");
                }
                _commandNames.Add(commandAttribute.Name);
                _commandArguments[commandAttribute.Name] = new T();
                currentCommand = commandAttribute.Name;
            }

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
                        var option = (OptionAttribute)attribute;

                        _optionNames.Add(option.ShortName);
                        _optionNames.Add(option.LongName);
                        _argumentProperties[new Tuple<string, string>(currentCommand, option.ShortName)] = argumentProperty;
                        _argumentProperties[new Tuple<string, string>(currentCommand, option.LongName)] = argumentProperty;
                    }
                    else if (attribute is FlagAttribute)
                    {
                        if (property.PropertyType != typeof(bool))
                        {
                            throw new InvalidAttributeUsageException($"{nameof(FlagAttribute)} can only be used on boolean properties.");
                        }
                        var flag = (FlagAttribute)attribute;
                        _flagNames.Add(flag.ShortName);
                        _flagNames.Add(flag.LongName);
                        _argumentProperties[new Tuple<string, string>(currentCommand, flag.ShortName)] = argumentProperty;
                        _argumentProperties[new Tuple<string, string>(currentCommand, flag.LongName)] = argumentProperty;
                    }
                    else if (attribute is ValueAttribute)
                    {
                        var value = (ValueAttribute)attribute;
                        _valueNames.Add(value.Name);
                        if (!_valueProperties.ContainsKey(currentCommand))
                        {
                            _valueProperties[currentCommand] = new Queue<ArgumentProperty>();
                        }
                        _valueProperties[currentCommand].Enqueue(argumentProperty);
                    }
                }
            }

            _argumentCallbacks[typeof(T)] = callback;
            return this;
        }

        /// <summary>
        /// Iterates over the arguments, maps them to properties of the registered types and executes their callbacks.
        /// </summary>
        /// <param name="args">The command line arguments</param>
        public void Parse(string[] args)
        {
            var tokenizer = new Tokenizer(_optionNames, _flagNames, _commandNames);
            var tokens = tokenizer.Tokenize(args).ToArray();
            Parse(tokens);

            foreach (var commandName in _commandNames)
            {
                var arguments = _commandArguments[commandName];
                _argumentCallbacks[arguments.GetType()].DynamicInvoke(arguments);
            }
        }

        private void Parse(Token[] tokens, string commandName = "")
        {
            _arguments = _commandArguments[commandName];
            while (_currentPosition < tokens.Length)
            {
                if (tokens[_currentPosition].Type == TokenType.Command)
                {
                    Parse(tokens, tokens[_currentPosition++].Value);
                }
                else if (tokens[_currentPosition].Type == TokenType.Option)
                {
                    var option = ParseOption(tokens);
                    option.CommandName = commandName;
                    EvaluateOption(option);
                }
                else if (tokens[_currentPosition].Type == TokenType.Flag)
                {
                    var flag = ParseFlag(tokens);
                    flag.CommandName = commandName;
                    EvaluateFlag(flag);
                }
                else if (tokens[_currentPosition].Type == TokenType.Value)
                {
                    var value = ParseValue(tokens);
                    value.CommandName = commandName;
                    EvaluateValue(value);
                    //_valueProperties.Remove(new Tuple<string, int>(commandName, value.Position));
                }
                _currentPosition++;
            }
        }

        private void EvaluateFlag(ParsedArgument flagArgument)
        {
            var argumentProperty = _argumentProperties[new Tuple<string, string>(flagArgument.CommandName, flagArgument.Name)];
            if (argumentProperty.Evaluated)
            {
                throw new RepeatingArgumentsException($"Argument {flagArgument.Name} has already been evaluated.");
            }
            argumentProperty.Property.SetValue(_arguments, true);
            argumentProperty.Evaluated = true;

        }

        private void EvaluateValue(ParsedArgument valueArgument)
        {
            var argumentProperty = _valueProperties[valueArgument.CommandName].Dequeue();
            var parser = _parserFactory.GetParser(argumentProperty.Property.PropertyType);
            var value = parser.Parse(valueArgument);
            argumentProperty.Property.SetValue(_arguments, value);
        }

        private void EvaluateOption(ParsedArgument optionArgument)
        {
            var argumentProperty = _argumentProperties[new Tuple<string, string>(optionArgument.CommandName, optionArgument.Name)];
            if (argumentProperty.Evaluated)
            {
                throw new RepeatingArgumentsException($"Argument {optionArgument.Name} has already been evaluated.");
            }
            var attribute = (OptionAttribute)argumentProperty.Argument;
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
            argumentProperty.Evaluated = true;
        }

        private ParsedArgument ParseValue(Token[] tokens)
        {
            var token = tokens[_currentPosition];
            var parsedValue = new ParsedArgument
            {
                Values = new[] { token.Value },
            };
            return parsedValue;
        }

        private ParsedArgument ParseOption(Token[] tokens)
        {
            var token = tokens[_currentPosition];
            var parsedOption = new ParsedArgument { Name = token.Value };
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