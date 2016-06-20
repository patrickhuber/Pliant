using Pliant.Automata;
using Pliant.Builders;
using Pliant.Grammars;
using Pliant.RegularExpressions;
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
            var grammarModel = new GrammarModel();
            Definition(ebnf, grammarModel);
            return grammarModel.ToGrammar();
        }

        private void Definition(EbnfDefinition definition, GrammarModel grammarModel)
        {
            Block(definition.Block, grammarModel);

            if (definition.NodeType != EbnfNodeType.EbnfDefinitionConcatenation)
                return;

            var definitionConcatenation = definition as EbnfDefinitionConcatenation;
            Definition(definitionConcatenation.Definition, grammarModel);                
        }

        void Block(EbnfBlock block, GrammarModel grammarModel)
        {
            switch (block.NodeType)
            {
                case EbnfNodeType.EbnfBlockLexerRule:
                    break;

                case EbnfNodeType.EbnfBlockRule:
                    var blockRule = block as EbnfBlockRule;
                    foreach (var production in Rule(blockRule.Rule))
                        grammarModel.Productions.Add(production);
                    break;

                case EbnfNodeType.EbnfBlockSetting:
                    break;
            }
        }

        IEnumerable<ProductionModel> Rule(EbnfRule rule)
        {
            var nonTerminal = GetNonTerminalFromQualifiedIdentifier(rule.QualifiedIdentifier);
            var productionModel = new ProductionModel(nonTerminal);
            foreach(var production in Expression(rule.Expression, productionModel))
                yield return production;
            yield return productionModel;           
        }

        IEnumerable<ProductionModel> Expression(EbnfExpression expression, ProductionModel currentProduction)
        {
            foreach (var production in Term(expression.Term, currentProduction))
                yield return production;

            if (expression.NodeType != EbnfNodeType.EbnfExpressionAlteration)
                yield break;

            var expressionAlteration = expression as EbnfExpressionAlteration;
            currentProduction.Lambda();

            foreach (var production in Expression(expressionAlteration.Expression, currentProduction))
                yield return production;            
        }

        IEnumerable<ProductionModel> Grouping(EbnfFactorGrouping grouping, ProductionModel currentProduction)
        {
            var name = grouping.ToString();
            var nonTerminal = new NonTerminal(name);
            var groupingProduction = new ProductionModel(nonTerminal);

            currentProduction.AddWithAnd(new NonTerminalModel(nonTerminal));

            var expression = grouping.Expression;           
            foreach (var production in Expression(expression, groupingProduction))
                yield return production; 

            yield return groupingProduction;
        }

        IEnumerable<ProductionModel> Optional(EbnfFactorOptional optional, ProductionModel currentProduction)
        {
            var name = optional.ToString();
            var nonTerminal = new NonTerminal(name);
            var optionalProduction = new ProductionModel(nonTerminal);

            currentProduction.AddWithAnd(new NonTerminalModel(nonTerminal));

            var expression = optional.Expression;
            foreach (var production in Expression(expression, optionalProduction))
                yield return production;

            optionalProduction.Lambda();
            yield return optionalProduction;
        }

        IEnumerable<ProductionModel> Repetition(EbnfFactorRepetition repetition, ProductionModel currentProduction)
        {
            var name = repetition.ToString();
            var nonTerminal = new NonTerminal(name);
            var repetitionProduction = new ProductionModel(nonTerminal);

            currentProduction.AddWithAnd(new NonTerminalModel(nonTerminal));

            var expression = repetition.Expression;
            foreach (var production in Expression(expression, repetitionProduction))
                yield return production;

            repetitionProduction.AddWithAnd(new NonTerminalModel(nonTerminal));
            repetitionProduction.Lambda();

            yield return repetitionProduction;
        }

        IEnumerable<ProductionModel> Term(EbnfTerm term, ProductionModel currentProduction)
        {
            foreach (var production in Factor(term.Factor, currentProduction))
                yield return production;

            if (term.NodeType != EbnfNodeType.EbnfTermConcatenation)
                yield break;

            var concatenation = term as EbnfTermConcatenation;
            foreach(var production in Term(concatenation.Term, currentProduction))
                yield return production;                    
        }

        IEnumerable<ProductionModel> Factor(EbnfFactor factor, ProductionModel currentProduction)
        {
            switch (factor.NodeType)
            {
                case EbnfNodeType.EbnfFactorGrouping:
                    var grouping = factor as EbnfFactorGrouping;
                    foreach (var production in Grouping(grouping, currentProduction))
                        yield return production;
                    break;

                case EbnfNodeType.EbnfFactorOptional:
                    var optional = factor as EbnfFactorOptional;
                    foreach (var production in Optional(optional, currentProduction))
                        yield return production;
                    break;

                case EbnfNodeType.EbnfFactorRepetition:
                    var repetition = factor as EbnfFactorRepetition;
                    foreach (var production in Repetition(repetition, currentProduction))
                        yield return production;
                    break;

                case EbnfNodeType.EbnfFactorIdentifier:
                    var identifier = factor as EbnfFactorIdentifier;
                    var nonTerminal = GetNonTerminalFromQualifiedIdentifier(identifier.QualifiedIdentifier);                   
                    currentProduction.AddWithAnd(new NonTerminalModel(nonTerminal));
                    break;

                case EbnfNodeType.EbnfFactorLiteral:
                    var literal = factor as EbnfFactorLiteral;
                    var stringLiteralRule = new StringLiteralLexerRule(literal.Value);
                    currentProduction.AddWithAnd( new LexerRuleModel(stringLiteralRule));
                    break;

                case EbnfNodeType.EbnfFactorRegex:
                    var regex = factor as EbnfFactorRegex;
                    var nfa = _thompsonConstructionAlgorithm.Transform(regex.Regex);
                    var dfa = _subsetConstructionAlgorithm.Transform(nfa);
                    var dfaLexerRule = new DfaLexerRule(dfa, regex.Regex.ToString());
                    currentProduction.AddWithAnd(new LexerRuleModel(dfaLexerRule));
                    break;                
            }            
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
