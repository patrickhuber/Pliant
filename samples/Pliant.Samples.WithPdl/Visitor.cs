using Pliant.Tree;
using Pliant.Samples.WithPdl.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Samples.WithPdl
{
    public class Visitor : TreeNodeVisitorBase
    {
        public Calculator Calculator { get; private set; }

        public override void Visit(IInternalTreeNode node)
        {
            if (node.Symbol.Value == "Calculator")
                Calculator = VisitCalculatorNode(node);            
        }

        private Calculator VisitCalculatorNode(IInternalTreeNode node)
        {
            Calculator calculator = new();
            if (node.Children.Count == 1)
                calculator.Expression = VisitExpressionNode(node.Children[0] as IInternalTreeNode);
            return calculator;
        }

        private Expression VisitExpressionNode(IInternalTreeNode node)
        {            
            if (node.Children.Count == 3)            
                return VisitExpressionOperatorTerm(node);

            return new Expression
            {
                Term = VisitTerm(node.Children[0] as IInternalTreeNode)
            };            
        }

        private ExpressionOperatorTerm VisitExpressionOperatorTerm(IInternalTreeNode node) 
        { 
            return new ExpressionOperatorTerm 
            {
                Expression = VisitExpressionNode(node.Children[0] as IInternalTreeNode),
                Operator = VisitOperatorNode(node.Children[1] as ITokenTreeNode),
                Term = VisitTerm(node.Children[2] as IInternalTreeNode)
            };
        }

        private Operator VisitOperatorNode(ITokenTreeNode tokenTreeNode)
        {
            var capture = tokenTreeNode.Token.Capture.ToString();
            switch (capture)
            {
                case "+":
                    return Operator.Plus;
                case "-":
                    return Operator.Minus;
                case "*":
                    return Operator.Multiply;
                case "/":
                    return Operator.Divide;
            }
            throw new InvalidOperationException($"Unrecognized token ${capture}");
        }

        private Term VisitTerm(IInternalTreeNode node)
        {
            if (node.Children.Count == 3)
                return VisitTermOperatorFactor(node);
            return new Term
            {
                Factor = VisitFactor(node.Children[0] as IInternalTreeNode)
            };
        }

        private TermOperatorFactor VisitTermOperatorFactor(IInternalTreeNode node)
        {
            return new TermOperatorFactor
            { 
                Term = VisitTerm(node.Children[0] as IInternalTreeNode),
                Operator = VisitOperatorNode(node.Children[1] as ITokenTreeNode),
                Factor = VisitFactor(node.Children[2] as IInternalTreeNode)
            };
        }    

        private Factor VisitFactor(IInternalTreeNode node)
        {
            return new Factor 
            { 
                Number = VisitNumber(node.Children[0] as IInternalTreeNode)
            };
        }

        private uint VisitNumber(IInternalTreeNode node)
        {
            var digits = node.Children[0] as ITokenTreeNode;
            return uint.Parse(digits.Token.Capture.ToString());
        }
    }
}
