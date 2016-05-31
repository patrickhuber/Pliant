using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Ebnf
{
    public enum EbnfNodeType
    {
        EbnfDefinition,
        EbnfDefinitionConcatenation,
        EbnfBlockRule,
        EbnfBlockSetting,
        EbnfBlockLexerRule,
        EbnfRule,
        EbnfSetting,
        EbnfLexerRule,
        EbnfExpression,
        EbnfExpressionAlteration,
        EbnfTerm,
        EbnfTermConcatenation,
        EbnfFactorIdentifier,
        EbnfFactorLiteral,
        EbnfFactorRegex,
        EbnfFactorOptional,
        EbnfFactorGrouping,
        EbnfFactorRepetition,
        EbnfSettingIdentifier,
        EbnfQualifiedIdentifier,
        EbnfQualifiedIdentifierConcatenation,
        EbnfLexerRuleTerm,
        EbnfLexerRuleTermConcatenation,
        EbnfLexerRuleFactorLiteral,
        EbnfLexerRuleFactorRegex,
        EbnfLexerRuleExpression,
        EbnfLexerRuleExpressionAlteration,
        EbnfExpressionEmpty,
    }
}
