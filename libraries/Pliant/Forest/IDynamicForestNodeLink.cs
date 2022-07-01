using Pliant.Grammars;

namespace Pliant.Forest
{
    public interface IDynamicForestNodeLink
    { 
        /// <summary>
        /// Returns the next link in the chain. Returns null if this is the tail ptr
        /// </summary>
        IDynamicForestNodeLink Next { get; }

        /// <summary>
        /// Returns the first link in the chain
        /// </summary>
        IDynamicForestNodeLink First { get; }

        /// <summary>
        /// Returns the bottom node of the reduction path
        /// </summary>
        IForestNode Bottom { get; }
                
        ISymbol Symbol { get; }
    }
}
