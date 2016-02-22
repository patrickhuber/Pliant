using System;

namespace Pliant.RegularExpressions
{
    public abstract class RegexAtom : RegexNode
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
            _hashCode = new Lazy<int>(ComputeHashCode);
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
        
        private readonly Lazy<int> _hashCode;

        public override int GetHashCode()
        {
            return _hashCode.Value;
        }

        private int ComputeHashCode()
        {
            return HashUtil.ComputeHash(Character.GetHashCode());
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
        {
            Expression = expression;
            _hashCode = new Lazy<int>(ComputeHashCode);
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
        
        private readonly Lazy<int> _hashCode;

        int ComputeHashCode()
        {
            return HashUtil.ComputeHash(Expression.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode.Value;
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
            _hashCode = new Lazy<int>(ComputeHashCode);
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
        
        private readonly Lazy<int> _hashCode;

        private int ComputeHashCode()
        {
            return HashUtil.ComputeHash(Set.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode.Value;
        }

        public override RegexNodeType NodeType
        {
            get { return RegexNodeType.RegexAtomSet; }
        }
    }
}