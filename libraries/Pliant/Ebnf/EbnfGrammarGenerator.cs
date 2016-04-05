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
        readonly SubsetConstructionAlgorithm _subsetConstructionAlgorithm;
        readonly ThompsonConstructionAlgorithm _thompsonConstructionAlgorithm;

        public EbnfGrammarGenerator(IEbnfProductionNamingStrategy strategy)
        {
            Assert.IsNotNull(strategy, nameof(strategy));
            _strategy = strategy;
            _thompsonConstructionAlgorithm = new ThompsonConstructionAlgorithm();
            _subsetConstructionAlgorithm = new SubsetConstructionAlgorithm();
        }

        public IGrammar Generate(EbnfDefinition ebnf)
        {
            var grammarBuilder = new GrammarBuilder();
            foreach (var production in CreateProductions(ebnf))
                grammarBuilder.AddProduction(production);
            return grammarBuilder.ToGrammar();
        }

        private IEnumerable<IProduction> CreateProductions(EbnfDefinition definition)
        {
            var productionList = new List<IProduction>();
            productionList.AddRange(CreateProductions(definition.Block));
            switch (definition.NodeType)
            {
                case EbnfNodeType.EbnfDefinitionConcatenation:
                    var definitionConcatenation = definition as EbnfDefinitionConcatenation;
                    productionList.AddRange(CreateProductions(definitionConcatenation.Definition));
                    break;
            }
            return productionList;
        }

        private IEnumerable<IProduction> CreateProductions(EbnfBlock block)
        {
            switch (block.NodeType)
            {
                case EbnfNodeType.EbnfBlockLexerRule:
                    break;

                case EbnfNodeType.EbnfBlockRule:
                    return CreateProductions(block as EbnfBlockRule);

                case EbnfNodeType.EbnfBlockSetting:
                    break;
            }
            throw new InvalidOperationException("Unrecognized block rule type.");
        }

        private IEnumerable<IProduction> CreateProductions(EbnfBlockRule blockRule)
        {
            var rule = blockRule.Rule;
            var leftHandSide = GetNonTerminalFromQualifiedIdentifier(rule.QualifiedIdentifier);            
            var rightHandSides = GetRightHandSideForExpression(rule.Expression);
            foreach (var rightHandSide in rightHandSides)
                yield return new Production(leftHandSide, rightHandSide);
        }

        private IList<IList<ISymbol>> GetRightHandSideForExpression(EbnfExpression expression)
        {
            var rightHandSides = new List<IList<ISymbol>>();
            var ruleBuilder = new RuleBuilder();

            var currentExpression = expression;

            while (currentExpression != null)
            {
                rightHandSides.Add(new List<ISymbol>());
                SetRightHandSideForTerm(rightHandSides,currentExpression.Term);
                currentExpression = (EbnfNodeType.EbnfExpressionAlteration == currentExpression.NodeType)
                    ? (currentExpression as EbnfExpressionAlteration).Expression
                    : null;
            }
            return rightHandSides;
        }

        private void SetRightHandSideForTerm(IList<IList<ISymbol>> rightHandSides, EbnfTerm term)
        {
            var currentTerm = term;
            while (currentTerm != null)
            {
                SetRightHandSideForFactor(rightHandSides, currentTerm.Factor);
                currentTerm = EbnfNodeType.EbnfTermConcatenation == currentTerm.NodeType 
                    ? (currentTerm as EbnfTermConcatenation).Term
                    : null;
            }
        }

        private void SetRightHandSideForFactor(IList<IList<ISymbol>> rightHandSides, EbnfFactor factor)
        {
            switch (factor.NodeType)
            {
                case EbnfNodeType.EbnfFactorIdentifier:
                    var factorIdentifider = factor as EbnfFactorIdentifier;
                    var symbol = GetNonTerminalFromQualifiedIdentifier(factorIdentifider.QualifiedIdentifier);
                    AppendSymbolToAllRules(rightHandSides, symbol);
                    break;

                case EbnfNodeType.EbnfFactorGrouping:
                    throw new NotImplementedException("Grouping is not implemented.");                    

                case EbnfNodeType.EbnfFactorOptional:
                    // duplicate every existing right hand side
                    var alternateRightHandSides = new List<List<ISymbol>>();
                    var factorOptional = factor as EbnfFactorOptional;
                    
                    foreach (var rightHandSide in rightHandSides)
                        alternateRightHandSides.Add(new List<ISymbol>(rightHandSide));
                    
                    break;

                case EbnfNodeType.EbnfFactorRepetition:
                    throw new NotImplementedException("Repetition is not implemented.");

                case EbnfNodeType.EbnfFactorLiteral:
                    var factorLiteral = factor as EbnfFactorLiteral;
                    var literalLexerRule = GetLiteralLexerRule(factorLiteral);
                    AppendSymbolToAllRules(rightHandSides, literalLexerRule);
                    break;

                case EbnfNodeType.EbnfFactorRegex:
                    var factorRegex = factor as EbnfFactorRegex;
                    var regexLexerRule = GetRegexLexerRule(factorRegex);
                    AppendSymbolToAllRules(rightHandSides, regexLexerRule);
                    break;          
            }
        }

        private static void AppendSymbolToAllRules(IList<IList<ISymbol>> rightHandSides, ISymbol symbol)
        {
            foreach (var rightHandSide in rightHandSides)
                rightHandSide.Add(symbol);
        }
        
        private static INonTerminal GetNonTerminalFromQualifiedIdentifier(EbnfQualifiedIdentifier qualifiedIdentifier)
        {
            var @namespace = new StringBuilder();
            var currentQualifiedIdentifier = qualifiedIdentifier;
            var index = 0;
            while (currentQualifiedIdentifier.NodeType == EbnfNodeType.EbnfQualifiedIdentifierConcatenation)
            {
                if (index > 0)
                    @namespace.Append(".");
                @namespace.Append(currentQualifiedIdentifier.Identifier);
                currentQualifiedIdentifier = (currentQualifiedIdentifier as EbnfQualifiedIdentifierConcatenation).QualifiedIdentifier;
                index++;
            }
            return new NonTerminal(@namespace.ToString(), currentQualifiedIdentifier.Identifier);
        }

        private static ILexerRule GetLexerRule(EbnfLexerRule lexerRule)
        {
            var symbol = GetNonTerminalFromQualifiedIdentifier(lexerRule.QualifiedIdentifier);
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
