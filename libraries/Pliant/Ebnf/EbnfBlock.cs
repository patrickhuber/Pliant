using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Ebnf
{
    public abstract class EbnfBlock : EbnfNode
    {
    }

    public class EbnfBlockRule : EbnfBlock
    {
        public EbnfRule Rule { get; private set; }
        public override EbnfNodeType NodeType { get { return EbnfNodeType.EbnfBlockRule; } }
        public EbnfBlockRule(EbnfRule rule)
        {
            Rule = rule;
        }
    }

    public class EbnfBlockSetting : EbnfBlock
    {
        public EbnfSetting Setting { get; private set; }
        public override EbnfNodeType NodeType { get { return EbnfNodeType.EbnfBlockSetting; } }

        public EbnfBlockSetting(EbnfSetting setting)
        {
            Setting = setting;
        }
    }

    public class EbnfBlockLexerRule : EbnfBlock
    {
        public EbnfLexerRule LexerRule { get; private set; }
        public override EbnfNodeType NodeType { get { return EbnfNodeType.EbnfBlockLexerRule; } }

        public EbnfBlockLexerRule(EbnfLexerRule lexerRule)
        {
            LexerRule = lexerRule;
        }
    }
}
