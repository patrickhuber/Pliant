using Pliant.Charts;
using Pliant.Grammars;
using System.Collections.Generic;
using System.Linq;

namespace Pliant.Ast
{
    public class NodeSet
    {
        private IList<ISymbolNode> _symbolNodes;
        private IList<IIntermediateNode> _intermediateNodes;

        public NodeSet()
        {
            _symbolNodes = new List<ISymbolNode>();
            _intermediateNodes = new List<IIntermediateNode>();
        }

        public ISymbolNode AddOrGetExistingSymbolNode(ISymbol symbol, int origin, int location)
        {
            // PERF: Avoid Linq FirstOrDefault due to lambda allocation
            ISymbolNode symbolNode = null;
            foreach (var node in _symbolNodes)
                if (node.Origin == origin
                    && node.Location == location
                    && node.Symbol.Equals(symbol))
                { 
                    symbolNode = node;
                    break;
                }
            
            if (symbolNode == null)
            {
                symbolNode = new SymbolNode(symbol, origin, location);
                _symbolNodes.Add(symbolNode);
            }
            return symbolNode;
        }
        
        public IIntermediateNode AddOrGetExistingIntermediateNode(IState trigger, int origin, int location)
        {
            // PERF: Avoid Linq FirstOrDefault due to lambda allocation
            IIntermediateNode intermediateNode = null;
            foreach (var node in _intermediateNodes)
                if (node.State.Equals(trigger))
                {
                    intermediateNode = node;
                    break;
                }

            if (intermediateNode == null)
            {
                intermediateNode = new IntermediateNode(trigger, origin, location);
            }
            return intermediateNode;
        }
        
        public void Clear()
        {
            _symbolNodes.Clear();
            _intermediateNodes.Clear();
        }
    }
}
