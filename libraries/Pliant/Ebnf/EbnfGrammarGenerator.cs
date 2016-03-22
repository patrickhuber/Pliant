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

        private ILexerRule GetRegexLexerRule(EbnfFactorRegex factor)
        {
            var nfa = _thompsonConstructionAlgorithm.Transform(factor.Regex);
            var dfa = _subsetConstructionAlgorithm.Transform(nfa);
            return new DfaLexerRule(dfa, factor.Regex.ToString());
        }
    }
}
