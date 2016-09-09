using Pliant.Automata;
using Pliant.Collections;
using Pliant.Grammars;
using Pliant.Lexemes;
using Pliant.Tokens;
using Pliant.Utilities;
using System.Collections.Generic;
using System.IO;

namespace Pliant.Runtime
{
    public class ParseRunner : IParseRunner
    {
        private List<ILexeme> _existingLexemes;
        private List<ILexeme> _ignoreLexemes;
        private readonly ILexemeFactoryRegistry _lexemeFactoryRegistry;

        private readonly TextReader _textReader;

        public IParseEngine ParseEngine { get; private set; }

        public int Position { get; private set; }
        
        public ParseRunner(IParseEngine parseEngine, string input)
            : this(parseEngine, new StringReader(input))
        {
        }

        public ParseRunner(IParseEngine parseEngine, TextReader input)
        {
            _textReader = input;

            _lexemeFactoryRegistry = new LexemeFactoryRegistry();
            RegisterDefaultLexemeFactories(_lexemeFactoryRegistry);

            _ignoreLexemes = new List<ILexeme>();
            _existingLexemes = new List<ILexeme>();

            Position = 0;
            ParseEngine = parseEngine;
        }

        public bool EndOfStream()
        {
            return _textReader.Peek() == -1;
        }

        public bool Read()
        {
            if (EndOfStream())
                return false;

            var character = ReadCharacter();

            if (MatchesExistingIncompleteIgnoreLexemes(character))
                return true;
            
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

            ClearExistingIngoreLexemes();

            return MatchesNewIgnoreLexemes(character);
        }

        private static void RegisterDefaultLexemeFactories(ILexemeFactoryRegistry lexemeFactoryRegistry)
        {
            lexemeFactoryRegistry.Register(new TerminalLexemeFactory());
            lexemeFactoryRegistry.Register(new ParseEngineLexemeFactory());
            lexemeFactoryRegistry.Register(new StringLiteralLexemeFactory());
            lexemeFactoryRegistry.Register(new DfaLexemeFactory());
        }
        private bool AnyExistingLexemes()
        {
            return _existingLexemes.Count > 0;
        }

        private void ClearExistingIngoreLexemes()
        {
            _ignoreLexemes.Clear();
        }
        
        private ILexemeFactory GetLexemeFactory(ILexerRule lexerRule)
        {
            return _lexemeFactoryRegistry
                .Get(lexerRule.LexerRuleType);
        }

        private bool MatchesExistingIgnoreLexemes(char character)
        {
            if (!AnyExistingIngoreLexemes())
                return false;

            var anyMatchedIgnoreLexemes = false;
            for(int i=0;i<_ignoreLexemes.Count;i++)
            {
                var existingLexeme = _ignoreLexemes[i];
                if (existingLexeme.Scan(character))
                {
                    anyMatchedIgnoreLexemes = true;
                }
            }
            return anyMatchedIgnoreLexemes;
        }

        private bool AnyExistingIngoreLexemes()
        {
            return _ignoreLexemes.Count != 0;
        }

        private bool MatchesExistingIncompleteIgnoreLexemes(char character)
        {
            if (!AnyExistingIngoreLexemes())
                return false;

            var anyMatchedIgnoreLexemes = false;
            for (int i = 0; i < _ignoreLexemes.Count; i++)
            {
                var existingLexeme = _ignoreLexemes[i];
                if (!existingLexeme.IsAccepted() && existingLexeme.Scan(character))
                    anyMatchedIgnoreLexemes = true;
            }
            return anyMatchedIgnoreLexemes;
        }

