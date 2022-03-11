using System.IO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Pliant.Automata;
using Pliant.Tokens;
using Pliant.Grammars;
using System.Text;
using Pliant.Captures;
using System.Linq;

namespace Pliant.Runtime
{
    public class ParseRunner : IParseRunner
    {
        private readonly ICapture<char> _capture;
        private readonly StringBuilder _builder;
        private readonly TextReader _reader;
        private readonly ILexemeFactoryRegistry _lexemeFactoryRegistry;
        private readonly List<ILexeme> _tokenLexemes;
        private readonly List<ILexeme> _ignoreLexemes;        
        private readonly List<ILexeme> _triviaAccumulator;
        private readonly List<ILexeme> _triviaLexemes;
        private readonly List<ErrorToken> _currentErrorTokens;
        private readonly List<ErrorToken> _errors;
        public IReadOnlyList<ErrorToken> Errors => _errors;

        private List<ILexeme> _previousTokenLexemes;

        public int Position { get; private set; }
                
        public int Line { get; private set; }
                
        public int Column { get; private set; }

        public IParseEngine ParseEngine { get; private set; }

        public ParseRunnerOptions Options { get; private set; }

        public ParseRunner(IParseEngine parseEngine, string input)
            : this(parseEngine, new StringReader(input))
        {
        }

        public ParseRunner(IParseEngine parseEngine, string input, ParseRunnerOptions options)
            : this(parseEngine, new StringReader(input), options)
        {
        }

        public ParseRunner(IParseEngine parseEngine, TextReader reader)
            : this(parseEngine, reader, new ParseRunnerOptions())
        { 
        }

