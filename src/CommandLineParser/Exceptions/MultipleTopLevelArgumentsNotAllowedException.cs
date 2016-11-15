using System;

namespace CommandLineParser.Exceptions
{
    public class MultipleTopLevelArgumentsNotAllowedException : Exception
    {
        public MultipleTopLevelArgumentsNotAllowedException(
            string message = "Multiple top level arguments are not allowed.") : base(message)
        {

        }
    }
}
