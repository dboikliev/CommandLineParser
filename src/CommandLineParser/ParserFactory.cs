﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandLineParser.Exceptions;
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
                    .FirstOrDefault(i => i.GetTypeInfo().IsGenericType && 
                                         i.GetGenericTypeDefinition() == typedParser);
                if (typedInterface != null)
                {
                    var lazyParser = new Lazy<ITypedParser>(() => (ITypedParser)Activator.CreateInstance(type));
                    var genericTypeArgument = typedInterface.GenericTypeArguments.First();
                    AddEnumerableParserForType(genericTypeArgument);
                    TypedParsers[typedInterface.GenericTypeArguments.First()] = lazyParser;
                }
            }
        }

        internal ITypedParser this[Type type] => GetParser(type);

        internal ITypedParser GetParser(Type type)
        {
            if (!TypedParsers.ContainsKey(type))
            {
                if (type.GetTypeInfo().IsEnum)
                {
                    return TypedParsers[typeof(Enum)].Value;
                }
                if (IsEnumerableOfEnum(type))
                {
                    AddEnumerableParserForType(type.GenericTypeArguments.First());
                    return TypedParsers[type].Value;
                }
                throw new TypeNotSupportedException(type);
            }
            return TypedParsers[type].Value;
        }

        private static bool IsEnumerableOfEnum(Type type)
        {
            return type.IsConstructedGenericType
                   && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                   && type.GenericTypeArguments.First().GetTypeInfo().IsEnum;
        }

        private static void AddEnumerableParserForType(Type type)
        {
            var enumerableParserType = typeof(EnumerableValueParser<>).MakeGenericType(type);
            var lazyEnumerableParser = new Lazy<ITypedParser>(() => (ITypedParser)Activator.CreateInstance(enumerableParserType));
            var enumerableKey =
                typeof(IEnumerable<>).MakeGenericType(type);
            TypedParsers[enumerableKey] = lazyEnumerableParser;
        }
    }
}
