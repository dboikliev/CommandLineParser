using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandLineParser
{
    public class Parser
    {
        private readonly ParserOptions _options;
        private readonly ParserFactory _parserFactory = new ParserFactory();

        private readonly IDictionary<Type, Action<object>> _argumentCallbacks =
            new Dictionary<Type, Action<object>>();

        private readonly HashSet<string> _arguments = new HashSet<string>();

        public Parser() : this(new ParserOptions()) 
        {
        }

        public Parser(ParserOptions options)
        {
            if (options == null)
            {
                throw new ArgumentException("The parser requires opions.", nameof(options));
            }
            _options = options;
        }

        public Parser On<T>(Action<T> callback) where T : class 
        {
            var type = typeof(T);
            if (_argumentCallbacks.ContainsKey(type))
            {
                throw new Exception($"A callback is registered for type { type }.");
            }
            _argumentCallbacks[type] = (Action<object>)callback;
            return this;
        }

        public Parser Parse(string[] args)
        {
            throw new NotImplementedException();
        }

        public Parser Register<T>()
        {
            throw new NotImplementedException();

            var properties = typeof(T)
                .GetRuntimeProperties()
                .ToArray();

            var argumentProperties = new List<ArgumentProperty>();
            foreach (var property in properties)
            {
                var argumentAttributes = property.GetCustomAttributes<ArgumentAttribute>()
                    .ToArray();
                if (argumentAttributes.Count() > 1)
                {
                    throw new Exception("Multiple argument attributes on the same target are not allowed.");
                }

                var attribute = argumentAttributes.First();

                argumentProperties.Add(new ArgumentProperty
                {
                    Argument = attribute,
                    Property = property
                });

            }

            foreach (var argumentProperty in argumentProperties)
            {

            }
            return this;
        }
    }
}
