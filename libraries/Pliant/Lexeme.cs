using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    /// <summary>
    /// A Lexeme is something special. It acts like a token and a mini parser.
    /// </summary>
    public class Lexeme : IState
    {
        
        public IDottedRule DottedRule { get; private set; }
        
        public IProduction Production
        {
            get { throw new NotImplementedException(); }
        }

        public int Origin
        {
            get { throw new NotImplementedException(); }
        }

        public StateType StateType
        {
            get { throw new NotImplementedException(); }
        }

        public IState NextState()
        {
            throw new NotImplementedException();
        }

        public IState NextState(int newOrigin)
        {
            throw new NotImplementedException();
        }

        public IState NextState(INode parseNode)
        {
            throw new NotImplementedException();
        }

        public IState NextState(int newOrigin, INode parseNode)
        {
            throw new NotImplementedException();
        }

        public INode ParseNode
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public bool IsSource(ISymbol searchSymbol)
        {
            throw new NotImplementedException();
        }
    }
}
