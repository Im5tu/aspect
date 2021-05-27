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
            switch (Mode)
            {
                case IndexerMode.AtLeastOne:
                    return $"{Accessor}.{Property.Name}[_]";
                case IndexerMode.IndexValue:
                    return $"{Accessor}.{Property.Name}[{((NumericValueSyntaxToken)Token).Value}]";
            }

            return $"{Accessor}.{Property.Name}[*]";
        }

        public enum IndexerMode
        {
            Everything,
            AtLeastOne,
            IndexValue
        }
    }
}
