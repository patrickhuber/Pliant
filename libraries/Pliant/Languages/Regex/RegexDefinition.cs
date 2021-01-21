using Pliant.Utilities;

namespace Pliant.Languages.Regex
{
    public class RegexDefinition : RegexNode
    {
        public bool StartsWith { get; private set; }
        public RegexExpression Expression { get; private set; }
        public bool EndsWith { get; private set; }

        public override RegexNodeType NodeType
        {
            get { return RegexNodeType.Regex; }
        }

        public RegexDefinition(
            bool startsWith,
            RegexExpression expression,
            bool endsWith)
        {
            StartsWith = startsWith;
            EndsWith = endsWith;
            Expression = expression;
            _hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (!(obj is RegexDefinition other))
                return false;

            return other.EndsWith == EndsWith
                && other.StartsWith == StartsWith
                && other.Expression.Equals(Expression);
        }
        
        private readonly int _hashCode;

        public override int GetHashCode()
        {
            return _hashCode;
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                StartsWith.GetHashCode(),
                Expression.GetHashCode(),
                EndsWith.GetHashCode());
        }

        public override string ToString()
        {
            return $"{(StartsWith ? "^" : string.Empty)}{Expression}{(EndsWith ? "$" : string.Empty)}";
        }
    }
}