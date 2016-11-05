using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandLineParser
{
    public class ParserFactory
    {
        private static readonly Dictionary<Type, Lazy<ITypedParser>> TypedParsers = new Dictionary<Type, Lazy<ITypedParser>>();

        static ParserFactory()
        {
            var typedParserInterface = typeof(ITypedParser<>);
            var types = typedParserInterface.GetTypeInfo().Assembly.GetTypes();

            foreach (var type in types)
            {
                var typeInfo = type.GetTypeInfo();
                var typedInterface = typeInfo.GetInterfaces()
                    .FirstOrDefault(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typedParserInterface);
                if (typedInterface != null)
                {
                    TypedParsers[typedInterface.GenericTypeArguments.First()] = new Lazy<ITypedParser>(() => (ITypedParser)Activator.CreateInstance(type));
                }
            }
        }

        public ITypedParser<T> GetParser<T>()
        {
            return (ITypedParser<T>)TypedParsers[typeof(T)].Value;
        }

        internal ITypedParser GetParser(Type propertyType)
        {
            return TypedParsers[propertyType].Value;
        }
    }
}
