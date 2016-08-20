using Pliant.Lexemes;
using Pliant.Tokens;
using Pliant.Utilities;
using System.Text;
using System;
using Pliant.Grammars;

namespace Pliant.Automata
{
    public class DfaLexeme : ILexeme
    {
        private StringBuilder _stringBuilder;
        private string _capture;

        private IDfaState _currentState;

        public TokenType TokenType { get { return LexerRule.TokenType; } }

        public ILexerRule LexerRule { get; private set; }

        public string Capture
        {
            get
            {
                if (IsStringBuilderAllocated())
                    DeallocateStringBuilderAndAssignCapture();
                return _capture;
            }
        }
        
        public DfaLexeme(IDfaLexerRule dfaLexerRule)
        {
            LexerRule = dfaLexerRule;
            _stringBuilder = SharedPools.Default<StringBuilder>().AllocateAndClear();
            _currentState = dfaLexerRule.Start;
        }

        private bool IsStringBuilderAllocated()
        {
            return _stringBuilder != null;
        }

        public void Reset(IDfaLexerRule dfaLexerRule)
        {
            _capture = null;
            if(IsStringBuilderAllocated())
                _stringBuilder.Clear();
            _currentState = dfaLexerRule.Start;
            LexerRule = dfaLexerRule;
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
            _stringBuilder.Append(_stringBuilder);
        }

        public bool IsAccepted()
        {
            return _currentState.IsFinal;
        }

        public bool Scan(char c)
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