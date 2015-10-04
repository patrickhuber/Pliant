using System;
using Pliant.Nodes;
using Pliant.Grammars;
using System.Collections.Generic;

namespace Pliant.Ebnf
{
    public class EbnfVisitor : INodeVisitor
    {
        public IGrammar Grammar { get; private set; }
        private IList<IProduction> _productions;
        private IList<ILexerRule> _ignoreRules;

        public EbnfVisitor()
        {
            _productions = new List<IProduction>();
            _ignoreRules = new List<ILexerRule>();
        }

        public void Visit(IIntermediateNode node, INodeVisitorStateManager stateManager)
        {
            throw new NotImplementedException();
        }

        public void Visit(ITokenNode tokenNode, INodeVisitorStateManager stateManager)
        {
            throw new NotImplementedException();
        }

        public void Visit(ISymbolNode node, INodeVisitorStateManager stateManager)
        {
            throw new NotImplementedException();            
        }

        public void Visit(ITerminalNode node, INodeVisitorStateManager stateManager)
        {
            throw new NotImplementedException();
        }

        public void Visit(IAndNode andNode, INodeVisitorStateManager stateManager)
        {
            throw new NotImplementedException();
        }
    }
}
