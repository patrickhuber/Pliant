namespace Pliant.Builders.Expressions
{
    public class BaseExpression
    {
        public static RuleExpression operator &(BaseExpression lhs, BaseExpression rhs)
        {
            return new RuleExpression();
        }

        public static RuleExpression operator &(string lhs, BaseExpression rhs)
        {
            return new RuleExpression();
        }

        public static RuleExpression operator &(BaseExpression lhs, string rhs)
        {
            return new RuleExpression();
        }

        public static RuleExpression operator &(char lhs, BaseExpression rhs)
        {
            return new RuleExpression();
        }

        public static RuleExpression operator &(BaseExpression lhs, char rhs)
        {
            return new RuleExpression();
        }

        public static RuleExpression operator |(BaseExpression lhs, BaseExpression rhs)
        {
            return new RuleExpression();
        }

        public static RuleExpression operator |(string lhs, BaseExpression rhs)
        {
            return new RuleExpression();
        }

        public static RuleExpression operator |(BaseExpression lhs, string rhs)
        {
            return new RuleExpression();
        }

        public static RuleExpression operator |(char lhs, BaseExpression rhs)
        {
            return new RuleExpression();
        }

        public static RuleExpression operator |(BaseExpression lhs, char rhs)
        {
            return new RuleExpression();
        }
    }
}
