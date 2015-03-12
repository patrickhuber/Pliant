using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class Lexer : Observable<IToken>
    {
        private PulseRecognizer _recognizer;

        public Lexer(IGrammar grammar)
            : base()
        {
            _recognizer = new PulseRecognizer(grammar);
        }

        public void Scan(char c)
        {
            _recognizer.Pulse(c);
            IToken token = null;
            if (TryGetToken(_recognizer, out token))
            {
                OnNext(token);
            }
        }

        public void Scan(TextReader textReader)
        {
 
        }

        private bool TryGetToken(PulseRecognizer recognizer, out IToken token)
        {
            token = null;
            return false;
        }
    }
}
