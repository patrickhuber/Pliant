using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
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
            var symbolNode = _symbolNodes.FirstOrDefault(
                n => n.Origin == origin
                    && n.Location == location
                    && n.Symbol.Equals(symbol));
            if (symbolNode == null)
            {
                symbolNode = new SymbolNode(symbol, origin, location);
                _symbolNodes.Add(symbolNode);
            }
            return symbolNode;
        }
        
        public IIntermediateNode AddOrGetExistingIntermediateNode(IState trigger, int origin, int location)
        {
            var intermediateNode = _intermediateNodes.FirstOrDefault(
                x=> x.State.Equals(trigger));
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
