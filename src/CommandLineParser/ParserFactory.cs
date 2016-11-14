using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandLineParser.Exceptions;
using CommandLineParser.ParsedArguments;
using CommandLineParser.TypeParsers;

namespace CommandLineParser
{
    public class ParserFactory
    {
        private static readonly Dictionary<Type, Lazy<ITypedParser>> TypedParsers =
            new Dictionary<Type, Lazy<ITypedParser>>();

        static ParserFactory()
        {
            var typedParser = typeof(ITypedValueParser<>);
            var types = typedParser.GetTypeInfo().Assembly
                .GetTypes()
                .Where(type => !type.GetTypeInfo().IsAbstract);

            foreach (var type in types)
            {
                var typeInfo = type.GetTypeInfo();
                var typedInterface = typeInfo.GetInterfaces()
                    .FirstOrDefault(i =>
                    {
                        if (i.GetTypeInfo().IsGenericType)
                        {
                            var genericInfo = i.GetGenericTypeDefinition();
                            if (genericInfo == typedParser)
                            {
                                return true;
                            }
                        }
                        return false;
                    });
                if (typedInterface != null)
                {
                    var lazyParser = new Lazy<ITypedParser>(() => (ITypedParser)Activator.CreateInstance(type));
                    var enumerableParserType = typeof(EnumerableValueParser<>).MakeGenericType(typedInterface.GenericTypeArguments.First());
                    var lazyEnumerableParser = new Lazy<ITypedParser>(() => (ITypedParser)Activator.CreateInstance(enumerableParserType));
                    var enumerableKey =
                        typeof(IEnumerable<>).MakeGenericType(typedInterface.GenericTypeArguments.First());
                    TypedParsers[typedInterface.GenericTypeArguments.First()] = lazyParser;
                    TypedParsers[enumerableKey] = lazyEnumerableParser;
                }
            }
        }

        public ITypedParser this[Type type] => GetParser(type);

        public ITypedParser GetParser(Type type)
        {
            if (!TypedParsers.ContainsKey(type))
            {
                throw new TypeNotSupportedException(type);
            }
            return TypedParsers[type].Value;
        }
    }
}
