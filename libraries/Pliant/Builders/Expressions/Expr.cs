using Pliant.Builders;
using Pliant.Grammars;

namespace Pliant.Builders.Expressions
{
    public class Expr : RuleExpression
    {
        public Expr(BaseExpression baseExpression) 
            : base(baseExpression)
        {
        }

        public static explicit operator Expr(string value)
        {
            return new Expr(
                new SymbolExpression(
                    new LexerRuleModel(
                        new StringLiteralLexerRule(value))));
        }

        public static explicit operator Expr(char value)
        {

            return new Expr(
                new SymbolExpression(
                    new LexerRuleModel(
                        new TerminalLexerRule(value))));
        }

        public static explicit operator Expr(BaseLexerRule value)
        {
            return new Expr(
                   new SymbolExpression(
                       new LexerRuleModel(
                           value)));
        }

        public static explicit operator Expr(BaseTerminal value)
        {
            return new Expr(
                new SymbolExpression(
                    new LexerRuleModel(
                        new TerminalLexerRule(value, value.ToString()))));
        }

        public static explicit operator Expr(ProductionExpression productionExpression)
        {
            return new Expr(productionExpression);
        }
    }
}
