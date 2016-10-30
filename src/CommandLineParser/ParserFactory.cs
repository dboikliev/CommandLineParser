using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandLineParser
{
    public class ParserFactory
    {
        private readonly Dictionary<Type, object> _typedParsers = new Dictionary<Type, object>();

        public ParserFactory()
        {
            var typedParserInterface = typeof(ITypedParser<Int32>);
            var types = typedParserInterface.GetTypeInfo().Assembly.GetTypes();

            foreach (var type in types)
            {
                var typeInfo = type.GetTypeInfo();
                var typedInterface = typeInfo.GetInterfaces()
                    .FirstOrDefault(i => i.GetTypeInfo().IsGenericType && i == typedParserInterface);
                if (typedInterface != null)
                {
                    _typedParsers[typedInterface.GenericTypeArguments.First()] = Activator.CreateInstance(type);
                }
            }
        }

        public ITypedParser<T> GetParser<T>()
        {
            return (ITypedParser<T>)_typedParsers[typeof(T)];
        }
    }
}
