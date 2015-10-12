using Pliant.Grammars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Builders
{
    public class AlterationBuilder : IAlterationBuilder
    {
        public IList<IList<ISymbol>> Alterations;

        public AlterationBuilder(IList<IList<ISymbol>> alterations)
        {
            Alterations = alterations;
        }

        public IAlterationBuilder Or(params SymbolBuilder[] rules)
        {
            var newAlterations = new List<ISymbol>();
            foreach (var rule in rules)
                newAlterations.Add(rule.Symbol);
            Alterations.Add(newAlterations);
            return new AlterationBuilder(Alterations);
        }
    }
}
