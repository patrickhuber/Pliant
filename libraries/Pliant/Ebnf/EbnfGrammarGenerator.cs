using Pliant.Automata;
using Pliant.Builders;
using Pliant.Grammars;
using Pliant.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pliant.Ebnf
{
    public class EbnfGrammarGenerator
    {
        IEbnfProductionNamingStrategy _strategy;
        SubsetConstructionAlgorithm _subsetConstructionAlgorithm;
        ThompsonConstructionAlgorithm _thompsonConstructionAlgorithm;

        public EbnfGrammarGenerator(IEbnfProductionNamingStrategy strategy)
        {
            _strategy = strategy;
            _thompsonConstructionAlgorithm = new ThompsonConstructionAlgorithm();
            _subsetConstructionAlgorithm = new SubsetConstructionAlgorithm();
        }

        public IGrammar Generate(EbnfDefinition ebnf)
        {
            throw new NotImplementedException();
        }

        private static string GetQualifiedIdentifierValue(EbnfQualifiedIdentifier qualifiedIdentifier)
        {
            var stringBuilder = new StringBuilder();
            var currentQualifiedIdentifier = qualifiedIdentifier;
            var index = 0;
            while (currentQualifiedIdentifier.NodeType == EbnfNodeType.EbnfQualifiedIdentifierConcatenation)
            {
                if (index > 0)
                    stringBuilder.Append(".");
                stringBuilder.Append(currentQualifiedIdentifier.Identifier);
                currentQualifiedIdentifier = (currentQualifiedIdentifier as EbnfQualifiedIdentifierConcatenation).QualifiedIdentifier;
                index++;
            }
            return stringBuilder.ToString();
        }

        private static ILexerRule GetLexerRule(EbnfLexerRule lexerRule)
        {
            var identifier = GetQualifiedIdentifierValue(lexerRule.QualifiedIdentifier);
            var expression = lexerRule.Expression;
            throw new NotImplementedException();
        }

        private static ILexerRule GetLiteralLexerRule(EbnfLexerRuleFactorLiteral literal, string name)
        {
            return new StringLiteralLexerRule(literal.Value, new Tokens.TokenType(name));
        }

        private static ILexerRule GetLiteralLexerRule(EbnfFactorLiteral literal)
        {
            return new StringLiteralLexerRule(literal.Value);
        }

        private ILexerRule GetRegexLexerRule(EbnfFactorRegex factor)
        {
            return GetRegexLexerRule(factor.Regex, factor.Regex.ToString());
        }

        private ILexerRule GetRegexLexerRule(EbnfLexerRuleFactorRegex factor, string name)
        {
            return GetRegexLexerRule(factor.Regex, name);        
        }

        private ILexerRule GetRegexLexerRule(Regex regex, string name)
        {
            var nfa = _thompsonConstructionAlgorithm.Transform(regex);
            var dfa = _subsetConstructionAlgorithm.Transform(nfa);
            return new DfaLexerRule(dfa, name);
        }
    }
}
