using Pliant.Automata;

namespace Pliant.RegularExpressions
{
    public interface IRegexToNfa
    {
        INfa Transform(Regex regex);
    }
}