namespace CommandLineParser
{
    public interface ITypedParser<T>
    {
        T Parse(string value);
    }
}
