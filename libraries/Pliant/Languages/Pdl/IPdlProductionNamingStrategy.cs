using Pliant.Grammars;

namespace Pliant.Languages.Pdl
{
    public interface IPdlProductionNamingStrategy
    {
        INonTerminal GetSymbolForRepetition(PdlFactorRepetition repetition);
        INonTerminal GetSymbolForOptional(PdlFactorOptional optional);        
    }
}
