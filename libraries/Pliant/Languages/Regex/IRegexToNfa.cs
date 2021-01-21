using Pliant.Automata;

namespace Pliant.Languages.Regex
{
    public interface IRegexToNfa
    {
        INfa Transform(RegexDefinition regex);
    }
}