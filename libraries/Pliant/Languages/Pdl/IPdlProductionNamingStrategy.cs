using Pliant.Grammars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Languages.Pdl
{
    public interface IPdlProductionNamingStrategy
    {
        INonTerminal GetSymbolForRepetition(PdlFactorRepetition repetition);
        INonTerminal GetSymbolForOptional(PdlFactorOptional optional);        
    }
}
