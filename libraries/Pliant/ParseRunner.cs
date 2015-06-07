using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class ParseRunner
    {
        private IGrammar _grammar;
        private TextReader _textReader;
        private char _character;
        private bool _shouldReadCharacter;
        private int _position;
        private IEnumerable<ILexeme> _lexemes;
        private IEnumerable<ILexeme> _ignoreRules;
        
        public Parser Parser { get; private set; }

        public ParseRunner(IGrammar grammar, string input)
            : this(grammar, new StringReader(input))
        {
        }

        public ParseRunner(IGrammar grammar, TextReader textReader)
        {
            _position = 0;
            _grammar = grammar;
            _textReader = textReader;
            _shouldReadCharacter = true;
            Parser = new Parser(_grammar);
            SetCurrentLexemes();
        }

        private void SetCurrentLexemes()
        {
            _lexemes = Parser
                .GetLexerRules()
                .Select(rule => new Lexeme(rule))
                .ToArray();
        }


        public bool Pulse()
        {
            // end of stream
            if (_textReader.Peek() == -1)
                return false;
            
            var c = ReadCharacter();

            IEnumerable<ILexeme> nextLexemes = _lexemes
                .Where(l => l.Scan(c))
                .ToArray();

            var passingLexemesRemain = nextLexemes.Count() > 0 && _textReader.Peek() != -1;
            if (passingLexemesRemain)
            {
                SetPassingLexemes(nextLexemes);
                return true;
            }
            
            // fail if no accepted lexemes were found in the current set
            var firstValidLexeme = GetLongestAcceptableMatchLexeme();
            if (firstValidLexeme == null)
            {
                var ignoreLexemes = _ignoreRules == null || _ignoreRules.Count() == 0
                    ? _grammar
                        .Ignores
                        .Select(i => new Lexeme(i))
                    : _ignoreRules;
                var nextIgnoreRules = ignoreLexemes
                    .Where(x => x.Scan(c))
                    .ToArray();
                if(nextIgnoreRules.Count() == 0)
                    return false;
                _ignoreRules = nextIgnoreRules;
                return true;
            }

            // emit the token to the parser
            var token = new Token(firstValidLexeme.Capture, 
                _position - ( firstValidLexeme.Capture.Length + 1), 
                firstValidLexeme.TokenType);

            var parseResult = Parser.Pulse(token);
            if (!parseResult)
            {
                return false;
            }

            // set the next wave of lexemes
            SetCurrentLexemes();

            return true;
        }
        
        private char ReadCharacter()
        {
            if (_shouldReadCharacter)
            {
                _character = (char)_textReader.Read();
                _position++;
            }
            _shouldReadCharacter = false;
            return _character;
        }
        
        private void SetPassingLexemes(IEnumerable<ILexeme> nextLexemes)
        {
            _lexemes = nextLexemes;
            _shouldReadCharacter = true;
        }

        private ILexeme GetLongestAcceptableMatchLexeme()
        {
            return _lexemes.Where(l => l.IsAccepted()).FirstOrDefault();
        }
    }
}
