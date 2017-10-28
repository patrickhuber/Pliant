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

        public IIntermediateForestNode AddOrGetExistingIntermediateNode(IDottedRule dottedRule, int origin, int location)
        {
            int hash = ComputeHashCode(dottedRule, origin, location);

            IIntermediateForestNode intermediateNode = null;
            if (_intermediateNodes.TryGetValue(hash, out intermediateNode))
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
            ITokenForestNode tokenNode = null;
            if (_tokenNodes.TryGetValue(token, out tokenNode))
                return tokenNode;
            tokenNode = new TokenForestNode(token, token.Position, token.Value.Length);
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
            ITransitionState transitionState,
            out VirtualForestNode node)
        {
            var targetState = transitionState.GetTargetState();
            var hash = ComputeHashCode(targetState.DottedRule.Production.LeftHandSide, targetState.Origin, location);
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