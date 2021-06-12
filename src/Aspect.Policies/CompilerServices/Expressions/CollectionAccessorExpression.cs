using System.Reflection;
using Aspect.Policies.CompilerServices.SyntaxTokens;

namespace Aspect.Policies.CompilerServices.Expressions
{
    internal class CollectionAccessorExpression : AccessorExpression
    {
        public IndexerMode Mode { get; }
        public SyntaxToken Token { get; }

        public CollectionAccessorExpression(AbstractExpression accessor, PropertyInfo property, IndexerMode mode, SyntaxToken token)
            : base (accessor, property)
        {
            Mode = mode;
            Token = token;
        }

        public override string ToString()
        {
            return Mode switch
            {
                IndexerMode.Any => $"{Accessor}.{Property.Name}[_]",
                _ => $"{Accessor}.{Property.Name}[*]"
            };
        }

        public enum IndexerMode
        {
            All,
            Any
        }
    }
}
