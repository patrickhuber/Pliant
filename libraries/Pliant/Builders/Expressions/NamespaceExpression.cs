using Pliant.Grammars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Builders.Expressions
{
    public class NamespaceExpression
    {
        public string Namespace { get; private set; }

        public NamespaceExpression(string @namespace)
        {
            Namespace = @namespace;
        }

        public static FullyQualifiedName operator +(NamespaceExpression @namespace, string name)
        {
            return new FullyQualifiedName(@namespace.Namespace, name);
        }

        public static implicit operator NamespaceExpression(string @namespace)
        {
            return new NamespaceExpression(@namespace);
        }
    }
}
