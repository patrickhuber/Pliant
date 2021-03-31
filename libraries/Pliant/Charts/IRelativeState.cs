using Pliant.Grammars;

namespace Pliant.Charts
{
    interface IRelativeState
    {
        IDottedRule DottedRule { get; }
        int Offset { get; }        
    }
}
