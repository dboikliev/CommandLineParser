namespace CommandLineParser.Extensions
{
    internal static class StringExtensions
    {
        internal static string TrimStart(this string text, string prefix)
        {
            var current = text;

            while (current.StartsWith(prefix))
            {
                current = current.Substring(prefix.Length);
            }

            return current;
        }
    }
}
