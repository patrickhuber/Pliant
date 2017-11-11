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

    public class MemoryEfficientParseRunner : IParseRunner
    {
        private TextReader _reader;
        private readonly ILexemeFactoryRegistry _lexemeFactoryRegistry;
        private List<ILexeme> _tokenLexemes;
        private List<ILexeme> _ignoreLexemes;
        private List<ILexeme> _previousTokenLexemes;
        private List<ILexeme> _triviaAccumulator;
        private List<ILexeme> _triviaLexemes;

        public int Position { get; private set; }

        public int Line { get; private set; }

        public int Column { get; private set; }

        public IParseEngine ParseEngine { get; private set; }

        public MemoryEfficientParseRunner(IParseEngine parseEngine, string input)
            : this(parseEngine, new StringReader(input))
        {
        }

        public MemoryEfficientParseRunner(IParseEngine parseEngine, TextReader reader)
        {
            ParseEngine = parseEngine;
            _reader = reader;
            _tokenLexemes = new List<ILexeme>();
            _ignoreLexemes = new List<ILexeme>();
            _triviaLexemes = new List<ILexeme>();
            _triviaAccumulator = new List<ILexeme>();
            _lexemeFactoryRegistry = new LexemeFactoryRegistry();
            RegisterDefaultLexemeFactories(_lexemeFactoryRegistry);
            Position = 0;
        }

        public bool Read()
        {
            if (EndOfStream())
                return false;

            var character = ReadCharacter();
            UpdatePositionMetrics(character);

            if (MatchesExistingIncompleteIgnoreLexemes(character))
                return true;

            if (MatchesExistingIncompleteTriviaLexemes(character))
                return true;

            if (MatchExistingTokenLexemes(character))
            {
                if (EndOfStream())
                    return TryParseExistingToken();
                return true;
            }

            if (AnyExistingTokenLexemes())
                if (!TryParseExistingToken())
                    return false;

            if (MatchesNewTokenLexemes(character))
            {
                if (!EndOfStream())
                {
                    if (AnyExistingTriviaLexemes())
                        AccumulateAcceptedTrivia();
                    return true;
                }
                return TryParseExistingToken();
            }

            if (MatchesExistingTriviaLexemes(character))
            {
                if (EndOfStream() || IsEndOfLineCharacter(character))
                {
                    AccumulateAcceptedTrivia();
                    AddTrailingTriviaToPreviousToken();
                }
                return true;
            }

            if (AnyExistingTriviaLexemes())
                AccumulateAcceptedTrivia();

            if (MatchesExistingIgnoreLexemes(character))
                return true;

            ClearExistingIgnoreLexemes();

            if (MatchesNewTriviaLexemes(character))
            {
                if (IsEndOfLineCharacter(character))
                {
                    AccumulateAcceptedTrivia();
                    AddTrailingTriviaToPreviousToken();
                }
                return true;
            }

            return MatchesNewIgnoreLexemes(character);
        }

        private bool AnyExistingTriviaLexemes()
        {
            return _triviaLexemes.Count > 0;
        }

        private void AddTrailingTriviaToPreviousToken()
        {
            if (_previousTokenLexemes == null
                || _previousTokenLexemes.Count == 0)
                return;

            for (var a = 0; a < _triviaAccumulator.Count; a++)
                for (var l = 0; l < _previousTokenLexemes.Count; l++)
                    _previousTokenLexemes[l].AddTrailingTrivia(_triviaAccumulator[a]);

            _triviaLexemes.Clear();
            _triviaAccumulator.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdatePositionMetrics(char character)
        {
            Position++;
            if (IsEndOfLineCharacter(character))
            {
                Column = 0;
                Line++;
            }
            else
            {
                Column++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsEndOfLineCharacter(char character)
        {
            switch (character)
            {
                case '\n':
                    return true;
                default:
                    return false;
            }
        }

        public bool EndOfStream()
        {
            return _reader.Peek() == -1;
        }

        public bool RunToEnd()
        {
            while (!EndOfStream())
                if (!Read())
                    return false;
            return ParseEngine.IsAccepted();
        }

        private char ReadCharacter()
        {
            var character = (char)_reader.Read();
            return character;
        }

        private bool MatchesNewTriviaLexemes(char character)
        {
            var matches = MatchLexerRules(character, ParseEngine.Grammar.Trivia);
            if (matches == null)
                return false;
            SharedPools.Default<List<ILexeme>>().ClearAndFree(_triviaLexemes);
            _triviaLexemes = matches;
            return true;
        }

        private bool MatchesExistingIncompleteTriviaLexemes(char character)
        {
            return MatchesExistingIncompleteLexemes(character, _triviaLexemes);
        }

        private bool MatchesExistingTriviaLexemes(char character)
        {
            return MatchesExistingLexemes(character, _triviaLexemes);
        }

        private void AccumulateAcceptedTrivia()
        {
            for (var i = 0; i < _triviaLexemes.Count; i++)
            {
                var trivia = _triviaLexemes[i];
                if (trivia.IsAccepted())
                    _triviaAccumulator.Add(trivia);
            }
            _triviaLexemes.Clear();
        }

        private bool MatchesExistingIncompleteIgnoreLexemes(char character)
        {
            return MatchesExistingIncompleteLexemes(character, _ignoreLexemes);
        }

        private bool MatchExistingTokenLexemes(char character)
        {
            return MatchesExistingLexemes(character, _tokenLexemes);
        }

        private bool TryParseExistingToken()
        {
            var listPool = SharedPools.Default<List<ILexeme>>();

            List<ILexeme> matches = null;
            List<ILexeme> misses = null;

            for (int i = 0; i < _tokenLexemes.Count; i++)
            {
                var lexeme = _tokenLexemes[i];
                if (lexeme.IsAccepted())
                {
                    if (matches == null)
                        matches = listPool.AllocateAndClear();
                    matches.Add(lexeme);
                }
                else
                {
                    if (misses == null)
                        misses = listPool.AllocateAndClear();
                    misses.Add(lexeme);
                }
            }

            if (matches == null)
            {
                if (misses != null)
                    listPool.ClearAndFree(misses);
                return false;
            }

            if (!ParseEngine.Pulse(matches))
            {
                listPool.ClearAndFree(matches);
                if (misses != null)
                    listPool.ClearAndFree(misses);
                return false;
            }


            if (misses != null)
            {
                listPool.ClearAndFree(misses);
                ClearLexemes(misses);
            }
            _tokenLexemes.Clear();

            for (var i = 0; i < _triviaAccumulator.Count; i++)
                for (var j = 0; j < matches.Count; j++)
                    matches[j].AddLeadingTrivia(_triviaAccumulator[i]);

            _triviaAccumulator.Clear();
            _previousTokenLexemes = matches;

            return true;
        }

        private bool MatchesNewTokenLexemes(char character)
        {
            var lexerRules = ParseEngine.GetExpectedLexerRules();
            var matches = MatchLexerRules(character, lexerRules);
            if (matches == null)
                return false;

            SharedPools.Default<List<ILexeme>>().ClearAndFree(_tokenLexemes);
            _tokenLexemes = matches;

            return true;
        }

        private bool MatchesExistingIgnoreLexemes(char character)
        {
            return MatchesExistingLexemes(character, _ignoreLexemes);
        }

        private void ClearExistingIgnoreLexemes()
        {
            ClearLexemes(_ignoreLexemes);
        }

        private bool MatchesNewIgnoreLexemes(char character)
        {
            var matches = MatchLexerRules(character, ParseEngine.Grammar.Ignores);
            if (matches == null)
                return false;

            SharedPools.Default<List<ILexeme>>().ClearAndFree(_ignoreLexemes);
            _ignoreLexemes = matches;

            return true;
        }

        private List<ILexeme> MatchLexerRules(char character, IReadOnlyList<ILexerRule> lexerRules)
        {
            var pool = SharedPools.Default<List<ILexeme>>();

            // defer creation of matches until one match is made
            List<ILexeme> matches = null;

            for (var i = 0; i < lexerRules.Count; i++)
            {
                var lexerRule = lexerRules[i];
                var factory = _lexemeFactoryRegistry.Get(lexerRule.LexerRuleType);
                var lexeme = factory.Create(lexerRule, Position);

                if (!lexeme.Scan(character))
                {
                    FreeLexeme(lexeme);
                    continue;
                }

                if (matches == null)
                    matches = pool.AllocateAndClear();

                matches.Add(lexeme);
            }

            if (matches == null)
                return null;

            if (matches.Count == 0)
            {
                pool.ClearAndFree(matches);
                return null;
            }

            return matches;
        }

        private bool MatchesExistingLexemes(char character, List<ILexeme> lexemes)
        {
            var anyLexemes = lexemes != null && lexemes.Count > 0;
            if (!anyLexemes)
                return false;

            var i = 0;
            var size = lexemes.Count;

            while (i < size)
            {
                var lexeme = lexemes[i];
                if (lexeme.Scan(character))
                    i++;
                else
                {
                    if (i < size - 1)
                    {
                        var temp = lexemes[i];
                        lexemes[i] = lexemes[size - 1];
                        lexemes[size - 1] = temp;
                    }
                    size--;
                }
            }

            var anyMatches = size > 0;
            if (!anyMatches)
                return false;

            i = lexemes.Count - 1;
            while (i >= size)
            {
                FreeLexeme(lexemes[i]);
                lexemes.RemoveAt(i);
                i--;
            }
            return true;
        }
        
        private bool MatchesExistingIncompleteLexemes(char character, List<ILexeme> lexemes)
        {
            var anyLexemes = lexemes != null && lexemes.Count > 0;
            if (!anyLexemes)
                return false;

            var i = 0;
            var size = lexemes.Count;

            while (i < size)
            {
                var lexeme = lexemes[i];
                if (!lexeme.IsAccepted() && lexeme.Scan(character))
                    i++;
                else
                {
                    if (i < size - 1)
                    {
                        var temp = lexemes[i];
                        lexemes[i] = lexemes[size - 1];
                        lexemes[size - 1] = temp;
                    }
                    size--;
                }
            }

            var anyMatches = size > 0;
            if (!anyMatches)
                return false;

            i = lexemes.Count - 1;
            while (i >= size)
            {
                FreeLexeme(lexemes[i]);
                lexemes.RemoveAt(i);
                i--;
            }
            return true;
        }

        private void ClearLexemes(List<ILexeme> lexemes)
        {
            for (var i = 0; i < lexemes.Count; i++)
                FreeLexeme(lexemes[i]);
            lexemes.Clear();
        }

        private void FreeLexeme(ILexeme lexeme)
        {
            var lexemeFactory = _lexemeFactoryRegistry.Get(lexeme.LexerRule.LexerRuleType);
            lexemeFactory.Free(lexeme);
        }

        private bool AnyExistingTokenLexemes()
        {
            return _tokenLexemes.Count > 0;
        }

        private static void RegisterDefaultLexemeFactories(ILexemeFactoryRegistry lexemeFactoryRegistry)
        {
            lexemeFactoryRegistry.Register(new TerminalLexemeFactory());
            lexemeFactoryRegistry.Register(new ParseEngineLexemeFactory());
            lexemeFactoryRegistry.Register(new StringLiteralLexemeFactory());
            lexemeFactoryRegistry.Register(new DfaLexemeFactory());
        }

    }
}