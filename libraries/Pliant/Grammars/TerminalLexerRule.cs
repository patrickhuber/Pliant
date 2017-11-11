using System;
using Pliant.Tokens;
using Pliant.Utilities;

namespace Pliant.Grammars
{
    public class TerminalLexerRule : BaseLexerRule, ITerminalLexerRule
    {
        public ITerminal Terminal { get; }

        public static readonly LexerRuleType TerminalLexerRuleType = new LexerRuleType("Terminal");

        private readonly int _hashCode;

        public TerminalLexerRule(char character)
            : this(new CharacterTerminal(character), new TokenType(character.ToString()))
        {
        }

        public TerminalLexerRule(ITerminal terminal, TokenType tokenType)
            : base(TerminalLexerRuleType, tokenType)
        {
            Terminal = terminal;
            _hashCode = ComputeHashCode(terminal, TerminalLexerRuleType, tokenType);
        }

        public TerminalLexerRule(ITerminal terminal, string tokenTypeId)
            : this(terminal, new TokenType(tokenTypeId))
        {
        }

        public override string ToString()
        {
            return TokenType.ToString();
        }

        public override bool Equals(object obj)
        {
            if (((object)obj) == null)
                return false;
            var terminalLexerRule = obj as TerminalLexerRule;
            if (((object)terminalLexerRule) == null)
                return false;
            return LexerRuleType.Equals(terminalLexerRule.LexerRuleType)
                && Terminal.Equals(terminalLexerRule.Terminal);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        private static int ComputeHashCode(
            ITerminal terminal,
            LexerRuleType terminalLexerRuleType,
            TokenType tokenType)
        {
            return HashCode.Compute(
                terminalLexerRuleType.GetHashCode(),
                tokenType.GetHashCode(),
                terminal.GetHashCode());
        }

        public override bool CanApply(char c)
        {
            return Terminal.IsMatch(c);
        }
    }
}