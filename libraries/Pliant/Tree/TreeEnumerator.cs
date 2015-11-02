using Pliant.Ast;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Tree
{
    public class TreeEnumerator : IEnumerator<ITreeNode>
    {
        INodeVisitorStateManager _stateManager;
        IInternalNode _root;

        public TreeEnumerator(IInternalNode internalNode, INodeVisitorStateManager stateManager)
        {
            _root = internalNode;
            _stateManager = stateManager;
        }

        public ITreeNode Current
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        object IEnumerator.Current
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
