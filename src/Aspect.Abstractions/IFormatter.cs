namespace Aspect.Abstractions
{
    public interface IFormatter
    {
        FormatterType FormatterType { get; }

        string Format<T>(T entity)  where T : class;
    }
}
