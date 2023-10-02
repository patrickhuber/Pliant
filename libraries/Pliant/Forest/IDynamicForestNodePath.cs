using Pliant.Grammars;

namespace Pliant.Forest
{
    public interface IDynamicForestNodePath
    {
        /// <summary>
        /// Returns the next link in the chain. Returns null if this is the tail ptr
        /// </summary>
        IDynamicForestNodePath Next();

        /// <summary>
        /// Returns the node associated with this link in the path
        /// </summary>
        /// <returns></returns>
        IForestNode Node();
    }
}
