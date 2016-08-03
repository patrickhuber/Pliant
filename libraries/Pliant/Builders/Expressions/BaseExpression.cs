using Pliant.Builders;
using Pliant.Grammars;

namespace Pliant.Builders.Expressions
{
    public class BaseExpression
    {
        public static RuleExpression operator +(BaseExpression lhs, BaseExpression rhs)
        {
            return AddWithAnd(lhs, rhs);
        }

        public static RuleExpression operator +(string lhs, BaseExpression rhs)
        {
            return AddWithAnd(new StringLiteralLexerRule(lhs), rhs);
        }

        public static RuleExpression operator +(BaseExpression lhs, string rhs)
        {
            return AddWithAnd(lhs, new StringLiteralLexerRule(rhs));
        }

        public static RuleExpression operator +(char lhs, BaseExpression rhs)
        {
            return AddWithAnd(new TerminalLexerRule(lhs), rhs);
        }

        public static RuleExpression operator +(BaseExpression lhs, char rhs)
        {
            return AddWithAnd(lhs, new TerminalLexerRule(rhs));
        }

        public static RuleExpression operator +(BaseExpression lhs, BaseLexerRule rhs)
        {
            return AddWithAnd(lhs, rhs);
        }

        public static RuleExpression operator +(BaseLexerRule lhs, BaseExpression rhs)
        {
            return AddWithAnd(lhs, rhs);
        }

        public static RuleExpression operator +(BaseTerminal lhs, BaseExpression rhs)
        {
            return AddWithAnd(lhs, rhs);
        }
        
        public static RuleExpression operator +(BaseExpression lhs, BaseTerminal rhs)
        {
            return AddWithAnd(lhs, rhs);
        }

        public static RuleExpression operator |(BaseExpression lhs, BaseExpression rhs)
        {
            return AddWithOr(lhs, rhs);
        }

        public static RuleExpression operator |(string lhs, BaseExpression rhs)
        {
            return AddWithOr(new StringLiteralLexerRule(lhs), rhs);
        }

        public static RuleExpression operator |(BaseExpression lhs, string rhs)
        {
            return AddWithOr(lhs, new StringLiteralLexerRule(rhs));
        }

        public static RuleExpression operator |(char lhs, BaseExpression rhs)
        {
            return AddWithOr(new TerminalLexerRule(lhs), rhs);
        }

        public static RuleExpression operator |(BaseExpression lhs, char rhs)
        {
            return AddWithOr(lhs, new TerminalLexerRule(rhs));
        }

        public static RuleExpression operator |(BaseExpression lhs, BaseLexerRule rhs)
        {
            return AddWithOr(lhs, rhs);
        }

        public static RuleExpression operator |(BaseLexerRule lhs, BaseExpression rhs)
        {
            return AddWithOr(lhs, rhs);
        }
        
        public static RuleExpression operator |(BaseTerminal lhs, BaseExpression rhs)
        {
            return AddWithOr(lhs, rhs);
        }

        public static RuleExpression operator |(BaseExpression lhs, BaseTerminal rhs)
        {
            return AddWithOr(lhs, rhs);
        }

        private static RuleExpression AddWithAnd(BaseLexerRule lhs, BaseExpression rhs)
        {
            return AddWithAnd(new SymbolExpression(new LexerRuleModel(lhs)), rhs);
        }

        private static RuleExpression AddWithAnd(BaseExpression lhs, BaseLexerRule rhs)
        {
            return AddWithAnd(lhs, new SymbolExpression(new LexerRuleModel(rhs)));
        }

        private static RuleExpression AddWithAnd(BaseTerminal lhs, BaseExpression rhs)
        {
            return AddWithAnd(
                new SymbolExpression(
                    new LexerRuleModel(
                        new TerminalLexerRule(lhs, lhs.ToString()))), 
                rhs);
        }

        private static RuleExpression AddWithAnd(BaseExpression lhs, BaseTerminal rhs)
        {
            return AddWithAnd(
                lhs, 
                new SymbolExpression(
                    new LexerRuleModel(
                        new TerminalLexerRule(rhs, rhs.ToString()))));
        }

        private static RuleExpression AddWithAnd(BaseExpression lhs, BaseExpression rhs)
        {
            var expression = lhs as RuleExpression ?? new RuleExpression(lhs);
            expression.Alterations[expression.Alterations.Count - 1].Add(rhs);
            return expression;
        }
        
        private static RuleExpression AddWithOr(BaseLexerRule lhs, BaseExpression rhs)
        {
            return AddWithOr(new SymbolExpression(new LexerRuleModel(lhs)), rhs);
        }

        private static RuleExpression AddWithOr(BaseExpression lhs, BaseLexerRule rhs)
        {
            return AddWithOr(lhs, new SymbolExpression(new LexerRuleModel(rhs)));
        }

        private static RuleExpression AddWithOr(BaseTerminal lhs, BaseExpression rhs)
        {
            return AddWithOr(
                new SymbolExpression(
                    new LexerRuleModel(
                        new TerminalLexerRule(lhs, lhs.ToString()))),
                rhs);
        }

        private static RuleExpression AddWithOr(BaseExpression lhs, BaseTerminal rhs)
        {
            return AddWithOr(
                lhs,
                new SymbolExpression(
                    new LexerRuleModel(
                        new TerminalLexerRule(rhs, rhs.ToString()))));
        }

        private static RuleExpression AddWithOr(BaseExpression lhs, BaseExpression rhs)
        {
            var lhsExpression = (lhs as RuleExpression) ?? new RuleExpression(lhs);
            var rhsExpression = (rhs as RuleExpression) ?? new RuleExpression(rhs);
            foreach (var symbolList in rhsExpression.Alterations)
                lhsExpression.Alterations.Add(symbolList);
            return lhsExpression;
        }
    }
}
