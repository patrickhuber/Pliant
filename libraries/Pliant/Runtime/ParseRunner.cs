using System;
using System.IO;
using Pliant.Runtime;

using Pliant.Automata;
using System.Collections.Generic;
using Pliant.Utilities;
using Pliant.Tokens;
using Pliant.Grammars;

public class ParseRunner : IParseRunner
{
    private TextReader _reader;
    private readonly ILexemeFactoryRegistry _lexemeFactoryRegistry;
    private List<ILexeme> _existingLexemes;
    private List<ILexeme> _ignoreLexemes;

    public int Position { get; private set; }

    public IParseEngine ParseEngine { get; private set; }

    public ParseRunner(IParseEngine parseEngine, string input)
        : this(parseEngine, new StringReader(input))
    {
    }

    public ParseRunner(IParseEngine parseEngine, TextReader reader)
    {
        ParseEngine = parseEngine;
        _reader = reader;
        _existingLexemes = new List<ILexeme>();
        _ignoreLexemes = new List<ILexeme>();
        _lexemeFactoryRegistry = new LexemeFactoryRegistry();
        RegisterDefaultLexemeFactories(_lexemeFactoryRegistry);
        Position = 0;
    }

    public bool Read(IParseContext context)
    {
        if (EndOfStream())
            return false;
        
        //Ensure we have a parse context
        if (context == null)
            context = new ParseContext();
        
        var character = ReadCharacter(context);

        if (MatchesExistingIncompleteIgnoreLexemes(context, character))
            return true;

        if (MatchExistingLexemes(context, character))
        {
            if (EndOfStream())
                return TryParseExistingToken(context);
            return true;
        }

        if (AnyExistingLexemes())
            if (!TryParseExistingToken(context))
                return false;

        if (MatchesNewLexemes(context, character))
        {
            if (!EndOfStream())
                return true;
            return TryParseExistingToken(context);
        }

        if (MatchesExistingIgnoreLexemes(context, character))
            return true;

        ClearExistingIngoreLexemes();

        return MatchesNewIgnoreLexemes(context, character);
    }

    public bool EndOfStream()
    {
        return _reader.Peek() == -1;
    }

    private bool MatchesExistingIncompleteIgnoreLexemes(ILexContext lexContext, char character)
    {
        if (!AnyExistingIngoreLexemes())
            return false;

        var pool = SharedPools.Default<List<ILexeme>>();
        var matches = pool.AllocateAndClear();

        for (var i = 0; i < _ignoreLexemes.Count; i++)
        {
            var lexeme = _ignoreLexemes[i];
            if (!lexeme.IsAccepted())
            {
                if (lexeme.Scan(lexContext, character))
                    matches.Add(lexeme);
                else
                    FreeLexeme(lexeme);
            }
        }

        if (matches.Count == 0)
        {
            pool.ClearAndFree(matches);
            return false;
        }

        pool.ClearAndFree(_ignoreLexemes);
        _ignoreLexemes = matches;
        return true;
    }

    private bool AnyExistingIngoreLexemes()
    {
        return _ignoreLexemes.Count > 0;
    }

    private char ReadCharacter(ILexContext lexContext)
    {
        var character = (char)_reader.Read();
        Position++;

        lexContext.ReadCharacter(Position, character);

        return character;
    }

    private bool MatchExistingLexemes(ILexContext lexContext, char character)
    {
        if (!AnyExistingLexemes())
            return false;

        var pool = SharedPools.Default<List<ILexeme>>();
        var matches = pool.AllocateAndClear();
        var misses = pool.AllocateAndClear();

        for (var i = 0; i < _existingLexemes.Count; i++)
        {
            var lexeme = _existingLexemes[i];
            if (lexeme.Scan(lexContext, character))
                matches.Add(lexeme);
            else
                misses.Add(lexeme);
        }

        if (matches.Count == 0)
        {
            pool.ClearAndFree(matches);
            pool.ClearAndFree(misses);
            return false;
        }

        for (var i = 0; i < misses.Count; i++)
            FreeLexeme(misses[i]);

        pool.ClearAndFree(misses);

        pool.ClearAndFree(_existingLexemes);
        _existingLexemes = matches;

        return true;
    }

