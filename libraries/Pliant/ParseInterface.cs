using Pliant.Collections;
using Pliant.Grammars;
using Pliant.Lexemes;
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
                .Select(CreateLexemeForLexerRule)
                .ToArray();
        }

        public bool Read()
        {
            if (EndOfStream())
                return false;

            var character = ReadCharacter();

            if (ShouldIgnoreCharacter(character))
                return true;

            var currentPassingLexemes = GetPassingLexemesForCharacter(character);
            if (currentPassingLexemes.Any())
            {
                if (!EndOfStream())
                {
                    SetNextLexemes(currentPassingLexemes);
                    return true;
                }
                var token = CreateToken(currentPassingLexemes);
                return ParseEngine.Pulse(token);
            }
            else if(_lexemes.Any())
            {
                var token = CreateToken(_lexemes);
                var parseResult = ParseEngine.Pulse(token);
                if (!parseResult)
                    return false;

                currentPassingLexemes = GetPredictedLexemesForCharacter(character);
                
                if (currentPassingLexemes.Any())
                {
                    SetNextLexemes(currentPassingLexemes);
                    if (EndOfStream())
                    {
                        token = CreateToken(currentPassingLexemes);
                        return ParseEngine.Pulse(token);
                    }
                    return true;
                }
            }

            var ignoreLexemes = GetIgnoreRulesForCharacter(character);
            SetIgnoreRules(ignoreLexemes);
            return ignoreLexemes.Any();
        }
                
        public bool EndOfStream()
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

        private static readonly ILexeme[] EmptyLexemeArray = new ILexeme[] { };

        private IEnumerable<ILexeme> GetPassingLexemesForCharacter(char character)
        {
            if (_lexemes.IsNullOrEmpty())
                return EmptyLexemeArray;
            return _lexemes
                .Where(lexeme => lexeme.Scan(character))
                .ToArray();
        }

        private IEnumerable<ILexeme> GetPredictedLexemesForCharacter(char character)
        {
            return ParseEngine
                .GetExpectedLexerRules()
                .Select(CreateLexemeForLexerRule)
                .Where(lexeme => lexeme.Scan(character))
                .ToArray();
        }
        
        private void SetNextLexemes(IEnumerable<ILexeme> nextLexemes)
        {
            _lexemes = nextLexemes;
        }

        private IEnumerable<ILexeme> GetNextLexemesForCharacter(char character)
        {
            if (!_lexemes.IsNullOrEmpty())
            {
                var passedLexemes = _lexemes
                    .Where(lexeme => lexeme.Scan(character))
                    .ToArray();
                if (passedLexemes.Length > 0)
                    return passedLexemes;
            }
            return ParseEngine
                .GetExpectedLexerRules()
                .Select(CreateLexemeForLexerRule)
                .Where(lexeme => lexeme.Scan(character))
                .ToArray();
        }

        private static ILexeme CreateLexemeForLexerRule(ILexerRule lexerRule)
        {
            return new Lexeme(lexerRule);
            /*
            return new Lexeme(
                lexerRule.TokenType,
                new ParseEngine(lexerRule.Grammar));
            */
        }

        private bool ShouldEmitTokenFromNextLexemes(IEnumerable<ILexeme> nextLexemes)
        {
            return EndOfStream() && nextLexemes.Any();
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

        private IEnumerable<ILexeme> GetIgnoreRulesForCharacter(char character)
        {
            return ParseEngine.Grammar
                .Ignores
                .Select(CreateLexemeForLexerRule)
                .Where(lexeme => lexeme.Scan(character))
                .ToArray();
        }

        private void SetIgnoreRules(IEnumerable<ILexeme> ignoreRules)
        {
            _ignoreLexemes = ignoreRules;
        }
    }
}
