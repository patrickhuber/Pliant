using Pliant.Automata;
using Pliant.Builders;
using Pliant.Grammars;
using System.Collections.Generic;
using System.Text;
using System;
using Pliant.Languages.Regex;

namespace Pliant.Languages.Pdl
{
    public class PdlGrammarGenerator
    {
        readonly INfaToDfa _nfaToDfaAlgorithm;
        readonly IRegexToNfa _regexToNfaAlgorithm;

        public PdlGrammarGenerator()
        {
            _regexToNfaAlgorithm = new ThompsonConstructionAlgorithm();
            _nfaToDfaAlgorithm = new SubsetConstructionAlgorithm();
        }

        public IGrammar Generate(PdlDefinition ebnf)
        {
            var grammarModel = new GrammarModel();
            Definition(ebnf, grammarModel);
            return grammarModel.ToGrammar();
        }

        private void Definition(PdlDefinition definition, GrammarModel grammarModel)
        {
            Block(definition.Block, grammarModel);

            if (definition.NodeType != PdlNodeType.PdlDefinitionConcatenation)
                return;

            var definitionConcatenation = definition as PdlDefinitionConcatenation;
            Definition(definitionConcatenation.Definition, grammarModel);                
        }

        void Block(PdlBlock block, GrammarModel grammarModel)
        {
            switch (block.NodeType)
            {
                case PdlNodeType.PdlBlockLexerRule:
                    var blockLexerRule = block as PdlBlockLexerRule;
                    grammarModel.LexerRules.Add(LexerRule(blockLexerRule));
                    break;

                case PdlNodeType.PdlBlockRule:
                    var blockRule = block as PdlBlockRule;
                    foreach (var production in Rule(blockRule.Rule))
                        grammarModel.Productions.Add(production);
                    break;

                case PdlNodeType.PdlBlockSetting:
                    var blockSetting = block as PdlBlockSetting;

                    switch (blockSetting.Setting.SettingIdentifier.Value.ToString())
                    {
                        case StartProductionSettingModel.SettingKey:
                            grammarModel.StartSetting = StartSetting(blockSetting);
                            break;

                        case IgnoreSettingModel.SettingKey:
                            var ignoreSettings = IgnoreSettings(blockSetting);
                            for (var i = 0; i < ignoreSettings.Count; i++)
                                grammarModel.IgnoreSettings.Add(ignoreSettings[i]);
                            break;

                        case TriviaSettingModel.SettingKey:
                            var triviaSettings = TriviaSettings(blockSetting);
                            for (var i = 0; i < triviaSettings.Count; i++)
                                grammarModel.TriviaSettings.Add(triviaSettings[i]);
                            break;
                    }
                    break;
            }
        }

        private LexerRuleModel LexerRule(PdlBlockLexerRule blockLexerRule)
        {
            var ebnfLexerRule = blockLexerRule.LexerRule;

            var fullyQualifiedName = GetFullyQualifiedNameFromQualifiedIdentifier(
                ebnfLexerRule.QualifiedIdentifier);

            var lexerRule = LexerRuleExpression(
                fullyQualifiedName,
                ebnfLexerRule.Expression);

            return new LexerRuleModel(lexerRule);
        }

        private ILexerRule LexerRuleExpression(
            FullyQualifiedName fullyQualifiedName, 
            PdlLexerRuleExpression ebnfLexerRule)
        {
            if (TryRecognizeSimpleLiteralExpression(fullyQualifiedName, ebnfLexerRule, out ILexerRule lexerRule))
                return lexerRule;

            var nfa = LexerRuleExpression(ebnfLexerRule);
            var dfa = _nfaToDfaAlgorithm.Transform(nfa);

            return new DfaLexerRule(dfa, fullyQualifiedName.FullName);
        }

        private bool TryRecognizeSimpleLiteralExpression(
            FullyQualifiedName fullyQualifiedName,
            PdlLexerRuleExpression ebnfLexerRule, 
            out ILexerRule lexerRule)
        {
            lexerRule = null;

            if (ebnfLexerRule.NodeType != PdlNodeType.PdlLexerRuleExpression)
                return false;

            var term = ebnfLexerRule.Term;
            if (term.NodeType != PdlNodeType.PdlLexerRuleTerm)
                return false;

            var factor = term.Factor;
            if (factor.NodeType != PdlNodeType.PdlLexerRuleFactorLiteral)
                return false;

            var literal = factor as PdlLexerRuleFactorLiteral;
            lexerRule = new StringLiteralLexerRule(
                literal.Value.ToString(), 
                new TokenType(fullyQualifiedName.FullName));

            return true;
        }        

