using System;

namespace Pliant.Grammars
{
    public class FullyQualifiedName
    {
        public string Name { get; private set; }
        public string Namespace { get; private set; }
        public string FullName { get; private set; }

        private readonly int _hashCode;

        public FullyQualifiedName(string @namespace, string name)
        {
            Namespace = @namespace;
            Name = name;
            FullName = !string.IsNullOrWhiteSpace(@namespace)
                ? $"{@namespace}.{name}"
                : $"{name}";
            _hashCode = ComputeHashCode(FullName);
        }

        private static int ComputeHashCode(string fullName)
        {
            return fullName.GetHashCode();
        }

        public override string ToString()
        {
            return FullName;
        }

        public static bool operator ==(FullyQualifiedName fullyQualifiedName, string value)
        {
            return fullyQualifiedName.FullName.Equals(value);
        }

        public static bool operator !=(FullyQualifiedName fullyQualifiedName, string value)
        {
            return !fullyQualifiedName.FullName.Equals(value);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var otherFullyQualifiedName = obj as FullyQualifiedName;
            if (otherFullyQualifiedName == null)
                return false;
            return otherFullyQualifiedName.FullName == FullName;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}