        private bool MatchesExistingLexemes(char character)
        {
            if (!AnyExistingLexemes())
                return false;

            List<ILexeme> matchedLexemes = null;

            for (int i=0;i<_existingLexemes.Count;i++)
            {
                var existingLexeme = _existingLexemes[i];
                if (!existingLexeme.Scan(character))
                {
                    var factory = GetLexemeFactory(existingLexeme.LexerRule);
                    factory.Free(existingLexeme);
                    continue;
                }
                
                if(matchedLexemes == null)
                    matchedLexemes = SharedPools.Default<List<ILexeme>>().AllocateAndClear();

                matchedLexemes.Add(existingLexeme);      
            }

            if (matchedLexemes == null)
                return false;

            if (matchedLexemes.Count == 0)
            {
                SharedPools.Default<List<ILexeme>>().Free(matchedLexemes);
                return false;
            }

            SharedPools.Default<List<ILexeme>>().ClearAndFree(_existingLexemes);
            _existingLexemes = matchedLexemes;
            return true;
        }
        
        private bool MatchesNewIgnoreLexemes(char character)
        {
            if (ParseEngine.Grammar.Ignores.Count == 0)
                return false;

            List<ILexeme> matchingIgnoreLexemes = null;
            
            for (int i = 0; i < ParseEngine.Grammar.Ignores.Count; i++)
            {
                var ignoreLexerRule = ParseEngine.Grammar.Ignores[i];
                var lexemeFactory = GetLexemeFactory(ignoreLexerRule);
                var lexeme = lexemeFactory.Create(ignoreLexerRule);
                if (!lexeme.Scan(character))
                {
                    lexemeFactory.Free(lexeme);
                    continue;
                }
                if(matchingIgnoreLexemes == null)
                    matchingIgnoreLexemes = SharedPools.Default<List<ILexeme>>().AllocateAndClear();
                matchingIgnoreLexemes.Add(lexeme);
            }

            if (matchingIgnoreLexemes == null)
                return false;

            if (matchingIgnoreLexemes.Count == 0)
            {
                SharedPools.Default<List<ILexeme>>().Free(matchingIgnoreLexemes);
                return false;
            }

            SharedPools.Default<List<ILexeme>>().ClearAndFree(_ignoreLexemes);
            _ignoreLexemes = matchingIgnoreLexemes;
            return true;
        }

        private bool MatchesNewLexemes(char character)
        {
            List<ILexeme> newLexemes = null; 
            
            var expectedLexerRules = ParseEngine.GetExpectedLexerRules();
            // PERF: Avoid foreach due to boxing IEnumerable<T>

            for (var l = 0; l< expectedLexerRules.Count; l++)
            {
                var lexerRule = expectedLexerRules[l];
                var lexemeFactory = GetLexemeFactory(lexerRule);
                var lexeme = lexemeFactory.Create(lexerRule);
                if (!lexeme.Scan(character))
                {
                    lexemeFactory.Free(lexeme);
                    continue;
                }
                if(newLexemes == null)
                    newLexemes = SharedPools.Default<List<ILexeme>>().AllocateAndClear();
                newLexemes.Add(lexeme);                
            }

            SharedPools.Default<List<ILexerRule>>().ClearAndFree(expectedLexerRules);
            expectedLexerRules = null;

            if (newLexemes == null)
                return false;

            if (newLexemes.Count == 0)
            {
                SharedPools.Default<List<ILexeme>>().Free(newLexemes);
                return false;
            }

            SharedPools.Default<List<ILexeme>>().ClearAndFree(_existingLexemes);
            _existingLexemes = newLexemes;

            return true;
        }

        private char ReadCharacter()
        {
            var character = (char)_textReader.Read();
            Position++;
            return character;
        }

        private bool TryParseExistingToken()
        {
            // PERF: Avoid Linq FirstOrDefault due to lambda allocation
            ILexeme longestAcceptedMatch = null;
            for (int i = 0; i < _existingLexemes.Count; i++)
            {
                var lexeme = _existingLexemes[i];
                if (lexeme.IsAccepted())
                {
                    longestAcceptedMatch = lexeme;
                    break;
                }
            }

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
        
        private IToken CreateTokenFromLexeme(ILexeme lexeme)
        {
            var capture = lexeme.Capture;
            return new Token(
                capture,
                Position - capture.Length - 1,
                lexeme.TokenType);
        }
        
        private void ClearExistingLexemes()
        {
            _existingLexemes.Clear();
        }
    }
}