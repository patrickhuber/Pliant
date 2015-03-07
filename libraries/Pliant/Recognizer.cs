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

        public Chart Parse(TextReader textReader)
        {
            var pulseRecognizer = new PulseRecognizer(_grammar);

            int origin = 0;
            while(origin < pulseRecognizer.Chart.Count)
            {
                var token = (char)textReader.Read();
                pulseRecognizer.Pulse(token);
                origin++;                
            }
            return pulseRecognizer.Chart;
        }                
    }
}
