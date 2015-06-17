using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (other == null)
                return false;
            return other.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
