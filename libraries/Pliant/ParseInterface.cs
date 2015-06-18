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
        private IEnumerable<ILexeme> _existingLexemes;
        private IEnumerable<ILexeme> _ignoreLexemes;
        private ILexemeFactoryRegistry _lexemeFactoryRegistry;

        private static readonly ILexeme[] EmptyLexemeArray = new ILexeme[] { };

        public ParseInterface(IParseEngine parseEngine, string input)
            : this(parseEngine, new StringReader(input))
        {
        }

        public ParseInterface(IParseEngine parseEngine, TextReader input)
        {
            _textReader = input;
            _lexemeFactoryRegistry = new LexemeFactoryRegistry();
            _lexemeFactoryRegistry.Register(new TerminalLexemeFactory());
            _lexemeFactoryRegistry.Register(new ParseEngineLexemeFactory());
            Position = -1;
            ParseEngine = parseEngine;
        }

        public bool Read()
        {
            if (EndOfStream())
                return false;

            var character = ReadCharacter();

            if (MatchesExistingIgnoreLexemes(character))
                return true;
            else
                ClearExistingIngoreLexemes();

            if (MatchesExistingLexemes(character))
            {
                if (!EndOfStream())
                    return true;
                return TryParseExistingToken();
            }
            else
            {
                if (AnyExistingLexemes())
                    if (!TryParseExistingToken())
                        return false;

                if (MatchesIgnoreLexemes(character))
                    return true;

                if (MatchesNextLexemes(character))
                {
                    if (!EndOfStream())
                        return true;
                    return TryParseExistingToken();
                }
            }

            return false;
        }

        private bool MatchesNextLexemes(char character)
        {
             var nextLexemes = ParseEngine
                .GetExpectedLexerRules()
                .Select(CreateLexemeForLexerRule)
                .Where(x=>x.Scan(character))
                .ToArray();

            if (!nextLexemes.Any())
                return false;

            _existingLexemes = nextLexemes;
            return true;
        }

        private bool MatchesExistingIgnoreLexemes(char character)
        {
            if (_ignoreLexemes.IsNullOrEmpty())
                return false;
            return _ignoreLexemes
                .Where(x => x.Scan(character))
                .Any();
        }

        private bool MatchesIgnoreLexemes(char character)
        {
            var ignoreLexerRules = ParseEngine.Grammar.Ignores;
            if (ignoreLexerRules.IsNullOrEmpty())
                return false;
            var matchingIgnoreLexemes = ignoreLexerRules
                .Select(CreateLexemeForLexerRule)
                .Where(x => x.Scan(character))
                .ToArray();
            if (matchingIgnoreLexemes.Any())
            {
                _ignoreLexemes = matchingIgnoreLexemes;
                return true;
            }
            return false;   
        }

        private void ClearExistingIngoreLexemes()
        {
            _ignoreLexemes = EmptyLexemeArray;
        }

        private bool MatchesExistingLexemes(char character)
        {
            if (_existingLexemes.IsNullOrEmpty())
                return false;
            return _existingLexemes
                .Where(x => x.Scan(character))
                .Any();
        }

        private void ClearExistingLexemes()
        {
            _existingLexemes = EmptyLexemeArray;
        }

        private bool TryParseExistingToken()
        {
            var longestAcceptedMatch = _existingLexemes
                .FirstOrDefault(x => x.IsAccepted());

            if (longestAcceptedMatch == null)
                return false;

            var token = CreateTokenFromLexeme(longestAcceptedMatch);
            if (token == null)
                return false;

            if (!ParseEngine.Pulse(token))
                return false;

            ClearExistingLexemes();
            return true;
        }
        
        private bool AnyExistingLexemes()
        {
            return !_existingLexemes.IsNullOrEmpty();
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
        
        private ILexeme CreateLexemeForLexerRule(ILexerRule lexerRule)
        {
            return _lexemeFactoryRegistry
                .Get(lexerRule.LexerRuleType)
                .Create(lexerRule);
        }
        
        private IToken CreateTokenFromLexeme(ILexeme lexeme)
        {
            return new Token(
                lexeme.Capture,
                Position - (lexeme.Capture.Length + 1),
                lexeme.TokenType);
        }
                
    }
}
