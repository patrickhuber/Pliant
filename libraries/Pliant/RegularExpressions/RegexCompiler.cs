using Pliant.Automata;

namespace Pliant.RegularExpressions
{
    public class RegexCompiler
    {
        private IRegexToNfa _regexToNfa;
        private INfaToDfa _nfaToDfa;

        public RegexCompiler(
            IRegexToNfa regexToNfa,
            INfaToDfa nfaToDfa)
        {
            _regexToNfa = regexToNfa;
            _nfaToDfa = nfaToDfa;
        }

        public IDfaState Compile(Regex regex)
        {
            var nfa = _regexToNfa.Transform(regex);
            var dfa = _nfaToDfa.Transform(nfa);
            return dfa;
        }
    }
}