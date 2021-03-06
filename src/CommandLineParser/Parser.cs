﻿using System;
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
        private readonly Options _options;
        private object _currentArguments;
        private int _currentPosition;

        private readonly ParserFactory _parserFactory = new ParserFactory();
        private readonly HashSet<string> _optionNames = new HashSet<string>();
        private readonly HashSet<string> _flagNames = new HashSet<string>();
        private readonly HashSet<string> _commandNames = new HashSet<string>();
        private readonly HashSet<string> _valueNames = new HashSet<string>();

        private readonly Dictionary<Type, Delegate> _argumentCallbacks = new Dictionary<Type, Delegate>();
        private readonly Dictionary<string, Queue<ArgumentProperty>> _valueProperties = new Dictionary<string, Queue<ArgumentProperty>>();
        private readonly Dictionary<string, Dictionary<string, ArgumentProperty>> _argumentProperties = new Dictionary<string, Dictionary<string, ArgumentProperty>>();


        private readonly Dictionary<string, object> _commandArguments = new Dictionary<string, object>();

        private readonly Dictionary<string, string> _commandHelp = new Dictionary<string, string>();

        private readonly Queue<string> _parsedCommands = new Queue<string>();

        public Parser(Options options = null)
        {
            _options = options ?? Options.Default;
        }

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
                _commandHelp[currentCommand] = string.Empty;
            }
            else
            {
                if (_commandArguments.ContainsKey(commandAttribute.Name))
                {
                    throw new InvalidOperationException($"Command whith name \"{commandAttribute.Name}\" has already been registered.");
                }

                _commandNames.Add(commandAttribute.Name);
                _commandArguments[commandAttribute.Name] = new T();
                currentCommand = commandAttribute.Name;
                _commandHelp[currentCommand] = $"{currentCommand}{Environment.NewLine}\t{commandAttribute.Description}{Environment.NewLine}";
            }

            var properties = argumentsType
                .GetRuntimeProperties()
                .ToArray();

            foreach (var property in properties)
            {
                var argumentAttributes = property.GetCustomAttributes<ArgumentAttribute>().ToArray();
                if (argumentAttributes.Length > 1)
                {
                    throw new MultipleArgumentAttributesNotAllowedException();
                }

                if (!_argumentProperties.ContainsKey(currentCommand))
                {
                    _argumentProperties[currentCommand] = new Dictionary<string, ArgumentProperty>();
                }

                if (argumentAttributes.Length == 1)
                {
                    var attribute = argumentAttributes[0];

                    var argumentProperty = new ArgumentProperty
                    {
                        Argument = attribute,
                        Property = property
                    };

                    switch (attribute)
                    {
                        case OptionAttribute option:
                            RegisterOption(currentCommand, argumentProperty, option);
                            _commandHelp[currentCommand] += $"\t{option.ShortName}|{option.LongName} {attribute.Description}{Environment.NewLine}";
                            break;
                        case FlagAttribute flag:
                            if (property.PropertyType != typeof(bool))
                            {
                                throw new InvalidAttributeUsageException($"{nameof(FlagAttribute)} can only be used on boolean properties.");
                            }

                            RegisterFlag(currentCommand, argumentProperty, flag);
                            _commandHelp[currentCommand] += $"\t{flag.ShortName}|{flag.LongName} {attribute.Description}{Environment.NewLine}";
                            break;
                        case ValueAttribute value:
                            RegisterValue(currentCommand, argumentProperty, value);
                            _commandHelp[currentCommand] += $"\t{value.Name} {attribute.Description}{Environment.NewLine}";
                            break;
                    }
                }
            }

            _argumentCallbacks[typeof(T)] = callback;
            return this;
        }

        private void RegisterValue(string currentCommand, ArgumentProperty argumentProperty, ValueAttribute value)
        {
            _valueNames.Add(value.Name);
            if (!_valueProperties.ContainsKey(currentCommand))
            {
                _valueProperties[currentCommand] = new Queue<ArgumentProperty>();
            }
            _valueProperties[currentCommand].Enqueue(argumentProperty);
        }

        private void RegisterFlag(string currentCommand, ArgumentProperty argumentProperty, FlagAttribute flag)
        {
            _flagNames.Add(flag.ShortName);
            _flagNames.Add(flag.LongName);

            _argumentProperties[currentCommand][flag.ShortName] = argumentProperty;
            _argumentProperties[currentCommand][flag.LongName] = argumentProperty;
        }

        private void RegisterOption(string currentCommand, ArgumentProperty argumentProperty, OptionAttribute option)
        {
            _optionNames.Add(option.ShortName);
            _optionNames.Add(option.LongName);

            _argumentProperties[currentCommand][option.ShortName] = argumentProperty;
            _argumentProperties[currentCommand][option.LongName] = argumentProperty;
        }

        /// <summary>
        /// Iterates over the arguments, maps them to properties of the registered types and executes their callbacks.
        /// </summary>
        /// <param name="args">The command line arguments</param>
        public void Parse(string[] args)
        {
            var tokenizer = new Lexer(_options, _optionNames, _flagNames, _commandNames);
            var tokens = tokenizer.Tokenize(args).ToList();
            try
            {
                Parse(tokens);
                EvaluateRemainingProperties();
                ExecuteCallbacks();
            }
            catch (Exception ex) when (ex is MissingRequiredPositionalValueException ||
                                       ex is MissingRequiredOptionException ||
                                       ex is RepeatingArgumentsException ||
                                       ex is MissingValueForOptionException)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ExecuteCallbacks()
        {
            while (_parsedCommands.Count > 0)
            {
                var commandName = _parsedCommands.Dequeue();
                if (_commandArguments.TryGetValue(commandName, out var arguments))
                {
                    _argumentCallbacks[arguments.GetType()].DynamicInvoke(arguments);
                }
            }
        }

        private void EvaluateRemainingProperties()
        {
            foreach (var command in _parsedCommands)
            {
                if (_commandArguments.ContainsKey(command))
                {
                    var commandArgumentProperties = _argumentProperties[command];

                    foreach (var commandArgumentProperty in commandArgumentProperties)
                    {
                        var argumentProperty = commandArgumentProperty.Value;
                        if (argumentProperty.Argument is OptionAttribute attribute && !argumentProperty.Evaluated)
                        {
                            if (attribute.IsRequired)
                            {
                                throw new MissingRequiredOptionException(command, attribute.ShortName,
                                    attribute.LongName);
                            }

                            if (attribute.DefaultValue != null)
                            {
                                var commandArguments = _commandArguments[command];
                                argumentProperty.Property.SetValue(commandArguments,
                                    attribute.DefaultValue);
                            }
                        }
                    }

                    while (_valueProperties.ContainsKey(command) && _valueProperties[command].Count > 0)
                    {
                        var valueProperty = _valueProperties[command].Dequeue();
                        if (!valueProperty.Evaluated)
                        {
                            var attribute = (ValueAttribute)valueProperty.Argument;
                            if (attribute.IsRequired)
                            {
                                throw new MissingRequiredPositionalValueException(command, attribute.Name);
                            }

                            if (attribute.DefaultValue != null)
                            {
                                var commandArguments = _commandArguments[command];
                                valueProperty.Property.SetValue(commandArguments,
                                    attribute.DefaultValue);
                            }
                        }
                    }
                }
            }
        }

        private void Parse(IReadOnlyList<Token> tokens, string commandName = "")
        {
            _parsedCommands.Enqueue(commandName);
            while (_currentPosition < tokens.Count)
            {
                switch (tokens[_currentPosition].Type)
                {
                    case TokenType.Command:
                        commandName = tokens[_currentPosition].Symbol;
                        _currentArguments = _commandArguments[commandName];
                        _currentPosition++;
                        Parse(tokens, commandName);
                        break;
                    case TokenType.Option:
                        var option = ParseOption(tokens);
                        EvaluateOption(commandName, option);
                        break;
                    case TokenType.Flag:
                        var flag = ParseFlag(tokens);
                        EvaluateFlag(commandName, flag);
                        break;
                    case TokenType.Value:
                        var value = ParseValue(tokens);
                        EvaluateValue(commandName, value);
                        break;
                    case TokenType.Help when tokens[_currentPosition - 1].Type == TokenType.Command:
                        PrintHelp(commandName);
                        _currentPosition = tokens.Count;
                        _parsedCommands.Clear();
                        break;
                }
                _currentPosition++;
            }
        }

        private void PrintHelp(string commandName)
        {
            Console.WriteLine(_commandHelp[commandName]);
        }

        private void EvaluateFlag(string command, ParsedArgument flagArgument)
        {
            var argumentProperty = _argumentProperties[command][flagArgument.Name];
            if (argumentProperty.Evaluated)
            {
                throw new RepeatingArgumentsException($"{command}: Argument \"{flagArgument.Name}\" has already been evaluated.");
            }
            argumentProperty.Property.SetValue(_currentArguments, true);
            argumentProperty.Evaluated = true;
        }

        private void EvaluateValue(string command, ParsedArgument valueArgument)
        {
            var argumentProperty = _valueProperties[command].Dequeue();
            var parser = _parserFactory.GetParser(argumentProperty.Property.PropertyType);
            var value = parser.Parse(valueArgument);
            argumentProperty.Property.SetValue(_currentArguments, value);
        }

        private void EvaluateOption(string command, ParsedArgument optionArgument)
        {
            var argumentProperty = _argumentProperties[command][optionArgument.Name];
            if (argumentProperty.Evaluated)
            {
                throw new RepeatingArgumentsException($"{command}: Argument \"{optionArgument.Name}\" has already been evaluated.");
            }
            
            optionArgument.Type = argumentProperty.Property.PropertyType;

            if (!optionArgument.Values.Any())
            {
                throw new MissingValueForOptionException(command, optionArgument.Name);
            }

            var parser = _parserFactory.GetParser(optionArgument.Type);
            var value = parser.Parse(optionArgument);
            argumentProperty.Property.SetValue(_currentArguments, value);
            argumentProperty.Evaluated = true;
        }

        private ParsedArgument ParseValue(IReadOnlyList<Token> tokens)
        {
            var token = tokens[_currentPosition];
            var parsedValue = new ParsedArgument
            {
                Values = new[] { token.Symbol },
            };
            return parsedValue;
        }

        private ParsedArgument ParseOption(IReadOnlyList<Token> tokens)
        {
            var token = tokens[_currentPosition];
            var parsedOption = new ParsedArgument { Name = token.Symbol };
            var values = new List<string>();

            while (_currentPosition + 1 < tokens.Count && tokens[_currentPosition + 1].Type == TokenType.Value)
            {
                _currentPosition++;
                token = tokens[_currentPosition];
                values.Add(token.Symbol);
            }

            parsedOption.Values = values;
            return parsedOption;
        }

        private ParsedArgument ParseFlag(IReadOnlyList<Token> tokens)
        {
            var token = tokens[_currentPosition];
            var parsedOption = new ParsedArgument { Name = token.Symbol };
            return parsedOption;
        }
    }
}