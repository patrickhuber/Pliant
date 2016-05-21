using Pliant.Forest;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Pliant.Tree
{
    public class TreeEnumerator : IEnumerator<ITreeNode>
    {
        private IForestNodeVisitorStateManager _stateManager;
        private IInternalForestNode _root;

        public TreeEnumerator(IInternalForestNode internalNode, IForestNodeVisitorStateManager stateManager)
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