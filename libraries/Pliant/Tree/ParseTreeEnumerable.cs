using Pliant.Forest;
using System.Collections;
using System.Collections.Generic;

namespace Pliant.Tree
{
    public class ParseTreeEnumerable : IEnumerable<ITreeNode>
    {
        readonly IInternalForestNode _internalForestNode;

        public ParseTreeEnumerable(IInternalForestNode internalForestNode)
        {
            this._internalForestNode = internalForestNode;
        }

        public IEnumerator<ITreeNode> GetEnumerator()
        {
            return new ParseTreeEnumerator(_internalForestNode);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ParseTreeEnumerator(_internalForestNode);
        }
    }
}
