namespace CommandLineParser
{
    public interface ITypedParser<T> : ITypedParser
    {
        new T Parse(string value);
    }

    public interface ITypedParser
    {
        object Parse(string value);
    }
}
