using Pliant.Grammars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Builders
{
    public class TriviaSettingModel : SettingModel
    {
        public const string SettingKey = "trivia";

        public TriviaSettingModel(LexerRuleModel lexerRuleModel)
            : base(SettingKey, lexerRuleModel.Value.TokenType.Id)
        { }

        public TriviaSettingModel(FullyQualifiedName fullyQualifiedName)
            : base(SettingKey, fullyQualifiedName.FullName)
        {
        }
    }
}
