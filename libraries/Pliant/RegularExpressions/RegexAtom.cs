namespace Pliant.RegularExpressions
{
    public class RegexAtom : RegexNode
    {
        public override RegexNodeType NodeType
        {
            get { return RegexNodeType.RegexAtom; }
        }
    }

    public class RegexAtomAny : RegexAtom
    {
        public override RegexNodeType NodeType
        {
            get { return RegexNodeType.RegexAtomAny; }
        }
    }

    public class RegexAtomCharacter : RegexAtom
    {
        public RegexCharacter Character { get; private set; }

        public RegexAtomCharacter(RegexCharacter character)
        {
            Character = character;
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var atomCharacter = obj as RegexAtomCharacter;
            if ((object)atomCharacter == null)
                return false;
            return Character.Equals(atomCharacter.Character);
        }

        private bool _isHashCodeSet = false;
        private int _hashCode = 0;

        public override int GetHashCode()
        {
            if (!_isHashCodeSet)
            {
                _hashCode = HashUtil.ComputeHash(Character.GetHashCode());
                _isHashCodeSet = true;
            }
            return _hashCode;
        }

        public override RegexNodeType NodeType
        {
            get { return RegexNodeType.RegexAtomCharacter; }
        }
    }

    public class RegexAtomExpression : RegexAtom
    {
        public RegexExpression Expression { get; private set; }

        public RegexAtomExpression(RegexExpression expression)
            : base()
        {
            Expression = expression;
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var atomExpression = obj as RegexAtomExpression;
            if ((object)atomExpression == null)
                return false;
            return Expression.Equals(atomExpression.Expression);
        }

        private bool _isHashCodeSet = false;
        private int _hashCode = 0;

        public override int GetHashCode()
        {
            if (!_isHashCodeSet)
            {
                _hashCode = HashUtil.ComputeHash(Expression.GetHashCode());
                _isHashCodeSet = true;
            }
            return _hashCode;
        }

        public override RegexNodeType NodeType
        {
            get { return RegexNodeType.RegexAtomExpression; }
        }
    }

    public class RegexAtomSet : RegexAtom
    {
        public RegexSet Set { get; private set; }

        public RegexAtomSet(RegexSet set)
        {
            Set = set;
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var atomSet = obj as RegexAtomSet;
            if ((object)atomSet == null)
                return false;
            return Set.Equals(atomSet.Set);
        }

        private bool _isHashCodeSet = false;
        private int _hashCode = 0;

        public override int GetHashCode()
        {
            if (!_isHashCodeSet)
            {
                _hashCode = HashUtil.ComputeHash(Set.GetHashCode());
                _isHashCodeSet = true;
            }
            return _hashCode;
        }

        public override RegexNodeType NodeType
        {
            get { return RegexNodeType.RegexAtomSet; }
        }
    }
}