using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandLineParser
{
    public class Parser
    {
        public static T Parse<T>(string[] args) where T : class
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
                Console.WriteLine($"{ argumentProperty.Property.PropertyType }, { argumentProperty.Argument }");
            }
            return null;
        }
    }

    class ArgumentProperty
    {
        public PropertyInfo Property { get; set; }
        public ArgumentAttribute Argument { get; set; }
    }
}
