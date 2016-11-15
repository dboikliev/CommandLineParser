using System;

namespace CommandLineParser.Exceptions
{
    public class MultipleArgumentAttributesNotAllowedException : Exception
    {
        public MultipleArgumentAttributesNotAllowedException(
            string message = "Multiple argument attributes on the same property are not allowed") : base(message)
        {
            
        }
    }
}
