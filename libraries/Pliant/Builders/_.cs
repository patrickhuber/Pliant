using Pliant.Grammars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Builders
{
    public class _ : RuleBuilder
    {
        public _(BaseBuilder baseBuilder) : base(baseBuilder)
        { }

        public _() : base() { }

        public static explicit operator _(char terminal)
        {
            return new _(new SymbolBuilder(new TerminalLexerRule(terminal)));
        }

        public static explicit operator _(string lexerRule)
        {
            return new _(
                new SymbolBuilder(
                    new StringLiteralLexerRule(lexerRule)));
        }

        public static explicit operator _(ProductionBuilder productionBuilder)
        {
            return new _(productionBuilder);
        }

        public static explicit operator _(BaseLexerRule lexerRule)
        {
            return new _(new SymbolBuilder(lexerRule));
        }

        public static explicit operator _(BaseTerminal terminal)
        {
            return new _(
                new SymbolBuilder(
                    new TerminalLexerRule(terminal, terminal.ToString())));
        }
    }
}
