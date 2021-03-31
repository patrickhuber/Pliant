using Pliant.Grammars;

namespace Pliant.Builders
{
    public class IgnoreSettingModel : SettingModel
    {
        public const string SettingKey = "ignore";

        public IgnoreSettingModel(LexerRuleModel lexerRuleModel) 
            : base(SettingKey, lexerRuleModel.Value.TokenType.Id)
        { }

        public IgnoreSettingModel(FullyQualifiedName fullyQualifiedName)
            : base(SettingKey, fullyQualifiedName.FullName)
        { }
    }
}
