using Pliant.Grammars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Ebnf
{
    public interface IEbnfProductionNamingStrategy
    {
        INonTerminal GetSymbolForRepetition(EbnfFactorRepetition repetition);
        INonTerminal GetSymbolForOptional(EbnfFactorOptional optional);        
    }
}
