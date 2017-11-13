using System.IO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Pliant.Automata;
using Pliant.Tokens;
using Pliant.Grammars;

namespace Pliant.Runtime
{
    public class ParseRunner : IParseRunner
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

        public ParseRunner(IParseEngine parseEngine, string input)
            : this(parseEngine, new StringReader(input))
        {
        }

        public ParseRunner(IParseEngine parseEngine, TextReader reader)
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
            return MatchLexerRules(character, ParseEngine.Grammar.Trivia, _triviaLexemes);
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
            var anyLexemes = _tokenLexemes.Count > 0;
            if (!anyLexemes)
                return false;

            var i = 0;
            var size = _tokenLexemes.Count;

            while (i < size)
            {
                var lexeme = _tokenLexemes[i];
                if (lexeme.IsAccepted())
                    i++;
                else
                {
                    if (i < size - 1)
                    {
                        _tokenLexemes[i] = _tokenLexemes[size - 1];
                        _tokenLexemes[size - 1] = lexeme;
                    }
                    size--;
                }
            }

            var anyMatches = size > 0;
            if (!anyMatches)
                return false;

            i = _tokenLexemes.Count - 1;
            while (i >= size)
            {
                FreeLexeme(_tokenLexemes[i]);
                _tokenLexemes.RemoveAt(i);
                i--;
            }

            if (!ParseEngine.Pulse(_tokenLexemes))
                return false;

            for (i = 0; i < _triviaAccumulator.Count; i++)
                for (var j = 0; j < _tokenLexemes.Count; j++)
                    _tokenLexemes[j].AddLeadingTrivia(_triviaAccumulator[i]);

            _triviaAccumulator.Clear();
            if (_previousTokenLexemes != null)
            {
                _previousTokenLexemes.Clear();
                _previousTokenLexemes.AddRange(_tokenLexemes);
            }
            else
                _previousTokenLexemes = new List<ILexeme>(_tokenLexemes);

            _tokenLexemes.Clear();

            return true;
        }

        private bool MatchesNewTokenLexemes(char character)
        {
            return MatchLexerRules(character, ParseEngine.GetExpectedLexerRules(), _tokenLexemes);
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
            return MatchLexerRules(character, ParseEngine.Grammar.Ignores, _ignoreLexemes);
        }

        private bool MatchLexerRules(char character, IReadOnlyList<ILexerRule> lexerRules, List<ILexeme> lexemes)
        {
            var anyMatches = false;
            for (var i = 0; i < lexerRules.Count; i++)
            {
                var lexerRule = lexerRules[i];
                if (!lexerRule.CanApply(character))
                    continue;
                var factory = _lexemeFactoryRegistry.Get(lexerRule.LexerRuleType);
                var lexeme = factory.Create(lexerRule, Position);

                if (!lexeme.Scan(character))
                {
                    FreeLexeme(lexeme);
                    continue;
                }

                if (!anyMatches)
                {
                    anyMatches = true;
                    lexemes.Clear();
                }

                lexemes.Add(lexeme);
            }

            return anyMatches;
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
                        lexemes[i] = lexemes[size - 1];
                        lexemes[size - 1] = lexeme;
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
                        lexemes[i] = lexemes[size - 1];
                        lexemes[size - 1] = lexeme;
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