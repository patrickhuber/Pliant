using System.IO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Pliant.Runtime;
using Pliant.Automata;
using Pliant.Utilities;
using Pliant.Tokens;
using Pliant.Grammars;
using System;

public class ParseRunner : IParseRunner
{
    private TextReader _reader;
    private readonly ILexemeFactoryRegistry _lexemeFactoryRegistry;
    private List<ILexeme> _tokenLexemes;
    private List<ILexeme> _ignoreLexemes;
    private ILexeme _previousTokenLexeme;
    private List<ILexeme> _leadingTrivia;
    private ILexeme _currentTrivia;

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
        ShiftTrivia(character);

        if (MatchesExistingIncompleteIgnoreLexemes(character))
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
                return true;
            return TryParseExistingToken();
        }

        if (MatchesExistingIgnoreLexemes(character))
            return true;

        ClearExistingIngoreLexemes();

        return MatchesNewIgnoreLexemes(character);
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
    private void ShiftTrivia(char character)
    {
        if (!IsEndOfLineCharacter(character))
            return;

        if (_currentTrivia == null)
            return;

        bool isFirstToken = _previousTokenLexeme == null;
        if (isFirstToken)
        {
            if (_leadingTrivia == null)
                _leadingTrivia = new List<ILexeme>();
            _leadingTrivia.Add(_currentTrivia);
            var matchingTrivia = MatchLexerRules(character, ParseEngine.Grammar.Trivia);
            if(matchingTrivia == null)
            {
                _currentTrivia = null;
                return;
            }
        }
        else
        { }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsEndOfLineCharacter(char character)
    {
        switch (character)
        {
            case '\n':
                return false;
            default:
                return true;
        }
    }

    public bool EndOfStream()
    {
        return _reader.Peek() == -1;
    }

    private bool MatchesExistingIncompleteIgnoreLexemes(char character)
    {
        var matches = MatchExistingIncompleteLexemes(character, _ignoreLexemes);
        if (matches != null)
        {
            SharedPools.Default<List<ILexeme>>()
                .ClearAndFree(_ignoreLexemes);
            _ignoreLexemes = matches;
            return true;
        }
        return false;
    }
    
    private char ReadCharacter()
    {
        var character = (char)_reader.Read();
        Position++;
        return character;
    }

    private bool MatchExistingTokenLexemes(char character)
    {
        var matches = MatchExistingLexemes(character, _tokenLexemes);
        if (matches == null)
            return false;
        SharedPools.Default<List<ILexeme>>().ClearAndFree(_tokenLexemes);
        _tokenLexemes = matches;
        return true;
    }

    private bool TryParseExistingToken()
    {
        // PERF: Avoid Linq FirstOrDefault due to lambda allocation
        ILexeme longestAcceptedMatch = null;
        var doNotFreeLexemeIndex = -1;
        for (int i = 0; i < _tokenLexemes.Count; i++)
        {
            var lexeme = _tokenLexemes[i];
            if (lexeme.IsAccepted())
            {
                doNotFreeLexemeIndex = i;
                longestAcceptedMatch = lexeme;
                break;
            }
        }

        if (longestAcceptedMatch == null)
            return false;
        
        if (!ParseEngine.Pulse(longestAcceptedMatch))
            return false;
        _previousTokenLexeme = longestAcceptedMatch;

        ClearTokenLexemes(doNotFreeLexemeIndex);

        return true;
    }
    
    private void ClearTokenLexemes(int doNotFreeLexemeIndex)
    {
        ClearLexemes(_tokenLexemes, doNotFreeLexemeIndex);
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
        var matches = MatchExistingLexemes(character, _ignoreLexemes);
        if (matches == null)
        {
            return false;
        }

        SharedPools.Default<List<ILexeme>>().ClearAndFree(_ignoreLexemes);
        _ignoreLexemes = matches;
        return true;
    }
    
    private void ClearExistingIngoreLexemes()
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

    private List<ILexeme> MatchExistingLexemes(char character, List<ILexeme> lexemes)
    {
        var anyLexemes = lexemes.Count > 0;
        if (!anyLexemes)
            return null;

        var pool = SharedPools.Default<List<ILexeme>>();

        var matches = pool.AllocateAndClear();
        var misses = pool.AllocateAndClear();

        // partition the existing lexemes into misses and matches
        // by scanning the current character
        // we need two collections because the original _existingLexemes collection
        // needs to be preserved. Agressively freeing here would break that constraint. 
        for (var i = 0; i < lexemes.Count; i++)
        {
            var lexeme = lexemes[i];
            if (lexeme.Scan(character))
                matches.Add(lexeme);
            else
                misses.Add(lexeme);
        }

        // If there were no matches return and free the collections lexemes needs to be preserved.
        // Do not free each individual missed lexeme as this will destroy the lexemes integrity
        if (matches.Count == 0)
        {
            pool.ClearAndFree(matches);
            pool.ClearAndFree(misses);
            return null;
        }

        // remove any missed lexemes returning them to the lexeme pool
        // we know that we have at least one match at this point so the integrity of the lexems collection
        // can be broken
        for (var i = 0; i < misses.Count; i++)
            FreeLexeme(misses[i]);

        // free the collection as it is no longer used
        pool.ClearAndFree(misses);

        // return the matched items collection
        return matches;
    }

    private List<ILexeme> MatchExistingIncompleteLexemes(char character, List<ILexeme> lexemes)
    {
        var anyLexemes = lexemes.Count > 0;
        if (!anyLexemes)
            return null;

        // partition the lexemes into missing and matching collections
        // we will recycle the missing collection and use the matching collection as the 
        // return value
        var pool = SharedPools.Default<List<ILexeme>>();
        var matches = pool.AllocateAndClear();
        var misses = pool.AllocateAndClear();

        for (var i = 0; i < lexemes.Count; i++)
        {
            var lexeme = lexemes[i];
            if (!lexeme.IsAccepted())
            {
                if (lexeme.Scan(character))
                    matches.Add(lexeme);
                else
                    misses.Add(lexeme);
            }
        }

        // if no matches, cleanup the missing and matching collections and return null
        if (matches.Count == 0)
        {
            pool.ClearAndFree(matches);
            pool.ClearAndFree(misses);
            return null;
        }

        // remove any missed lexemes returning them to the lexeme pool
        // we know that we have at least one match at this point so the integrity of the lexems collection
        // can be broken
        for (var i = 0; i < misses.Count; i++)
            FreeLexeme(misses[i]);

        // free the collection as it is no longer used
        pool.ClearAndFree(misses);

        return matches;
    }

    private void ClearLexemes(List<ILexeme> lexemes, int doNotFreeLexemeIndex = -1)
    {
        for (var i = 0; i < lexemes.Count; i++)
            if(i != doNotFreeLexemeIndex)
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