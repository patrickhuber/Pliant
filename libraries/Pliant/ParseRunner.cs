using Pliant.Collections;
using Pliant.Tokens;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pliant
{
    public class ParseRunner
    {
        private IGrammar _grammar;
        private TextReader _textReader;
        private int _position;
        private IEnumerable<ILexeme> _lexemes;
        private IEnumerable<ILexeme> _ignoreLexemes;
        
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
            if (IsEndOfStream())
                return false;

            var character = ReadCharacter();

            if (!_ignoreLexemes.IsNullOrEmpty())
            {
                _ignoreLexemes = _ignoreLexemes.Where(x => x.Scan(character));
                if (_ignoreLexemes.Any())
                    return true;
            }

            var nextLexemes = (_lexemes.IsNullOrEmpty() 
                    ? Parser.GetLexerRules().Select(lexerRule => new Lexeme(lexerRule))
                    : _lexemes)
                .Where(lexeme => lexeme.Scan(character))
                .ToArray();

            var emitTokenFromCurrentLexemes = IsEndOfStream() && nextLexemes.Any();
            var emitTokenFromPreviousLexemes = _lexemes.Any() && !nextLexemes.Any();
            var emitToken = emitTokenFromPreviousLexemes || emitTokenFromCurrentLexemes;

            if (emitToken)
            {
                var token = emitTokenFromCurrentLexemes
                    ? CreateTokenFromLexemes(nextLexemes, _position)
                    : CreateTokenFromLexemes(_lexemes, _position);
                                
                if (token == null)
                    return false;

                _lexemes = nextLexemes;

                var parseResult = Parser.Pulse(token);
                if (!parseResult)
                    return false;
            }

            if (nextLexemes.Any())
            {
                _lexemes = nextLexemes;
                return true;
            }

            _ignoreLexemes = _grammar
                .Ignores
                .Select(lexerRule => new Lexeme(lexerRule))
                .Where(lexeme => lexeme.Scan(character));

            return _ignoreLexemes.Any();            
        }

        private static IToken CreateTokenFromLexemes(IEnumerable<ILexeme> lexemes, int position)
        {
            var firstAcceptedLexeme = lexemes
                .FirstOrDefault(x => x.IsAccepted());

            if (firstAcceptedLexeme == null)
                return null;

            return CreateTokenFromLexeme(firstAcceptedLexeme, position);
        }
                
        private static IToken CreateTokenFromLexeme(ILexeme lexeme, int position)
        {
            return new Token(
                lexeme.Capture,
                position - (lexeme.Capture.Length + 1),
                lexeme.TokenType);
        }

        private char ReadCharacter()
        {
            var character = (char)_textReader.Read();
            _position++;
            return character;
        }

        private bool IsEndOfStream()
        {
            return _textReader.Peek() == -1;
        }        
    }
}
