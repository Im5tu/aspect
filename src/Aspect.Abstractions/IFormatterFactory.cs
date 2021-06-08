namespace Aspect.Abstractions
{
    public interface IFormatterFactory
    {
        IFormatter GetFormatterFor(FormatterType type);
    }
}