    private bool TryParseExistingToken(IParseContext parseContext)
    {
        // PERF: Avoid Linq FirstOrDefault due to lambda allocation
        ILexeme longestAcceptedMatch = null;
        int doNotFreeLexemeIndex = -1;
        for (int i = 0; i < _existingLexemes.Count; i++)
        {
            var lexeme = _existingLexemes[i];
            if (lexeme.IsAccepted())
            {
                doNotFreeLexemeIndex = i;
                longestAcceptedMatch = lexeme;
                break;
            }
        }

        if (longestAcceptedMatch == null)
            return false;

        //var token = CreateTokenFromLexeme(longestAcceptedMatch);
        //if (token == null)
        //    return false;
        if (!ParseEngine.Pulse(parseContext, longestAcceptedMatch))
            return false;

        ClearExistingLexemes(doNotFreeLexemeIndex);

        return true;
    }

    private IToken CreateTokenFromLexeme(ILexeme lexeme)
    {
        var capture = lexeme.Value;
        return new Token(
            capture,
            Position - capture.Length - 1,
            lexeme.TokenType);
    }

    private void ClearExistingLexemes(int doNotFreeLexemeIndex)
    {
        ClearLexemes(_existingLexemes, doNotFreeLexemeIndex);
    }

    private bool MatchesNewLexemes(ILexContext lexContext, char character)
    {
        var newLexerRules = ParseEngine.GetExpectedLexerRules();
        var anyMatchingLexeme = false;
        for (var i = 0; i < newLexerRules.Count; i++)
        {
            var lexerRule = newLexerRules[i];
            var factory = _lexemeFactoryRegistry.Get(lexerRule.LexerRuleType);
            var lexeme = factory.Create(lexerRule, Position);
            if (!lexeme.Scan(lexContext, character))
            {
                factory.Free(lexeme);
                continue;
            }
            anyMatchingLexeme = true;
            _existingLexemes.Add(lexeme);
        }

        SharedPools.Default<List<ILexerRule>>().ClearAndFree(newLexerRules);
        return anyMatchingLexeme;
    }

    private bool MatchesExistingIgnoreLexemes(ILexContext lexContext, char character)
    {
        if (!AnyExistingIngoreLexemes())
            return false;

        var pool = SharedPools.Default<List<ILexeme>>();
        List<ILexeme> matches = null;

        for (int i = 0; i < _ignoreLexemes.Count; i++)
        {
            var lexeme = _ignoreLexemes[i];
            if (!lexeme.Scan(lexContext, character))
            {
                FreeLexeme(lexeme);
                continue;
            }
            if (matches == null)
                matches = pool.AllocateAndClear();
            matches.Add(lexeme);
        }

        if (matches == null)
        {
            _ignoreLexemes.Clear();
            pool.ClearAndFree(matches);
            return false;
        }

        pool.ClearAndFree(_ignoreLexemes);
        _ignoreLexemes = matches;

        return _ignoreLexemes.Count > 0;
    }

    private void ClearExistingIngoreLexemes()
    {
        ClearLexemes(_ignoreLexemes);
    }

    private bool MatchesNewIgnoreLexemes(ILexContext lexContext, char character)
    {
        var lexerRules = ParseEngine.Grammar.Ignores;
        var pool = SharedPools.Default<List<ILexeme>>();
        List<ILexeme> matches = null;

        for (var i = 0; i < lexerRules.Count; i++)
        {
            var lexerRule = lexerRules[i];
            var factory = _lexemeFactoryRegistry.Get(lexerRule.LexerRuleType);
            var lexeme = factory.Create(lexerRule, Position);

            if (!lexeme.Scan(lexContext, character))
            {
                FreeLexeme(lexeme);
                continue;
            }

            if (matches == null)
                matches = pool.AllocateAndClear();

            matches.Add(lexeme);
        }

        if (matches == null)
            return false;

        if (matches.Count == 0)
        {
            pool.ClearAndFree(matches);
            return false;
        }

        pool.ClearAndFree(_ignoreLexemes);
        _ignoreLexemes = matches;

        return true;
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

    private bool AnyExistingLexemes()
    {
        return _existingLexemes.Count > 0;
    }

    private static void RegisterDefaultLexemeFactories(ILexemeFactoryRegistry lexemeFactoryRegistry)
    {
        lexemeFactoryRegistry.Register(new TerminalLexemeFactory());
        lexemeFactoryRegistry.Register(new ParseEngineLexemeFactory());
        lexemeFactoryRegistry.Register(new StringLiteralLexemeFactory());
        lexemeFactoryRegistry.Register(new DfaLexemeFactory());
    }

}