        INfa LexerRuleExpression(PdlLexerRuleExpression expression)
        {
            var nfa = LexerRuleTerm(expression.Term);
            if (expression.NodeType == PdlNodeType.PdlLexerRuleExpressionAlteration)
            {
                var alteration = expression as PdlLexerRuleExpressionAlteration;
                var alterationNfa = LexerRuleExpression(alteration);
                nfa = nfa.Union(alterationNfa);
            }
            return nfa;
        }

        INfa LexerRuleTerm(PdlLexerRuleTerm term)
        {
            var nfa = LexerRuleFactor(term.Factor);
            if (term.NodeType == PdlNodeType.PdlLexerRuleTermConcatenation)
            {
                var concatenation = term as PdlLexerRuleTermConcatenation;
                var concatNfa = LexerRuleTerm(concatenation.Term);
                nfa = nfa.Concatenation(concatNfa);
            }
            return nfa;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0601:Value type to reference type conversion causing boxing allocation", Justification = "ToString is not called in the critical section of code")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0502:Explicit new reference type allocation", Justification = "Exceptions are not part of control flow")]
        INfa LexerRuleFactor(PdlLexerRuleFactor factor)
        {
            switch (factor.NodeType)
            {
                case PdlNodeType.PdlLexerRuleFactorLiteral:
                    return LexerRuleFactorLiteral(factor as PdlLexerRuleFactorLiteral);                    

                case PdlNodeType.PdlLexerRuleFactorRegex:
                    return LexerRuleFactorRegex(factor as PdlLexerRuleFactorRegex);


                default:
                    throw new InvalidOperationException(
                        $"Invalid PdlLexerRuleFactor node type detected. Found {Enum.GetName(typeof(PdlNodeType), factor.NodeType)}, expected PdlLexerRuleFactorLiteral or PdlLexerRuleFactorRegex");
            }
        }

        private INfa LexerRuleFactorLiteral(PdlLexerRuleFactorLiteral ebnfLexerRuleFactorLiteral)
        {
            var literal = ebnfLexerRuleFactorLiteral.Value;
            var states = new NfaState[literal.Count + 1];
            for (var i = 0; i < states.Length; i++)
            {
                var current = new NfaState();
                states[i] = current;

                if (i == 0)
                    continue;

                var previous = states[i - 1];
                previous.AddTransistion(
                    new TerminalNfaTransition(
                        new CharacterTerminal(literal[i - 1]), current));
            }
            return new Nfa(states[0], states[states.Length - 1]);
        }
        
        private INfa LexerRuleFactorRegex(PdlLexerRuleFactorRegex ebnfLexerRuleFactorRegex)
        {
            var regex = ebnfLexerRuleFactorRegex.Regex;
            return _regexToNfaAlgorithm.Transform(regex);
        }

        private StartProductionSettingModel StartSetting(PdlBlockSetting blockSetting)
        {
            var productionName =  GetFullyQualifiedNameFromQualifiedIdentifier(
                blockSetting.Setting.QualifiedIdentifier);
            return new StartProductionSettingModel(productionName);
        }

        private IReadOnlyList<TriviaSettingModel> TriviaSettings(PdlBlockSetting blockSetting)
        {
            var fullyQualifiedName = GetFullyQualifiedNameFromQualifiedIdentifier(blockSetting.Setting.QualifiedIdentifier);
            var triviaSettingModel = new TriviaSettingModel(fullyQualifiedName);
            return new[] { triviaSettingModel};
        }

        private IReadOnlyList<IgnoreSettingModel> IgnoreSettings(PdlBlockSetting blockSetting)
        {
            var fullyQualifiedName = GetFullyQualifiedNameFromQualifiedIdentifier(blockSetting.Setting.QualifiedIdentifier);
            var ignoreSettingModel = new IgnoreSettingModel(fullyQualifiedName);
            return new[] { ignoreSettingModel };
        }

