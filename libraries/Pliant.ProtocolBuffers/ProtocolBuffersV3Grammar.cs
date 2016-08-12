using Pliant.Ebnf;
using Pliant.Grammars;
using System.IO;
using System.Reflection;

namespace Pliant.ProtocolBuffers
{
    public class ProtocolBuffersV3Grammar : GrammarWrapper
    {
        private static readonly IGrammar _grammar;

        static ProtocolBuffersV3Grammar()
        {
            using (var memoryStream = new MemoryStream(Resources.ProtocolBufferesV3))
            using (var textReader = new StreamReader(memoryStream))
            {
                var ebnfParser = new EbnfParser();
                var ebnf = ebnfParser.Parse(textReader.ReadToEnd());
                var ebnfGrammarGenerator = new EbnfGrammarGenerator();
                _grammar = ebnfGrammarGenerator.Generate(ebnf);
            }
        }

        public ProtocolBuffersV3Grammar() 
            : base(_grammar)
        {
        }
    }
}
