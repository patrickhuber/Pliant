
using Pliant.Tokens;
using Pliant.Utilities;
using System.Text;

namespace Pliant.Automata
{
    public class DfaLexeme : LexemeBase<IDfaLexerRule>, ILexeme
    {
        private StringBuilder _stringBuilder;
        private string _capture;

        private IDfaState _currentState;

        // TODO: Make property inspection work better for the debugger        
        public override string Value
        {
            get
            {
                if (IsStringBuilderAllocated())
                    DeallocateStringBuilderAndAssignCapture();
                return _capture;
            }
        }        
        
        public DfaLexeme(IDfaLexerRule dfaLexerRule, int position)
            : base(dfaLexerRule, position)
        {
            _stringBuilder = SharedPools.Default<StringBuilder>().AllocateAndClear();
            _currentState = dfaLexerRule.Start;
        }

        private bool IsStringBuilderAllocated()
        {
            return _stringBuilder != null;
        }
        
        public override void Reset()
        {
            _capture = null;
            if (IsStringBuilderAllocated())
                _stringBuilder.Clear();
            _currentState = ConcreteLexerRule.Start;
        }

        private void DeallocateStringBuilderAndAssignCapture()
        {
            _capture = _stringBuilder.ToString();
            SharedPools.Default<StringBuilder>().ClearAndFree(_stringBuilder);
            _stringBuilder = null;
        }

        private void ReallocateStringBuilderFromCapture()
        {
            _stringBuilder = SharedPools.Default<StringBuilder>().AllocateAndClear();
            if(!string.IsNullOrWhiteSpace(_capture))
                _stringBuilder.Append(_capture);
        }

        public override bool IsAccepted()
        {
            return _currentState.IsFinal;
        }

        public override bool Scan(char c)
        {
            for(var e = 0; e<_currentState.Transitions.Count; e++)
            {
                var edge = _currentState.Transitions[e];
                if (edge.Terminal.IsMatch(c))
                {
                    if (!IsStringBuilderAllocated())
                        ReallocateStringBuilderFromCapture();
                    _currentState = edge.Target;
                    _stringBuilder.Append(c);
                    return true;
                }
            }
            return false;
        }
    }
}