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
        private object _currentArguments;

        private readonly ParserFactory _parserFactory = new ParserFactory();
        private readonly HashSet<string> _optionNames = new HashSet<string>();
        private readonly HashSet<string> _flagNames = new HashSet<string>();
        private readonly HashSet<string> _commandNames = new HashSet<string>();
        private readonly HashSet<string> _valueNames = new HashSet<string>();

        private readonly Dictionary<Type, MulticastDelegate> _argumentCallbacks =
            new Dictionary<Type, MulticastDelegate>();

        private readonly Dictionary<string, Queue<ArgumentProperty>> _valueProperties =
            new Dictionary<string, Queue<ArgumentProperty>>();


        private readonly Dictionary<string, Dictionary<string, ArgumentProperty>> _argumentProperties =
            new Dictionary<string, Dictionary<string, ArgumentProperty>>();

        private int _currentPosition;

        private readonly Dictionary<string, object> _commandArguments =
            new Dictionary<string, object>();

        private readonly Queue<string> _parsedCommands = new Queue<string>();

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
            if (_currentArguments != null && commandAttribute == null)
            {
                throw new MultipleTopLevelArgumentsNotAllowedException();
            }
            var currentCommand = string.Empty;
            if (commandAttribute == null)
            {
                _commandNames.Add(string.Empty);
                _commandArguments[string.Empty] = new T();
                _currentArguments = _commandArguments[string.Empty];
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

                if (!_argumentProperties.ContainsKey(currentCommand))
                {
                    _argumentProperties[currentCommand] =
                        new Dictionary<string, ArgumentProperty>();
                }

                if (argumentAttributes.Length == 1)
                {
                    var attribute = argumentAttributes[0];

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

                        _argumentProperties[currentCommand][option.ShortName] = argumentProperty;
                        _argumentProperties[currentCommand][option.LongName] = argumentProperty;
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
                        _argumentProperties[currentCommand][flag.ShortName] = argumentProperty;
                        _argumentProperties[currentCommand][flag.LongName] = argumentProperty;
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
            EvaluateRemainingProperties();
        }

        private void EvaluateRemainingProperties()
        {
            while (_parsedCommands.Count > 0)
            {
                var commandName = _parsedCommands.Dequeue();
                if (_commandArguments.ContainsKey(commandName))
                {
                    var commandArgumentProperties = _argumentProperties[commandName];

                    foreach (var commandArgumentProperty in commandArgumentProperties)
                    {
                        var argumentProperty = commandArgumentProperty.Value;

                        if (argumentProperty.Argument is OptionAttribute)
                        {
                            var attribute = ((OptionAttribute)argumentProperty.Argument);
                            if (!argumentProperty.Evaluated)
                            {
                                if (attribute.IsRequired)
                                {
                                    throw new Exception(
                                        $"A value was not provided for a required option with name {attribute.LongName}");
                                }

                                if (attribute.DefaultValue != null)
                                {
                                    var commandArguments = _commandArguments[commandName];
                                    argumentProperty.Property.SetValue(commandArguments,
                                        attribute.DefaultValue);
                                }
                            }
                        }
                        else if (argumentProperty.Argument is ValueAttribute)
                        {
                            var attribute = ((ValueAttribute)argumentProperty.Argument);
                            if (!argumentProperty.Evaluated)
                            {
                                if (attribute.IsRequired)
                                {
                                    throw new Exception(
                                        $"A value was not provided for a required positional value with name {attribute.Name}");
                                }
                                if (attribute.DefaultValue != null)
                                {
                                    var commandArguments = _commandArguments[commandName];
                                    argumentProperty.Property.SetValue(commandArguments,
                                        attribute.DefaultValue);
                                }
                            }
                        }
                    }

                    var arguments = _commandArguments[commandName];
                    _argumentCallbacks[arguments.GetType()].DynamicInvoke(arguments);
                }
            }
        }

        private void Parse(Token[] tokens, string commandName = "")
        {
            _parsedCommands.Enqueue(commandName);
            while (_currentPosition < tokens.Length)
            {
                if (tokens[_currentPosition].Type == TokenType.Command)
                {
                    commandName = tokens[_currentPosition].Value;
                    _currentArguments = _commandArguments[commandName];
                    _currentPosition++;
                    Parse(tokens, commandName);
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
                }
                _currentPosition++;
            }
        }

        private void EvaluateFlag(ParsedArgument flagArgument)
        {
            var argumentProperty = _argumentProperties[flagArgument.CommandName][flagArgument.Name];
            if (argumentProperty.Evaluated)
            {
                throw new RepeatingArgumentsException($"Argument {flagArgument.Name} has already been evaluated.");
            }
            argumentProperty.Property.SetValue(_currentArguments, true);
            argumentProperty.Evaluated = true;

        }

        private void EvaluateValue(ParsedArgument valueArgument)
        {
            var argumentProperty = _valueProperties[valueArgument.CommandName].Dequeue();
            var parser = _parserFactory.GetParser(argumentProperty.Property.PropertyType);
            var value = parser.Parse(valueArgument);
            argumentProperty.Property.SetValue(_currentArguments, value);
        }

        private void EvaluateOption(ParsedArgument optionArgument)
        {
            var argumentProperty = _argumentProperties[optionArgument.CommandName][optionArgument.Name];
            if (argumentProperty.Evaluated)
            {
                throw new RepeatingArgumentsException($"Argument {optionArgument.Name} has already been evaluated.");
            }
            optionArgument.Type = argumentProperty.Property.PropertyType;

            if (!optionArgument.Values.Any())
            {
                throw new MissingValueForOption(optionArgument.Name);
            }

            var parser = _parserFactory.GetParser(optionArgument.Type);
            var value = parser.Parse(optionArgument);
            argumentProperty.Property.SetValue(_currentArguments, value);
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