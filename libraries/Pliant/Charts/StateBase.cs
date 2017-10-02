using System;
using Pliant.Diagnostics;
using Pliant.Forest;
using Pliant.Grammars;

namespace Pliant.Charts
{
    public abstract class StateBase : IState
    {
        public IDottedRule DottedRule { get; private set; }

        public int Origin { get; private set; }
        
        public abstract StateType StateType { get; }

        public IForestNode ParseNode { get; set; }
        
        protected StateBase(IDottedRule dottedRule, int origin)
        {
            Assert.IsNotNull(dottedRule, nameof(dottedRule));
            Assert.IsGreaterThanEqualToZero(origin, nameof(origin));
            DottedRule = dottedRule;
            Origin = origin;
        }
    }
}
