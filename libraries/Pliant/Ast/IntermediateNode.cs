﻿using System;
using Pliant.Charts;

namespace Pliant.Ast
{
    public class IntermediateNode : InternalNode, IIntermediateNode
    {
        public IState State { get; private set; }

        public override NodeType NodeType { get { return NodeType.Intermediate; } }

        public IntermediateNode(IState state, int origin, int location)
            : base(origin, location)
        {
            State = state;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", State, Origin, Location);
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}