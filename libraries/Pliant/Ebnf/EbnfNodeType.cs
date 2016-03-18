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
        EbnfTermRepetition,
        EbnfFactorIdentifier,
        EbnfFactorLiteral,
        EbnfFactorRegex,
        EbnfFactorConcatenation,
        EbnfFactorOptional,
        EbnfFactorGrouping,
        EbnfSettingIdentifier,
        EbnfQualifiedIdentifier,
        EbnfQualifiedIdentifierConcatenation,
        EbnfFactorRepetition,
        EbnfLexerRuleTerm,
        EbnfLexerRuleTermConcatenation,
        EbnfLexerRuleFactorLiteral,
        EbnfLexerRuleFactorRegex,
        EbnfLexerRuleExpression,
        EbnfLexerRuleExpressionAlteration,
    }
}
