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
            Position = -1;
            ParseEngine = parseEngine;
            SetCurrentLexemes();
        }

        private void SetCurrentLexemes()
        {
            _lexemes = GetExpectedLexerRules()
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
            ClearIgnoreLexemes();

            var currentPassingLexemes = GetPassingLexemesForCharacter(character);
            if (currentPassingLexemes.Any())
            {
                if (!EndOfStream())
                {
                    SetNextLexemes(currentPassingLexemes);
                    return true;
                }
                var lastToken = CreateToken(currentPassingLexemes);
                return ParseEngine.Pulse(lastToken);
            }
            else
            {
                var ignoreLexemes = GetIgnoreLexemesForCharacter(character);
                SetIgnoreLexemes(ignoreLexemes);
                if (ignoreLexemes.Any())
                    return true;
            }

            var token = CreateToken(_lexemes);
            var parseResult = ParseEngine.Pulse(token);
            if (!parseResult)
                return false;

            _lexemes = GetPredictedLexemes();
            currentPassingLexemes = GetPassingLexemesForCharacter(character);

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

            return true;
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
        
        private IEnumerable<ILexeme> GetPredictedLexemes()
        {
            return GetExpectedLexerRules()
                .Select(CreateLexemeForLexerRule)
                .ToArray();
        }
        
        private void SetNextLexemes(IEnumerable<ILexeme> nextLexemes)
        {
            _lexemes = nextLexemes;
        }
        
        private IEnumerable<ILexerRule> GetExpectedLexerRules()
        {
            return ParseEngine.GetExpectedLexerRules();
        }

        private IEnumerable<ILexerRule> GetIgnoreLexerRules()
        {
            return ParseEngine.Grammar.Ignores;
        }

        private static ILexeme CreateLexemeForLexerRule(ILexerRule lexerRule)
        {
            if (lexerRule.LexerRuleType == LexerRuleType.Grammar)
            {
                var grammarLexerRule = lexerRule as IGrammarLexerRule;
                var parseEngine = new ParseEngine(grammarLexerRule.Grammar);
                return new ParseEngineLexeme(parseEngine, lexerRule.TokenType);
            }

            return new TerminalLexeme(lexerRule as ITerminalLexerRule);
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

        private IEnumerable<ILexeme> GetIgnoreLexemesForCharacter(char character)
        {
            return GetIgnoreLexerRules()
                .Select(CreateLexemeForLexerRule)
                .Where(lexeme => lexeme.Scan(character))
                .ToArray();
        }

        private void ClearIgnoreLexemes()
        {
            SetIgnoreLexemes(EmptyLexemeArray);
        }

        private void SetIgnoreLexemes(IEnumerable<ILexeme> ignoreRules)
        {
            _ignoreLexemes = ignoreRules;
        }        
    }
}
