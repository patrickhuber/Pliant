using Pliant.Lexemes;
using Pliant.Tokens;
using Pliant.Utilities;
using System.Text;
using System;

namespace Pliant.Automata
{
    public class DfaLexeme : ILexeme
    {
        private StringBuilder _stringBuilder;
        private string _capture;

        private IDfaState _currentState;

        public DfaLexeme(IDfaState dfaState, TokenType tokenType)
        {
            _stringBuilder = SharedPools.Default<StringBuilder>().AllocateAndClear();
            _currentState = dfaState;
            TokenType = tokenType;
        }

        public string Capture
        {
            get
            {
                if (IsStringBuilderAllocated())
                    DeallocateStringBuilderAndAssignCapture();
                return _capture;
            }
        }

        private bool IsStringBuilderAllocated()
        {
            return _stringBuilder != null;
        }

        public void Reset(IDfaLexerRule dfaLexerRule)
        {
            _capture = null;
            _stringBuilder.Clear();
            _currentState = dfaLexerRule.Start;
            TokenType = dfaLexerRule.TokenType;
        }

        private void DeallocateStringBuilderAndAssignCapture()
        {
            SharedPools.Default<StringBuilder>().Free(_stringBuilder);
            _capture = _stringBuilder.ToString();
            _stringBuilder = null;
        }

        private void ReallocateStringBuilderFromCapture()
        {
            _stringBuilder = SharedPools.Default<StringBuilder>().AllocateAndClear();
            _stringBuilder.Append(_stringBuilder);
        }

        public TokenType TokenType { get; private set; }

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