using System.IO;

namespace Pliant
{
    public class Recognizer
    {
        private IGrammar _grammar;
        private const int EndOfRead = -1;

        public Recognizer(IGrammar grammar)
        {
            Assert.IsNotNull(grammar, "grammar");
            _grammar = grammar;
        }

        public bool Recognize(TextReader textReader)
        {
            var pulseRecognizer = new PulseRecognizer(_grammar);
            
            while(textReader.Peek() != EndOfRead)
            {
                var token = (char)textReader.Read();
                if (!pulseRecognizer.Pulse(token))
                    return false;
            }
            return pulseRecognizer.IsAccepted();
        }                
    }
}
