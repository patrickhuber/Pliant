using Pliant.Charts;
using Pliant.Grammars;
using Pliant.Utilities;
using System.Collections.Generic;

namespace Pliant.Forest
{
    public class ForestNodeSet
    {
        private readonly Dictionary<int, ISymbolForestNode> _symbolNodes;
        private readonly Dictionary<int, IIntermediateForestNode> _intermediateNodes;

        public ForestNodeSet()
        {
            _symbolNodes = new Dictionary<int, ISymbolForestNode>();
            _intermediateNodes = new Dictionary<int, IIntermediateForestNode>();
        }

        public ISymbolForestNode AddOrGetExistingSymbolNode(ISymbol symbol, int origin, int location)
        {
            var hash = ComputeHashCode(symbol, origin, location);

            ISymbolForestNode symbolNode = null;
            if (_symbolNodes.TryGetValue(hash, out symbolNode))
                return symbolNode;

            symbolNode = new SymbolForestNode(symbol, origin, location);
            _symbolNodes.Add(hash, symbolNode);
            return symbolNode;
        }
        
        private static int ComputeHashCode(ISymbol symbol, int origin, int location)
        {
            return HashCode.Compute(
                symbol.GetHashCode(), 
                origin.GetHashCode(), 
                location.GetHashCode());
        }

        public IIntermediateForestNode AddOrGetExistingIntermediateNode(IState trigger, int origin, int location)
        {
            var hash = trigger.GetHashCode();
            IIntermediateForestNode intermediateNode = null;
            if (_intermediateNodes.TryGetValue(hash, out intermediateNode))
                return intermediateNode;
            intermediateNode = new IntermediateForestNode(trigger, origin, location);
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