using Pliant.Collections;
using Pliant.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pliant
{
    public class ParseInterface : IParseInterface
    {
        public IParseEngine ParseEngine { get; private set; }

        public int Position { get; private set; }

        private TextReader _textReader;
        private IEnumerable<ILexeme> _lexemes;
        private IEnumerable<ILexeme> _ignoreLexemes;

        public ParseInterface(IParseEngine parseEngine, string input)
            : this(parseEngine, new StringReader(input))
        {
        }

        public ParseInterface(IParseEngine parseEngine, TextReader input)
        {
            _textReader = input;
            Position = 0;
            ParseEngine = parseEngine;
            SetCurrentLexemes();
        }

        private void SetCurrentLexemes()
        {
            _lexemes = ParseEngine.GetExpectedLexerRules()
                .Select(RuleBuilder => new Lexeme(RuleBuilder))
                .ToArray();
        }

        public bool Read()
        {
            if (EndOfStream())
                return false;

            var character = ReadCharacter();

            if (ShouldIgnoreCharacter(character))
                return true;

            var nextLexemes = GetNextLexemesForCharacter(character);
            if (ShouldEmitToken(nextLexemes))
            {
                var token = CreateToken(nextLexemes);
                if (token == null)
                    return false;

                _lexemes = nextLexemes;

                var parseResult = ParseEngine.Pulse(token);
                if (!parseResult)
                    return false;
            }

            if (nextLexemes.Any())
            {
                _lexemes = nextLexemes;
                return true;
            }

            var ignoreLexemes = CreateIgnoreRulesForCharacter(character);
            return ignoreLexemes.Any();
        }
        
        private bool EndOfStream()
        {
            return _textReader.Peek() == -1;
        }

        private char ReadCharacter()
        {
            var character = (char)_textReader.Read();
            Position++;
            return character;
        }

        private bool ShouldIgnoreCharacter(char character)
        {
            if (_ignoreLexemes.IsNullOrEmpty())
                return false;

            _ignoreLexemes = _ignoreLexemes
                .Where(x => x.Scan(character))
                .ToArray();

            return _ignoreLexemes.Any();
        }

        private IEnumerable<ILexeme> GetNextLexemesForCharacter(char character)
        {
            return (_lexemes.IsNullOrEmpty()
                    ? ParseEngine
                        .GetExpectedLexerRules()
                        .Select(lexerRule => new Lexeme(lexerRule))
                    : _lexemes)
                .Where(lexeme => lexeme.Scan(character))
                .ToArray();
        }

        private bool ShouldEmitToken(IEnumerable<ILexeme> nextLexemes)
        {
            return ShouldEmitTokenFromPreviousLexemes(nextLexemes) 
                || ShouldEmitTokenFromNextLexemes(nextLexemes);
        }

        private bool ShouldEmitTokenFromNextLexemes(IEnumerable<ILexeme> nextLexemes)
        {
            return EndOfStream() && nextLexemes.Any();
        }

        private bool ShouldEmitTokenFromPreviousLexemes(IEnumerable<ILexeme> nextLexemes)
        {
            return _lexemes.Any() && !nextLexemes.Any();
        }

        private IToken CreateToken(IEnumerable<ILexeme> nextLexemes)
        {
            if (ShouldEmitTokenFromNextLexemes(nextLexemes))
                return CreateTokenFromLexemes(nextLexemes);
            return CreateTokenFromLexemes(_lexemes);
        }

        private IToken CreateTokenFromLexemes(IEnumerable<ILexeme> lexemes)
        {
            var firstAcceptedLexeme = lexemes
                .FirstOrDefault(x => x.IsAccepted());

            if (firstAcceptedLexeme == null)
                return null;

            return CreateTokenFromLexeme(firstAcceptedLexeme);
        }

        private IToken CreateTokenFromLexeme(ILexeme lexeme)
        {
            return new Token(
                lexeme.Capture,
                Position - (lexeme.Capture.Length + 1),
                lexeme.TokenType);
        }

        private IEnumerable<ILexeme> CreateIgnoreRulesForCharacter(char character)
        {
            _ignoreLexemes = ParseEngine.Grammar
                .Ignores
                .Select(lexerRule => new Lexeme(lexerRule))
                .Where(lexeme => lexeme.Scan(character))
                .ToArray();
            return _ignoreLexemes;
        }
    }
}
