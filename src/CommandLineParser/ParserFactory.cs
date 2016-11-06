using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandLineParser.Exceptions;

namespace CommandLineParser
{
    public class ParserFactory
    {
        private static readonly Dictionary<Type, Lazy<ITypedParser>> TypedParsers =
            new Dictionary<Type, Lazy<ITypedParser>>();

        static ParserFactory()
        {
            var typedParserInterface = typeof(ITypedParser<>);
            var types = typedParserInterface.GetTypeInfo().Assembly.GetTypes();

            foreach (var type in types)
            {
                var typeInfo = type.GetTypeInfo();
                var typedInterface = typeInfo.GetInterfaces()
                    .FirstOrDefault(i => i.GetTypeInfo().IsGenericType 
                        && i.GetGenericTypeDefinition() == typedParserInterface);
                if (typedInterface != null)
                {
                    var lazyParser = new Lazy<ITypedParser>(() => (ITypedParser) Activator.CreateInstance(type));
                    TypedParsers[typedInterface.GenericTypeArguments.First()] = lazyParser;
                }
            }
        }

        public ITypedParser<T> GetParser<T>()
        {
            return (ITypedParser<T>)GetParser(typeof(T));
        }

        internal ITypedParser GetParser(Type type)
        {
            if (!TypedParsers.ContainsKey(type))
            {
                throw new TypeNotSupportedException(type);
            }
            return TypedParsers[type].Value;
        }
    }
}
