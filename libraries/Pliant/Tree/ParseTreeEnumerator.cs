using Pliant.Forest;
using System.Collections.Generic;
using System;
using System.Collections;

namespace Pliant.Tree
{
    public class ParseTreeEnumerator
        : IParseTreeEnumerator
    {
        public ParseTreeEnumerator(
            IForestNodeVisitorStateManager stateManager,
            IInternalForestNode forestRoot)
        {
            var internalTreeNode = new InternalTreeNode(forestRoot, stateManager);     
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
            throw new NotImplementedException();
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
