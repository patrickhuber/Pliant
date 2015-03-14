using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    /// <summary>
    /// A Lexeme is something special. It acts like a token and a mini parser.
    /// </summary>
    public class Lexeme
    {
        private StringBuilder _catpure;
        private PulseRecognizer _recognizer;
        
        public Lexeme(ILexerRule lexerRule)
        {
            _catpure = new StringBuilder();
            _recognizer = new PulseRecognizer(lexerRule.Grammar);
        }
                
        public string Capture { get { return _catpure.ToString(); } }
        
        public bool Match(char c)
        {
            int originalChartSize = _recognizer.Chart.Count;
            _recognizer.Pulse(c);
            bool characterWasMatched =  originalChartSize < _recognizer.Chart.Count;
            if (characterWasMatched)
                _catpure.Append(c);
            return characterWasMatched;
        }

    }
}
