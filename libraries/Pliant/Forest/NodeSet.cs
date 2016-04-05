using Pliant.Charts;
using Pliant.Grammars;
using System.Collections.Generic;

namespace Pliant.Forest
{
    public class NodeSet
    {
        private readonly IDictionary<int, ISymbolNode> _symbolNodes;
        private readonly IDictionary<int, IIntermediateNode> _intermediateNodes;

        public NodeSet()
        {
            _symbolNodes = new Dictionary<int, ISymbolNode>();
            _intermediateNodes = new Dictionary<int, IIntermediateNode>();
        }

        public ISymbolNode AddOrGetExistingSymbolNode(ISymbol symbol, int origin, int location)
        {
            var hash = HashUtil.ComputeHash(symbol.GetHashCode(), origin.GetHashCode(), location.GetHashCode());

            ISymbolNode symbolNode = null;
            if (_symbolNodes.TryGetValue(hash, out symbolNode))
                return symbolNode;

            symbolNode = new SymbolNode(symbol, origin, location);
            _symbolNodes.Add(hash, symbolNode);
            return symbolNode;
        }

        public IIntermediateNode AddOrGetExistingIntermediateNode(IState trigger, int origin, int location)
        {
            var hash = HashUtil.ComputeHash(trigger.GetHashCode());
            IIntermediateNode intermediateNode = null;
            if (_intermediateNodes.TryGetValue(hash, out intermediateNode))
                return intermediateNode;
            intermediateNode = new IntermediateNode(trigger, origin, location);
            _intermediateNodes.Add(hash, intermediateNode);
            return intermediateNode;
        }

        public void Clear()
        {
            _symbolNodes.Clear();
            _intermediateNodes.Clear();
        }
    }
}