using System.IO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Pliant.Runtime;
using Pliant.Automata;
using Pliant.Utilities;
using Pliant.Tokens;
using Pliant.Grammars;
using System;

namespace Pliant.Runtime
{
    public class ParseRunner : MemoryEfficientParseRunner
    {
        public ParseRunner(IParseEngine parseEngine, string input) 
            : base(parseEngine, input)
        { }

        public ParseRunner(IParseEngine parseEngine, TextReader textReader)
            : base(parseEngine, textReader)
        { }
    }   

}