        IEnumerable<ProductionModel> Rule(PdlRule rule)
        {
            var nonTerminal = GetFullyQualifiedNameFromQualifiedIdentifier(rule.QualifiedIdentifier);
            var productionModel = new ProductionModel(nonTerminal);
            foreach(var production in Expression(rule.Expression, productionModel))
                yield return production;
            yield return productionModel;           
        }

        IEnumerable<ProductionModel> Expression(PdlExpression expression, ProductionModel currentProduction)
        {
            foreach (var production in Term(expression.Term, currentProduction))
                yield return production;

            if (expression.NodeType != PdlNodeType.PdlExpressionAlteration)
                yield break;

            var expressionAlteration = expression as PdlExpressionAlteration;
            currentProduction.Lambda();

            foreach (var production in Expression(expressionAlteration.Expression, currentProduction))
                yield return production;            
        }

        IEnumerable<ProductionModel> Grouping(PdlFactorGrouping grouping, ProductionModel currentProduction)
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

        IEnumerable<ProductionModel> Optional(PdlFactorOptional optional, ProductionModel currentProduction)
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

        IEnumerable<ProductionModel> Repetition(PdlFactorRepetition repetition, ProductionModel currentProduction)
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

        IEnumerable<ProductionModel> Term(PdlTerm term, ProductionModel currentProduction)
        {
            foreach (var production in Factor(term.Factor, currentProduction))
                yield return production;

            if (term.NodeType != PdlNodeType.PdlTermConcatenation)
                yield break;

            var concatenation = term as PdlTermConcatenation;
            foreach(var production in Term(concatenation.Term, currentProduction))
                yield return production;                    
        }

        IEnumerable<ProductionModel> Factor(PdlFactor factor, ProductionModel currentProduction)
        {
            switch (factor.NodeType)
            {
                case PdlNodeType.PdlFactorGrouping:
                    var grouping = factor as PdlFactorGrouping;
                    foreach (var production in Grouping(grouping, currentProduction))
                        yield return production;
                    break;

                case PdlNodeType.PdlFactorOptional:
                    var optional = factor as PdlFactorOptional;
                    foreach (var production in Optional(optional, currentProduction))
                        yield return production;
                    break;

                case PdlNodeType.PdlFactorRepetition:
                    var repetition = factor as PdlFactorRepetition;
                    foreach (var production in Repetition(repetition, currentProduction))
                        yield return production;
                    break;

                case PdlNodeType.PdlFactorIdentifier:
                    var identifier = factor as PdlFactorIdentifier;
                    var nonTerminal = GetFullyQualifiedNameFromQualifiedIdentifier(identifier.QualifiedIdentifier);                   
                    currentProduction.AddWithAnd(new NonTerminalModel(nonTerminal));
                    break;

                case PdlNodeType.PdlFactorLiteral:
                    var literal = factor as PdlFactorLiteral;
                    var stringLiteralRule = new StringLiteralLexerRule(literal.Value.ToString());
                    currentProduction.AddWithAnd( new LexerRuleModel(stringLiteralRule));
                    break;

                case PdlNodeType.PdlFactorRegex:
                    var regex = factor as PdlFactorRegex;
                    var nfa = _regexToNfaAlgorithm.Transform(regex.Regex);
                    var dfa = _nfaToDfaAlgorithm.Transform(nfa);
                    var dfaLexerRule = new DfaLexerRule(dfa, regex.Regex.ToString());
                    currentProduction.AddWithAnd(new LexerRuleModel(dfaLexerRule));
                    break;                
            }            
        }
                        
        private static FullyQualifiedName GetFullyQualifiedNameFromQualifiedIdentifier(PdlQualifiedIdentifier qualifiedIdentifier)
        {
            var @namespace = new StringBuilder();
            var currentQualifiedIdentifier = qualifiedIdentifier;
            var index = 0;
            while (currentQualifiedIdentifier.NodeType == PdlNodeType.PdlQualifiedIdentifierConcatenation)
            {
                if (index > 0)
                    @namespace.Append(".");
                @namespace.Append(currentQualifiedIdentifier.Identifier);
                currentQualifiedIdentifier = (currentQualifiedIdentifier as PdlQualifiedIdentifierConcatenation).QualifiedIdentifier;
                index++;
            }
            return new FullyQualifiedName(@namespace.ToString(), currentQualifiedIdentifier.Identifier.ToString());
        }   
    }
}
