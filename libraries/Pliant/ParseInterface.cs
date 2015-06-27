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
            _lexemeFactoryRegistry.Register(new StringLiteralLexemeFactory());
            Position = -1;
            ParseEngine = parseEngine;
        }

        public bool Read()
        {
            if (EndOfStream())
                return false;

            var character = ReadCharacter();

            if (MatchesExistingLexemes(character))
            {
                if (!EndOfStream())
                    return true;
                return TryParseExistingToken();
            }
            
            if (AnyExistingLexemes())
                if (!TryParseExistingToken())
                    return false;

            if (MatchesNewLexemes(character))
            {
                if (!EndOfStream())
                    return true;
                return TryParseExistingToken();
            }

            if (MatchesExistingIgnoreLexemes(character))
                return true;
            else
                ClearExistingIngoreLexemes();
                        
            if (MatchesNewIgnoreLexemes(character))
                return true;            

            return false;
        }

        private bool MatchesNewLexemes(char character)
        {
            var newLexemes = new List<ILexeme>();
            var anyLexemeScanned = false;
            foreach ( var lexerRule in ParseEngine.GetExpectedLexerRules())
            {
                var lexeme = CreateLexemeForLexerRule(lexerRule);
                if (lexeme.Scan(character))
                {
                    anyLexemeScanned = true;
                    newLexemes.Add(lexeme);
                }
            }

            if (!anyLexemeScanned)
                return false;

            _existingLexemes = newLexemes;
            return true;
        }

        private bool MatchesExistingIgnoreLexemes(char character)
        {
            if (_ignoreLexemes.IsNullOrEmpty())
                return false;

            var matchedIgnoreLexemes = new List<ILexeme>();
            var anyMatchedIgnoreLexemes = false;
            foreach (var existingLexeme in _ignoreLexemes)
            {
                if (existingLexeme.Scan(character))
                {
                    matchedIgnoreLexemes.Add(existingLexeme);
                    anyMatchedIgnoreLexemes = true;
                }
            }
            return anyMatchedIgnoreLexemes;
        }

        private bool MatchesNewIgnoreLexemes(char character)
        {
            var ignoreLexerRules = ParseEngine.Grammar.Ignores;
            if (ignoreLexerRules.IsNullOrEmpty())
                return false;
            var matchingIgnoreLexemes = new List<ILexeme>();
            var anyMatchingIgnoreLexemes = false;
            foreach (var ignoreLexerRule in ignoreLexerRules)
            {
                var lexeme = CreateLexemeForLexerRule(ignoreLexerRule);
                if (lexeme.Scan(character))
                {
                    matchingIgnoreLexemes.Add(lexeme);
                    anyMatchingIgnoreLexemes = true;
                }
            }

            if (anyMatchingIgnoreLexemes)
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
            var matchedLexemes = new List<ILexeme>();
            var anyMatchedLexemes = false;
            foreach (var existingLexeme in _existingLexemes)
            {
                if (existingLexeme.Scan(character))
                {
                    matchedLexemes.Add(existingLexeme);
                    anyMatchedLexemes = true;
                }
            }
            if (!anyMatchedLexemes)
                return false;
            _existingLexemes = matchedLexemes;
            return true;
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
