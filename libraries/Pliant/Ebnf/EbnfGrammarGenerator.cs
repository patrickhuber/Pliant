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
        readonly SubsetConstructionAlgorithm _subsetConstructionAlgorithm;
        readonly ThompsonConstructionAlgorithm _thompsonConstructionAlgorithm;

        public EbnfGrammarGenerator()
        {
            _thompsonConstructionAlgorithm = new ThompsonConstructionAlgorithm();
            _subsetConstructionAlgorithm = new SubsetConstructionAlgorithm();
        }

        public IGrammar Generate(EbnfDefinition ebnf)
        {
            var grammarBuilder = new GrammarBuilder();
            Definition(ebnf, grammarBuilder);
            return grammarBuilder.ToGrammar();
        }

        private void Definition(EbnfDefinition definition, GrammarBuilder builder)
        {
            Block(definition.Block, builder);

            if (definition.NodeType != EbnfNodeType.EbnfDefinitionConcatenation)
                return;

            var definitionConcatenation = definition as EbnfDefinitionConcatenation;
            Definition(definitionConcatenation.Definition, builder);                
        }

        void Block(EbnfBlock block, GrammarBuilder builder)
        {
            switch (block.NodeType)
            {
                case EbnfNodeType.EbnfBlockLexerRule:
                    break;

                case EbnfNodeType.EbnfBlockRule:
                    var blockRule = block as EbnfBlockRule;
                    foreach (var production in Rule(blockRule.Rule))
                        builder.AddProduction(production);
                    break;

                case EbnfNodeType.EbnfBlockSetting:
                    break;
            }
        }

        IEnumerable<ProductionBuilder> Rule(EbnfRule rule)
        {
            var nonTerminal = GetNonTerminalFromQualifiedIdentifier(rule.QualifiedIdentifier);
            var productionBuilder = new ProductionBuilder(nonTerminal);
            foreach(var production in Expression(rule.Expression, productionBuilder))
                yield return production;
            yield return productionBuilder;           
        }

        IEnumerable<ProductionBuilder> Expression(EbnfExpression expression, ProductionBuilder currentProduction)
        {
            foreach (var production in Term(expression.Term, currentProduction))
                yield return production;

            if (expression.NodeType != EbnfNodeType.EbnfExpressionAlteration)
                yield break;

            var expressionAlteration = expression as EbnfExpressionAlteration;
            // TODO: Fix this to add OR instead of using lambda
            currentProduction.Definition.Lambda();
            foreach (var production in Expression(expressionAlteration.Expression, currentProduction))
                yield return production;            
        }

        IEnumerable<ProductionBuilder> Grouping(EbnfFactorGrouping grouping, ProductionBuilder currentProduction)
        {
            var name = grouping.ToString();
            var nonTerminal = new NonTerminal(name);
            var groupingProduction = new ProductionBuilder(nonTerminal);

            AddWithAnd(currentProduction, new SymbolBuilder(nonTerminal));

            var expression = grouping.Expression;           
            foreach (var production in Expression(expression, groupingProduction))
                yield return production; 

            yield return groupingProduction;
        }

        IEnumerable<ProductionBuilder> Optional(EbnfFactorOptional optional, ProductionBuilder currentProduction)
        {
            var name = optional.ToString();
            var nonTerminal = new NonTerminal(name);
            var optionalProduction = new ProductionBuilder(nonTerminal);

            AddWithAnd(currentProduction, new SymbolBuilder(nonTerminal));

            var expression = optional.Expression;
            foreach (var production in Expression(expression, optionalProduction))
                yield return production;

            optionalProduction.Definition.Lambda();
            yield return optionalProduction;
        }

        IEnumerable<ProductionBuilder> Repetition(EbnfFactorRepetition repetition, ProductionBuilder currentProduction)
        {
            var name = repetition.ToString();
            var nonTerminal = new NonTerminal(name);
            var repetitionProduction = new ProductionBuilder(nonTerminal);

            AddWithAnd(currentProduction, new SymbolBuilder(nonTerminal));

            var expression = repetition.Expression;
            foreach (var production in Expression(expression, repetitionProduction))
                yield return production;

            AddWithAnd(repetitionProduction, new SymbolBuilder(nonTerminal));
            repetitionProduction.Definition.Lambda();
            yield return repetitionProduction;
        }

        IEnumerable<ProductionBuilder> Term(EbnfTerm term, ProductionBuilder currentProduction)
        {
            foreach (var production in Factor(term.Factor, currentProduction))
                yield return production;

            if (term.NodeType != EbnfNodeType.EbnfTermConcatenation)
                yield break;

            var concatenation = term as EbnfTermConcatenation;
            foreach(var production in Term(concatenation.Term, currentProduction))
                yield return production;                    
        }

        IEnumerable<ProductionBuilder> Factor(EbnfFactor factor, ProductionBuilder currentProduction)
        {
            switch (factor.NodeType)
            {
                case EbnfNodeType.EbnfFactorGrouping:
                    var grouping = factor as EbnfFactorGrouping;
                    return Grouping(grouping, currentProduction);

                case EbnfNodeType.EbnfFactorOptional:
                    var optional = factor as EbnfFactorOptional;
                    return Optional(optional, currentProduction);                    

                case EbnfNodeType.EbnfFactorRepetition:
                    var repetition = factor as EbnfFactorRepetition;
                    return Repetition(repetition, currentProduction);                    

                case EbnfNodeType.EbnfFactorIdentifier:
                    var identifier = factor as EbnfFactorIdentifier;
                    var nonTerminal = GetNonTerminalFromQualifiedIdentifier(identifier.QualifiedIdentifier);                   
                    AddWithAnd(currentProduction, new SymbolBuilder(nonTerminal));
                    break;

                case EbnfNodeType.EbnfFactorLiteral:
                    var literal = factor as EbnfFactorLiteral;
                    var stringLiteralRule = new StringLiteralLexerRule(literal.Value);
                    AddWithAnd(currentProduction, new SymbolBuilder(stringLiteralRule));
                    break;

                case EbnfNodeType.EbnfFactorRegex:
                    var regex = factor as EbnfFactorRegex;
                    var nfa = _thompsonConstructionAlgorithm.Transform(regex.Regex);
                    var dfa = _subsetConstructionAlgorithm.Transform(nfa);
                    var dfaLexerRule = new DfaLexerRule(dfa, regex.Regex.ToString());
                    AddWithAnd(currentProduction, new SymbolBuilder(dfaLexerRule));
                    break;                
            }
            return new ProductionBuilder[] { };
        }

        private static void AddWithAnd(ProductionBuilder currentProduction, SymbolBuilder symbolBuilder)
        {
            if (currentProduction.Definition == null)
                currentProduction.Definition = new RuleBuilder();

            currentProduction.Definition.AddWithAnd(symbolBuilder);
        }
        
        private static NonTerminal GetNonTerminalFromQualifiedIdentifier(EbnfQualifiedIdentifier qualifiedIdentifier)
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
    }
}
