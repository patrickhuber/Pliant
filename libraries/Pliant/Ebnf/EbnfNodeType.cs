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
        EbnfDefinitionRepetition,
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
        EbnfFactorRepetition,
        EbnfFactorOptional,
        EbnfFactorGrouping,
        EbnfSettingIdentifier,
    }
}
