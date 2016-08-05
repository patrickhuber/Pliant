using Pliant.Forest;
using System;
using System.Collections;

namespace Pliant.Tree
{
    public class ParseTreeEnumerator
        : IParseTreeEnumerator
    {
        IInternalForestNode _forestRoot;

        public ParseTreeEnumerator(
            IInternalForestNode forestRoot)
        {
            _forestRoot = forestRoot;
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
