using Pliant.Grammars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Builders.Fluent
{
    public class FluentRuleBuilder
    {
        public FluentAlterationBuilder Rule(params ISymbol[] symbol)
        {
            return new FluentAlterationBuilder();
        }
    }
}
