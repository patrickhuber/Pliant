using Pliant.Charts;
using Pliant.Grammars;
using Pliant.Tokens;
using Pliant.Utilities;
using System.Collections.Generic;

namespace Pliant.Forest
{
    public class ForestNodeSet
    {
        private readonly Dictionary<int, ISymbolForestNode> _symbolNodes;
        private readonly Dictionary<int, IIntermediateForestNode> _intermediateNodes;
        private readonly Dictionary<int, VirtualForestNode> _virtualNodes;
        private readonly Dictionary<IToken, ITokenForestNode> _tokenNodes;

        public ForestNodeSet()
        {
            _symbolNodes = new Dictionary<int, ISymbolForestNode>();
            _intermediateNodes = new Dictionary<int, IIntermediateForestNode>();
            _virtualNodes = new Dictionary<int, VirtualForestNode>();
            _tokenNodes = new Dictionary<IToken, ITokenForestNode>();
        }

        public bool TryGetExistingSymbolForestNode(ISymbol symbol, int origin, int location, out ISymbolForestNode node)
        {
            var hash = ComputeHashCode(symbol, origin, location);
            node = null;
            if (!_symbolNodes.TryGetValue(hash, out ISymbolForestNode symbolNode))
                return false;
            node = symbolNode;
            return true;
        }

        public ISymbolForestNode AddOrGetExistingSymbolNode(ISymbol symbol, int origin, int location)
        {
            var hash = ComputeHashCode(symbol, origin, location);

            if (_symbolNodes.TryGetValue(hash, out ISymbolForestNode symbolNode))
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

        public bool TryGetExistingIntermediateNode(IDottedRule dottedRule, int origin, int location, out IIntermediateForestNode node)
        {
            int hash = ComputeHashCode(dottedRule, origin, location);
            node = null;
            if (!_intermediateNodes.TryGetValue(hash, out IIntermediateForestNode intermediateNode))
                return false;
            node = intermediateNode;
            return true;
        }

        public IIntermediateForestNode AddOrGetExistingIntermediateNode(IDottedRule dottedRule, int origin, int location)
        {
            int hash = ComputeHashCode(dottedRule, origin, location);

            if (_intermediateNodes.TryGetValue(hash, out IIntermediateForestNode intermediateNode))
                return intermediateNode;

            intermediateNode = new IntermediateForestNode(dottedRule, origin, location);
            _intermediateNodes.Add(hash, intermediateNode);
            return intermediateNode;
        }

        private static int ComputeHashCode(IDottedRule dottedRule, int origin, int location)
        {            
            return HashCode.Compute(
                dottedRule.GetHashCode(),
                origin.GetHashCode(),
                location.GetHashCode());
        }

        public ITokenForestNode AddOrGetExistingTokenNode(IToken token)
        {
            if (_tokenNodes.TryGetValue(token, out ITokenForestNode tokenNode))
                return tokenNode;
            tokenNode = new TokenForestNode(token, token.Position, token.Capture.Count);
            _tokenNodes.Add(token, tokenNode);
            return tokenNode;
        }

        public void AddNewVirtualNode(
            VirtualForestNode virtualNode)
        {
            var hash = ComputeHashCode(
                virtualNode.Symbol, 
                virtualNode.Origin, 
                virtualNode.Location);
            _virtualNodes.Add(hash, virtualNode);
        }

        public bool TryGetExistingVirtualNode(
            int location,
            ISymbol searchSymbol,
            int origin,
            out VirtualForestNode node)
        {
            var hash = ComputeHashCode(searchSymbol, origin, location);
            return _virtualNodes.TryGetValue(hash, out node);
        }

        public void Clear()
        {
            _symbolNodes.Clear();
            _intermediateNodes.Clear();
            _virtualNodes.Clear();
            _tokenNodes.Clear();
        }
    }
}