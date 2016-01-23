using Pliant.Grammars;

namespace Pliant.Builders
{
    public abstract class BaseBuilder
    {
        public static RuleBuilder operator +(BaseBuilder lhs, BaseBuilder rhs)
        {
            return AddWithAnd(lhs, rhs);
        }

        public static RuleBuilder operator +(BaseBuilder lhs, char rhs)
        {
            return AddWithAnd(lhs, new SymbolBuilder(new TerminalLexerRule(rhs)));
        }

        public static RuleBuilder operator +(char lhs, BaseBuilder rhs)
        {
            return AddWithAnd(new SymbolBuilder(new TerminalLexerRule(lhs)), rhs);
        }

        public static RuleBuilder operator +(BaseBuilder lhs, string rhs)
        {
            return AddWithAnd(lhs, new SymbolBuilder(new StringLiteralLexerRule(rhs)));
        }

        public static RuleBuilder operator +(string lhs, BaseBuilder rhs)
        {
            return AddWithAnd(new SymbolBuilder(new StringLiteralLexerRule(lhs)), rhs);
        }

        public static RuleBuilder operator +(BaseBuilder lhs, BaseTerminal rhs)
        {
            return AddWithAnd(lhs, new SymbolBuilder(new TerminalLexerRule(rhs, rhs.ToString())));
        }

        public static RuleBuilder operator +(BaseTerminal lhs, BaseBuilder rhs)
        {
            return AddWithAnd(new SymbolBuilder(new TerminalLexerRule(lhs, lhs.ToString())), rhs);
        }

        public static RuleBuilder operator +(BaseBuilder lhs, BaseLexerRule rhs)
        {
            return AddWithAnd(lhs, new SymbolBuilder(rhs));
        }

        public static RuleBuilder operator +(BaseLexerRule lhs, BaseBuilder rhs)
        {
            return AddWithAnd(new SymbolBuilder(lhs), rhs);
        }

        public static RuleBuilder operator |(BaseBuilder lhs, BaseBuilder rhs)
        {
            return AddWithOr(lhs, rhs);
        }

        public static RuleBuilder operator |(BaseBuilder lhs, char rhs)
        {
            return AddWithOr(lhs, new SymbolBuilder(new TerminalLexerRule(rhs)));
        }

        public static RuleBuilder operator |(char lhs, BaseBuilder rhs)
        {
            return AddWithOr(new SymbolBuilder(new TerminalLexerRule(lhs)), rhs);
        }

        public static RuleBuilder operator |(BaseBuilder lhs, string rhs)
        {
            return AddWithOr(lhs, new SymbolBuilder(new StringLiteralLexerRule(rhs)));
        }

        public static RuleBuilder operator |(string lhs, BaseBuilder rhs)
        {
            return AddWithOr(new SymbolBuilder(new StringLiteralLexerRule(lhs)), rhs);
        }

        public static RuleBuilder operator |(BaseBuilder lhs, BaseTerminal rhs)
        {
            return AddWithOr(lhs, new SymbolBuilder(new TerminalLexerRule(rhs, rhs.ToString())));
        }

        public static RuleBuilder operator |(BaseTerminal lhs, BaseBuilder rhs)
        {
            return AddWithOr(new SymbolBuilder(new TerminalLexerRule(lhs, lhs.ToString())), rhs);
        }

        public static RuleBuilder operator |(BaseBuilder lhs, BaseLexerRule rhs)
        {
            return AddWithOr(lhs, new SymbolBuilder(rhs));
        }

        public static RuleBuilder operator |(BaseLexerRule lhs, BaseBuilder rhs)
        {
            return AddWithOr(new SymbolBuilder(lhs), rhs);
        }

        private static RuleBuilder AddWithAnd(BaseBuilder lhs, BaseBuilder rhs)
        {
            var expression = lhs as RuleBuilder ?? new RuleBuilder(lhs);
            expression.Data[expression.Data.Count - 1].Add(rhs);
            return expression;
        }

        private static RuleBuilder AddWithOr(BaseBuilder lhs, BaseBuilder rhs)
        {
            var lhsExpression = (lhs as RuleBuilder) ?? new RuleBuilder(lhs);
            var rhsExpression = (rhs as RuleBuilder) ?? new RuleBuilder(rhs);
            lhsExpression.Data.AddRange(rhsExpression.Data);
            return lhsExpression;
        }
    }
}