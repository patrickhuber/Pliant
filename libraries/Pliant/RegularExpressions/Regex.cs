namespace Pliant.RegularExpressions
{
    public class Regex
    {
        public bool StartsWith { get; private set; }
        public RegexExpression Expression { get; private set; }
        public bool EndsWith { get; private set; }
        
        public Regex(
            bool startsWith, 
            RegexExpression expression, 
            bool endsWith)
        {
            StartsWith = startsWith;
            EndsWith = endsWith;
            Expression = expression;
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;

            var other = obj as Regex;
            if ((object)null == other)
                return false;

            return other.EndsWith == EndsWith
                && other.StartsWith == StartsWith
                && other.Expression.Equals(Expression);
        }

        private bool _isHashCodeSet = false;
        private int _hashCode = 0;

        public override int GetHashCode()
        {
            if (!_isHashCodeSet)
            {
                _hashCode = HashUtil.ComputeHash(
                    StartsWith.GetHashCode(),
                    Expression.GetHashCode(),
                    EndsWith.GetHashCode());
                _isHashCodeSet = true;
            }
            return _hashCode;
        }
    }
}
