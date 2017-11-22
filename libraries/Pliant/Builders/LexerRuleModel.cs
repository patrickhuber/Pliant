using Pliant.Grammars;

namespace Pliant.Builders
{
    public class LexerRuleModel : SymbolModel
    {
        public LexerRuleModel() { }

        public LexerRuleModel(ILexerRule value)
        {
            Value = value;
        }

        public override ISymbol Symbol { get { return Value; } }

        public ILexerRule Value { get; set; }

        public override SymbolModelType ModelType
        {
            get { return SymbolModelType.LexerRule; }
        }
        
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (null == obj)
                return false;
            var lexerRuleModel = obj as LexerRuleModel;
            if (null == lexerRuleModel)
                return false;
            return Value.Equals(lexerRuleModel.Value);
        }
    }
}
