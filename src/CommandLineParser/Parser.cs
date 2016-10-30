using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandLineParser
{
    public class Parser
    {
        private readonly ParserOptions _options;
        private readonly ParserFactory _parserFactory = new ParserFactory();

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

        public T Parse<T>(string[] args) where T : class
        {
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
            return null;
        }
    }
}
