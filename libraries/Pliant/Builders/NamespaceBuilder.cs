using Pliant.Grammars;

namespace Pliant.Builders
{
    public class NamespaceBuilder
    {
        public string Namespace { get; private set; }

        public NamespaceBuilder(string @namespace)
        {
            Namespace = @namespace;
        }

        public static FullyQualifiedName operator +(NamespaceBuilder @namespace, string name)
        {
            return new FullyQualifiedName(@namespace.Namespace, name);
        }

        public static implicit operator NamespaceBuilder(string @namespace)
        {
            return new NamespaceBuilder(@namespace);
        }
    }
}
