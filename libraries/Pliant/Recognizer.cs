using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class Recognizer
    {
        private IGrammar _grammar;

        public Recognizer(IGrammar grammar)
        {
            Assert.IsNotNull(grammar, "grammar");
            _grammar = grammar;
        }

        public bool Recognize(TextReader textReader)
        {
            var pulseRecognizer = new PulseRecognizer(_grammar);

            while(textReader.Peek() != -1)
            {
                var token = (char)textReader.Read();
                if (!pulseRecognizer.Pulse(token))
                    return false;
            }
            return pulseRecognizer.IsAccepted();
        }                
    }
}
