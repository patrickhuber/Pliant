using System;
using Pliant.Tokens;
using Pliant.Charts;
using Pliant.Grammars;

namespace Pliant.Runtime
{
    public class CompressedParseEngine
    {
        PreComputedGrammar _precomputedGrammar;
        PreComputedChart _chart;

        public int Location { get; private set; }

        public CompressedParseEngine(PreComputedGrammar preComputedGrammar)
        {
            _precomputedGrammar = preComputedGrammar;
            Initialize();
        }

        private void Initialize()
        {
            Location = 0;
            _chart = new PreComputedChart();
            var kernelFrame = _precomputedGrammar.Start;
            Enqueue(Location, new StateFrame(kernelFrame, 0));
        }

        private bool Enqueue(int location, StateFrame stateFrame)
        {
            if (!_chart.Enqueue(location, stateFrame))
                return false;            

            if (stateFrame.Frame.NullTransition == null)
                return true;

            var nullTransitionFrame = new StateFrame(
                stateFrame.Frame.NullTransition, 
                location);

            return _chart.Enqueue(location, nullTransitionFrame);
        }

        public bool PulseBreakout(Token token)
        {
            ScanPass(Location, token);
            
            var tokenRecognized = _chart.FrameSets.Count > Location + 1;
            if (!tokenRecognized)
                return false;

            Location++;
            ReductionPass(Location);

            return true;
        }

        public bool Pulse(Token token)
        {            
            Process(Location, token);
            var tokenRecognized = _chart.FrameSets.Count > Location + 1;
            if (!tokenRecognized)
                return false;
            Location++;
            return true;
        }
        
        private void ScanPass(int location, Token token)
        {
            var frameSet = _chart.FrameSets[Location];
            for (int i = 0; i < frameSet.Frames.Count; i++)
            {
                var frame = frameSet.Frames[i];
                Scan(token, location, frame);
            }
        }

        private void Scan(Token token, int location, StateFrame stateFrame)
        {
            var frame = stateFrame.Frame;
            var origin = stateFrame.Origin;

            foreach (var symbol in frame.Transitions.Keys)
            {
                if (symbol.SymbolType != SymbolType.LexerRule)
                    continue;

                var lexerRule = symbol as ILexerRule;
                if (lexerRule.TokenType != token.TokenType)
                    continue;

                var target = frame.Transitions[symbol];
                if (_chart.Enqueue(location + 1, new StateFrame(target, origin)))
                {
                    if (target.NullTransition != null)
                    {
                        _chart.Enqueue(location + 1, new StateFrame(target.NullTransition, location + 1));
                    }
                }
                // Enqueue(location + 1, new StateFrame(target, origin));
            }
        }

        private void ReductionPass(int location)
        {
            var frameSet = _chart.FrameSets[location];
                               
            for (int i = 0; i < frameSet.Frames.Count; i++)
            {
                var stateFrame = frameSet.Frames[i];
                var frame = stateFrame.Frame;
                var origin = stateFrame.Origin;

                if (origin == location)
                    continue;
                
                foreach (var preComputedState in frame.Data)
                {
                    var isComplete = preComputedState.Position == preComputedState.Production.RightHandSide.Count;
                    if (!isComplete)
                        continue;

                    var leftHandSide = preComputedState.Production.LeftHandSide;
                    var originFrame = _chart.FrameSets[origin];
                    
                    for (int j = 0; j < originFrame.Frames.Count; j++)
                    {
                        var searchStateFrame = originFrame.Frames[j];

                        Frame targetFrame = null;
                        if (!searchStateFrame.Frame.Transitions.TryGetValue(leftHandSide, out targetFrame))
                            continue;

                        if (_chart.Enqueue(location, new StateFrame(targetFrame, origin)))
                        {
                            if (targetFrame.NullTransition != null)
                            {
                                _chart.Enqueue(location, new StateFrame(targetFrame.NullTransition, location));
                            }
                        }
                        // Enqueue(location, new StateFrame(targetFrame, origin));
                    }
                }
            }
        }

        public bool IsAccepted()
        {
            // TODO: scan for a completed start production in the last frame set
            return true;
        }

        private void Process(int i, Token token)
        {
            var set = _chart.FrameSets[i];

            for (int f = 0; f < set.Frames.Count; f++)
            {
                var state = set.Frames[f];
                var parent = state.Origin;
                var frame = state.Frame;

                foreach (var symbol in frame.Transitions.Keys)
                {
                    if (symbol.SymbolType != SymbolType.LexerRule)
                        continue;

                    var lexerRule = symbol as ILexerRule;
                    if (lexerRule.TokenType != token.TokenType)
                        continue;

                    var target = frame.Transitions[symbol];

                    if (!_chart.Enqueue(i + 1, new StateFrame(target, parent)))
                        continue;

                    if (target.NullTransition == null)
                        continue;

                    _chart.Enqueue(i + 1, new StateFrame(target.NullTransition, i + 1));                    
                }

                if (parent == i)
                    continue;

                foreach (var preComputedState in frame.Data)
                {
                    var isComplete = preComputedState.Position == preComputedState.Production.RightHandSide.Count;
                    if (!isComplete)
                        continue;

                    var leftHandSide = preComputedState.Production.LeftHandSide;
                    var parentSet = _chart.FrameSets[parent];

                    for (int p = 0; p < parentSet.Frames.Count; p++)
                    {
                        var pState = parentSet.Frames[p];
                        var pParent = pState.Origin;

                        Frame target = null;
                        if (!pState.Frame.Transitions.TryGetValue(leftHandSide, out target))
                            continue;

                        if (!_chart.Enqueue(i, new StateFrame(target, pParent)))
                            continue;

                        if (target.NullTransition == null)
                            continue;

                        _chart.Enqueue(i, new StateFrame(target.NullTransition, i));
                    }
                }
            }
        }
    }
}
