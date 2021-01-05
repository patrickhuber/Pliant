
using Pliant.Captures;
using Pliant.Tokens;
using Pliant.Utilities;
using System.Text;

namespace Pliant.Automata
{
    public class DfaLexeme : LexemeBase<IDfaLexerRule>, ILexeme
    {       
        
        private IDfaState _currentState;
                
        public DfaLexeme(IDfaLexerRule dfaLexerRule, ICapture<char> segment, int offset)
            : base(dfaLexerRule, segment, offset)
        {            
            _currentState = dfaLexerRule.Start;
        }
                
        public override void Reset()
        {
            _currentState = ConcreteLexerRule.Start;
        }

        public override bool IsAccepted()
        {
            return _currentState.IsFinal;
        }

        public override bool Scan()
        {
            if (!Capture.Peek(out char c))
                return false;

            for (var e = 0; e < _currentState.Transitions.Count; e++)
            {
                var edge = _currentState.Transitions[e];

                if (edge.Terminal.IsMatch(c))
                {                    
                    _currentState = edge.Target;
                    return Capture.Grow();
                }
            }
            return false;
        }
    }
}