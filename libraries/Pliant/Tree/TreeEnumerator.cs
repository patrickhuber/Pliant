using Pliant.Forest;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Pliant.Tree
{
    public class TreeEnumerator : IEnumerator<ITreeNode>
    {
        private INodeVisitorStateManager _stateManager;
        private IInternalNode _root;

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