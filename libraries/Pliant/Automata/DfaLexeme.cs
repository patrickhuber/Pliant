using Pliant.Lexemes;
using Pliant.Tokens;
using System.Text;

namespace Pliant.Automata
{
    public class DfaLexeme : ILexeme
    {
        private readonly StringBuilder _capture;
        private IDfaState _currentState;

        public DfaLexeme(IDfaState dfaState, TokenType tokenType)
        {
            _capture = new StringBuilder();
            _currentState = dfaState;
            TokenType = tokenType;
        }

        public string Capture
        {
            get { return _capture.ToString(); }
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
                    _currentState = edge.Target;
                    _capture.Append(c);
                    return true;
                }
            }
            return false;
        }
    }
}