        public ParseRunner(IParseEngine parseEngine, TextReader reader, ParseRunnerOptions options)
        {
            ParseEngine = parseEngine;
            Options = options;

            _reader = reader;
            _tokenLexemes = new List<ILexeme>();
            _ignoreLexemes = new List<ILexeme>();
            _triviaLexemes = new List<ILexeme>();
            _triviaAccumulator = new List<ILexeme>();
            _currentErrorTokens = new List<ErrorToken>();
            _errors = new List<ErrorToken>();
            _lexemeFactoryRegistry = new LexemeFactoryRegistry();
            _builder = new StringBuilder();
            _capture = new StringBuilderCapture(_builder);            

            RegisterDefaultLexemeFactories(_lexemeFactoryRegistry);
            Position = -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// <see cref="ReadStatus.Success"/> if the operation succeeds
        /// <see cref="ReadStatus.Failure"/> if trying to read past the end of the stream or the operation could not recover
        /// <see cref="ReadStatus.Recovery"/> if the operation failed but was recovered
        /// </returns>
        public bool Read()
        {
            if (EndOfStream())
                return false;

            var character = ReadCharacter();
            UpdatePositionMetrics(character);

            if (MatchesExistingIncompleteIgnoreLexemes())
                return true;

            if (MatchesExistingIncompleteTriviaLexemes())
                return true;

            if (MatchExistingTokenLexemes())
            {
                if (EndOfStream())
                    return CleanUp();

                return true;
            }

            if (AnyExistingTokenLexemes())
                if (!TryParseExistingToken())
                    return HandleError();

            if (MatchesNewTokenLexemes())
            {
                Recover();

                if (EndOfStream())
                    return CleanUp();
                
                if (AnyExistingTriviaLexemes())
                    AccumulateAcceptedTrivia();
                return true;
            }

            if (MatchesExistingTriviaLexemes())
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

            if (MatchesExistingIgnoreLexemes())
                return true;

            ClearExistingIgnoreLexemes();

            if (MatchesNewTriviaLexemes() || MatchesNewIgnoreLexemes() || HandleError())
                Recover();

            if (!IsEndOfLineCharacter(character))
                return true;

            AccumulateAcceptedTrivia();
            AddTrailingTriviaToPreviousToken();

            return true;
        }

        /// <summary>
        /// Cleanup adjusts the state of the parser if there are hanging tokens or the parse engine is in an unaccepted state.
        /// </summary>
        /// <returns></returns>
        private bool CleanUp()
        {            
            if (!TryParseExistingToken())
                return false;
                        
            if (ParseEngine.IsAccepted())                
                return true;

            if (!HandleError())
                return false;

            Recover();
            return true;
        }
                

        /// <summary>
        /// HandleError puts the current expected lexer rules in the parser as error tokens. 
        /// </summary>
        /// <returns></returns>
        private bool HandleError()
        {
            if (!Options.EnableErrorRecovery)
                return true;

            if (_currentErrorTokens.Count != 0)
            {
                AdvanceErrors();
                return true;
            }

            var expectedLexerRules = ParseEngine.GetExpectedLexerRules();
            for (var r = 0; r < expectedLexerRules.Count; r++)
            {
                var expectedLexerRule = expectedLexerRules[r];
                var errorToken = new ErrorToken(expectedLexerRule.TokenType, _capture, Position);
                _currentErrorTokens.Add(errorToken);
            }
                        
            // advance the parse if these are missing tokens
            return ParseEngine.Errors(_currentErrorTokens);
        }

        private void Recover()
        {
            if (!Options.EnableErrorRecovery)
                return;
            _errors.AddRange(_currentErrorTokens);
            _currentErrorTokens.Clear();
        }

        private void AdvanceErrors()
        {
            if (!Options.EnableErrorRecovery)
                return;
            for (var e = 0; e < _currentErrorTokens.Count; e++)
            {
                var error = _currentErrorTokens[e];
                error.Scan();
            }
        }

        public bool ReadV0()
        {
            if (EndOfStream())
                return false;

            var character = ReadCharacter();
            UpdatePositionMetrics(character);

            if (MatchesExistingIncompleteIgnoreLexemes())
                return true;

            if (MatchesExistingIncompleteTriviaLexemes())
                return true;

            if (MatchExistingTokenLexemes())
            {
                if (EndOfStream())
                    return TryParseExistingToken();
                return true;
            }

            if (AnyExistingTokenLexemes())
                if (!TryParseExistingToken())
                    return false;

            if (MatchesNewTokenLexemes())
            {
                if (!EndOfStream())
                {
                    if (AnyExistingTriviaLexemes())
                        AccumulateAcceptedTrivia();
                    return true;
                }
                return TryParseExistingToken();
            }

            if (MatchesExistingTriviaLexemes())
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

            if (MatchesExistingIgnoreLexemes())
                return true;

            ClearExistingIgnoreLexemes();

            if (!MatchesNewTriviaLexemes())
                return MatchesNewIgnoreLexemes();

            if (!IsEndOfLineCharacter(character))            
                return true;
            
            AccumulateAcceptedTrivia();
            AddTrailingTriviaToPreviousToken();

            return true;
        }

        private bool AnyExistingTriviaLexemes()
        {
            return _triviaLexemes.Count > 0;
        }

        private void AddTrailingTriviaToPreviousToken()
        {
            if (_previousTokenLexemes is null
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
            if (IsEndOfLineCharacter(character))
            {
                Column = 0;
                Line++;
            }
            else            
                Column++;

            Position++;
        }

        private static bool IsEndOfLineCharacter(char c)
        {            
            return c == '\n';
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
            _builder.Append(character);
            return character;
        }

        private bool MatchesNewTriviaLexemes()
        {
            return MatchLexerRules(ParseEngine.Grammar.Trivia, _triviaLexemes);
        }

        private bool MatchesExistingIncompleteTriviaLexemes()
        {
            return MatchesExistingIncompleteLexemes(_triviaLexemes);
        }

        private bool MatchesExistingTriviaLexemes()
        {
            return MatchesExistingLexemes(_triviaLexemes);
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

        private bool MatchesExistingIncompleteIgnoreLexemes()
        {
            return MatchesExistingIncompleteLexemes(_ignoreLexemes);
        }

        private bool MatchExistingTokenLexemes()
        {
            return MatchesExistingLexemes(_tokenLexemes);
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
            if (_previousTokenLexemes is null)
                _previousTokenLexemes = new List<ILexeme>(_tokenLexemes);
            else
            {
                _previousTokenLexemes.Clear();
                _previousTokenLexemes.AddRange(_tokenLexemes);
            }                

            _tokenLexemes.Clear();

            return true;
        }

        private bool MatchesNewTokenLexemes()
        {
            return MatchLexerRules(ParseEngine.GetExpectedLexerRules(), _tokenLexemes);
        }

        private bool MatchesExistingIgnoreLexemes()
        {
            return MatchesExistingLexemes(_ignoreLexemes);
        }

        private void ClearExistingIgnoreLexemes()
        {
            ClearLexemes(_ignoreLexemes);
        }

        private bool MatchesNewIgnoreLexemes()
        {
            return MatchLexerRules(ParseEngine.Grammar.Ignores, _ignoreLexemes);
        }

        private bool MatchLexerRules(IReadOnlyList<ILexerRule> lexerRules, List<ILexeme> lexemes)
        {
            var anyMatches = false;
            var character = _capture[Position];
            for (var i = 0; i < lexerRules.Count; i++)
            {
                var lexerRule = lexerRules[i];
                if (!lexerRule.CanApply(character))
                    continue;
                var factory = _lexemeFactoryRegistry.Get(lexerRule.LexerRuleType);
                var lexeme = factory.Create(lexerRule, _capture, Position);
                if (!lexeme.Scan())
                {
                    FreeLexeme(lexeme);
                    continue;
                }

                if(!anyMatches)
                {
                    anyMatches = true;
                    lexemes.Clear();
                }

                lexemes.Add(lexeme);
            }
            return anyMatches;
        }

        private bool MatchesExistingLexemes(List<ILexeme> lexemes)
        {
            var anyLexemes = lexemes != null && lexemes.Count > 0;
            if (!anyLexemes)
                return false;

            var i = 0;
            var size = lexemes.Count;

            while (i < size)
            {
                var lexeme = lexemes[i];
                if (lexeme.Scan())
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

        private bool MatchesExistingIncompleteLexemes(List<ILexeme> lexemes)
        {
            var anyLexemes = lexemes != null && lexemes.Count > 0;
            if (!anyLexemes)
                return false;

            var i = 0;
            var size = lexemes.Count;

            while (i < size)
            {
                var lexeme = lexemes[i];
                if (!lexeme.IsAccepted() && lexeme.Scan())
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