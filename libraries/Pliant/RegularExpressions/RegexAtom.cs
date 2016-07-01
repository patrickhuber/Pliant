using Pliant.Utilities;
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
        const string Dot = ".";

        public override RegexNodeType NodeType
        {
            get { return RegexNodeType.RegexAtomAny; }
        }

        public override string ToString()
        {
            return Dot;
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var any = obj as RegexAtomAny;
            if ((object)any == null)
                return false;
            return true;
        }

        public override int GetHashCode()
        {
            return Dot.GetHashCode();
        }
    }

    public class RegexAtomCharacter : RegexAtom
    {
        public RegexCharacter Character { get; private set; }

        public RegexAtomCharacter(RegexCharacter character)
        {
            Character = character;
            _hashCode = ComputeHashCode();
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
        
        private readonly int _hashCode;

        public override int GetHashCode()
        {
            return _hashCode;
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(Character.GetHashCode());
        }

        public override RegexNodeType NodeType
        {
            get { return RegexNodeType.RegexAtomCharacter; }
        }

        public override string ToString()
        {
            return Character.ToString();
        }
    }

    public class RegexAtomExpression : RegexAtom
    {
        public RegexExpression Expression { get; private set; }

        public RegexAtomExpression(RegexExpression expression)
        {
            Expression = expression;
            _hashCode = ComputeHashCode();
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
        
        private readonly int _hashCode;

        int ComputeHashCode()
        {
            return HashCode.Compute(Expression.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override RegexNodeType NodeType
        {
            get { return RegexNodeType.RegexAtomExpression; }
        }

        public override string ToString()
        {
            return $"({Expression})";
        }
    }

    public class RegexAtomSet : RegexAtom
    {
        public RegexSet Set { get; private set; }

        public RegexAtomSet(RegexSet set)
        {
            Set = set;
            _hashCode = ComputeHashCode();
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
        
        private readonly int _hashCode;

        private int ComputeHashCode()
        {
            return HashCode.Compute(Set.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override RegexNodeType NodeType
        {
            get { return RegexNodeType.RegexAtomSet; }
        }

        public override string ToString()
        {
            return Set.ToString();            
        }
    }
}