using Pliant.Builders;
using Pliant.Grammars;
using System.Collections.Generic;

namespace Pliant.Builders.Expressions
{
    public class RuleExpression : BaseExpression
    {
        public List<List<BaseExpression>> Alterations { get; private set; }

        public RuleExpression()
        {
            Alterations = new List<List<BaseExpression>>();
        }

        public RuleExpression(BaseExpression baseExpression)
            : this()
        {
            AddWithAnd(baseExpression);
        }

        private void AddWithAnd(BaseExpression baseExpression)
        {
            if (Alterations.Count == 0)
                Alterations.Add(new List<BaseExpression>());
            Alterations[Alterations.Count - 1].Add(baseExpression);
        }

        public static implicit operator RuleExpression(ProductionExpression productionExpression)
        {
            return new RuleExpression(productionExpression);
        }

        public static implicit operator RuleExpression(string literal)
        {
            return new RuleExpression(
                new SymbolExpression(
                    new LexerRuleModel(
                        new StringLiteralLexerRule(literal))));
        }

        public static implicit operator RuleExpression(char literal)
        {
            return new RuleExpression(
                new SymbolExpression(
                    new LexerRuleModel(
                        new TerminalLexerRule(literal))));
        }

        public static implicit operator RuleExpression(BaseLexerRule lexerRule)
        {
            return new RuleExpression(
                new SymbolExpression(
                    new LexerRuleModel(
                        lexerRule)));
        }

        public static implicit operator RuleExpression(BaseTerminal baseTerminal)
        {
            return new RuleExpression(
                new SymbolExpression(
                    new LexerRuleModel(
                        new TerminalLexerRule(baseTerminal, baseTerminal.ToString()))));
        }

        public static implicit operator RuleExpression(ProductionReferenceExpression productionReference)
        {
            return new RuleExpression(productionReference);
        }
    }
}