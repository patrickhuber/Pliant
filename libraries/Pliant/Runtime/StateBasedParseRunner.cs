using Pliant.Automata;
using Pliant.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Pliant.Runtime
{
    public class StateBasedParseRunner : IParseRunner
    {
        public int Position { get; private set; }

        public int Line { get; private set; }

        public int Column { get; private set; }

        public IParseEngine ParseEngine { get; }

        private TextReader _reader;
        private ScanState _state;
        private readonly ILexemeFactoryRegistry _lexemeFactoryRegistry;
        private List<ILexeme> _tokenLexemes;
        private List<ILexeme> _ignoreLexemes;
        private List<ILexeme> _previousTokenLexemes;
        private List<ILexeme> _triviaAccumulator;
        private List<ILexeme> _triviaLexemes;

        public StateBasedParseRunner(IParseEngine parseEngine, TextReader textReader)
        {
            ParseEngine = parseEngine;
            _reader = textReader;
            _state = ScanState.Start;
            _tokenLexemes = new List<ILexeme>();
            _ignoreLexemes = new List<ILexeme>();
            _triviaLexemes = new List<ILexeme>();
            _triviaAccumulator = new List<ILexeme>();
            _lexemeFactoryRegistry = new LexemeFactoryRegistry();
            RegisterDefaultLexemeFactories(_lexemeFactoryRegistry);
            Position = 0;
        }
        
        public bool EndOfStream()
        {
            if (_state == ScanState.EndOfStream)
                return true;

            if (_reader.Peek() > -1)
                return false;

            _state = ScanState.EndOfStream;
            return true;
        }

        public bool Read()
        {
            if (EndOfStream())
                return false;

            var character = ReadCharacter();
            UpdatePositionMetrics(character);

            switch (_state)
            {
                case ScanState.Start:
                    return MatchAny(character);

                case ScanState.MatchingIgnore:
                    return ContinueMatchIgnore(character);

                case ScanState.MatchingTrivia:
                    return ContinueMatchTrivia(character);

                case ScanState.MatchingToken:
                    return ContinueMatchToken(character);                
            }

            return false;
        }
        
        private char ReadCharacter()
        {
            var character = (char)_reader.Read();
            return character;
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

        private bool MatchAny(char character)
        {
            if (MatchesNewTokenLexemes(character))
            {
                if (EndOfStream())
                {
                    _state = ScanState.EndOfStream;
                    return TryParseExistingToken();
                }
                if (AnyExistingTriviaLexemes())
                    AccumulateAcceptedTrivia();
                _state = ScanState.MatchingToken;
                return true;                
            }

            if (MatchesNewTriviaLexemes(character))
            {
                if (IsEndOfLineCharacter(character))
                {
                    AccumulateAcceptedTrivia();
                    AddTrailingTriviaToPreviousToken();
                }
                _state = ScanState.MatchingTrivia;
                return true;
            }

            if (MatchesNewIgnoreLexemes(character))
            {
                _state = ScanState.MatchingIgnore;
                return true;
            }

            return false;
        }

        private bool TryParseExistingToken()
        {
            throw new NotImplementedException();
        }

        private bool AnyExistingTriviaLexemes()
        {
            throw new NotImplementedException();
        }

        private void AccumulateAcceptedTrivia()
        {
            throw new NotImplementedException();
        }

        private void AddTrailingTriviaToPreviousToken()
        {
            throw new NotImplementedException();
        }

        private bool MatchesNewTriviaLexemes(char character)
        {
            throw new NotImplementedException();
        }

        private bool MatchesNewIgnoreLexemes(char character)
        {
            throw new NotImplementedException();
        }

        private bool MatchesNewTokenLexemes(char character)
        {
            throw new NotImplementedException();
        }

        private bool ContinueMatchIgnore(char character)
        {
            return false;
        }

        private bool ContinueMatchTrivia(char character)
        {
            return false;
        }

        private bool ContinueMatchToken(char character)
        {
            return false;
        }

        public bool RunToEnd()
        {
            while (!EndOfStream())
            {
                if (!Read())
                    return false;
            }
            return ParseEngine.IsAccepted();
        }

        private enum ScanState
        {
            Start,
            MatchingToken,
            MatchingIgnore,
            MatchingTrivia,
            EndOfStream
        }
        
        private void RegisterDefaultLexemeFactories(ILexemeFactoryRegistry lexemeFactoryRegistry)
        {
            lexemeFactoryRegistry.Register(new TerminalLexemeFactory());
            lexemeFactoryRegistry.Register(new ParseEngineLexemeFactory());
            lexemeFactoryRegistry.Register(new StringLiteralLexemeFactory());
            lexemeFactoryRegistry.Register(new DfaLexemeFactory());
        }
    }
}
