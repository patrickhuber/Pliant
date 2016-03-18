using Pliant.Builders;
using Pliant.Grammars;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pliant.Ebnf
{
    public class EbnfGrammarGenerator
    {
        public IGrammar Generate(EbnfDefinition ebnf)
        {
            var grammarContext = new GrammarContext();
            VisitDefinition(grammarContext, ebnf);
            return grammarContext.ToGrammar();
        }

        private void VisitDefinition(GrammarContext grammarContext, EbnfDefinition definition)
        {
            switch (definition.NodeType)
            {
                case EbnfNodeType.EbnfDefinition:
                    VisitBlock(grammarContext, definition.Block);
                    break;

                case EbnfNodeType.EbnfDefinitionConcatenation:
                    var ebnfDefinitionContactenation = definition as EbnfDefinitionConcatenation;
                    VisitBlock(grammarContext, ebnfDefinitionContactenation.Block);
                    VisitDefinition(grammarContext, ebnfDefinitionContactenation.Definition);
                    break;
            }
        }

        private void VisitBlock(GrammarContext grammarContext, EbnfBlock block)
        {
            switch (block.NodeType)
            {
                case EbnfNodeType.EbnfBlockLexerRule:
                    var blockLexerRule = block as EbnfBlockLexerRule;
                    VisitLexerRule(grammarContext, blockLexerRule.LexerRule);
                    break;
                case EbnfNodeType.EbnfBlockRule:
                    var blockRule = block as EbnfBlockRule;
                    VisitRule(grammarContext, blockRule.Rule);
                    break;
                case EbnfNodeType.EbnfBlockSetting:
                    var blockSetting = block as EbnfBlockSetting;
                    VisitSetting(grammarContext, blockSetting.Setting);
                    break;
            }
        }

        private void VisitLexerRule(GrammarContext grammarContext, EbnfLexerRule lexerRule)
        {
            var tokenType = GetQualifiedIdentifierValue(lexerRule.QualifiedIdentifier);
        
        }

        private string GetQualifiedIdentifierValue(EbnfQualifiedIdentifier qualifiedIdentifier)
        {
            switch (qualifiedIdentifier.NodeType)
            {
                case EbnfNodeType.EbnfQualifiedIdentifierConcatenation:
                    var ebnfQualifiedIdentifierContatination = qualifiedIdentifier as EbnfQualifiedIdentifierConcatenation;
                    var childValue = GetQualifiedIdentifierValue(ebnfQualifiedIdentifierContatination.QualifiedIdentifier);
                    return ebnfQualifiedIdentifierContatination.Identifier + childValue;
                    
                default:
                    return qualifiedIdentifier.Identifier;
            }            
        }

        private void VisitRule(GrammarContext grammarContext, EbnfRule rule)
        {
            throw new NotImplementedException();
        }

        private void VisitSetting(GrammarContext grammarContext, EbnfSetting setting)
        {
            throw new NotImplementedException();
        }

        private class GrammarContext : IGrammarBuilder
        {
            public IGrammar ToGrammar()
            {
                throw new NotImplementedException();
            }
        }
    }
}
