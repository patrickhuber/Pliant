namespace Pliant.Grammars
{
    public class LexerRuleType
    {
        public string Id { get; private set; }

        public LexerRuleType(string id)
        {
            Id = id;
        }

        public override bool Equals(object obj)
        {
            var other = obj as LexerRuleType;
            // order is important here, null must preceed obj 
            // to make sure the == operator is not called
            if (null == obj)
                return false;
            return other.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator == (LexerRuleType first, LexerRuleType second)
        {
            return first.Equals(second);
        }

        public static bool operator != (LexerRuleType first, LexerRuleType second)
        {
            return !first.Equals(second);
        }
    }
}
