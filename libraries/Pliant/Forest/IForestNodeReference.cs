using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Forest
{
    /// <summary>
    /// IForestNodeReference is used as a observable refernce to a IForestNode. 
    /// During parse tree creation, ITransitionState nodes can result in the same 
    /// logical VirtualForestNode being created but not properly linked in the 
    /// parse forest. This class simultaneously updates all ITransitionState nodes
    /// with the correct IForestNode when any one of related notes receives its parse node
    /// during forest traversal.
    /// </summary>
    public interface IForestNodeReference
    {
        IForestNode Node { get; set; }
    }
}
