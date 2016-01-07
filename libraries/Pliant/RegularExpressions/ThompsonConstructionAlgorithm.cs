using Pliant.Automata;
using Pliant.Grammars;
using System;

namespace Pliant.RegularExpressions
{
    public class ThompsonConstructionAlgorithm : IRegexToNfa
    {
        public INfa Transform(Regex regex)
        {
            return Expression(regex.Expression);
        }

        private INfa Expression(RegexExpression expression)
        {
            switch (expression.NodeType)
            {
                case RegexNodeType.RegexExpression:
                    return Empty();

                case RegexNodeType.RegexExpressionAlteration:
                    var regexExpressionAlteration = expression as RegexExpressionAlteration;
                    
                    var termNfa = Term(regexExpressionAlteration.Term);
                    var expressionNfa = Expression(regexExpressionAlteration.Expression);

                    return Union(termNfa, expressionNfa);

                case RegexNodeType.RegexExpressionTerm:
                    var regexExpressionTerm = expression as RegexExpressionTerm;
                    return Term(regexExpressionTerm.Term);
            }
            throw new InvalidOperationException("Unrecognized Regex Expression");
        }

        private INfa Term(RegexTerm term)
        {
            switch (term.NodeType)
            {
                case RegexNodeType.RegexTerm:
                    return Factor(term.Factor);

                case RegexNodeType.RegexTermFactor:
                    var regexTermFactor = term as RegexTermFactor;
                    var factorNfa = Factor(regexTermFactor.Factor);
                    var termNfa = Term(regexTermFactor.Term);
                    return Concatenation(factorNfa, termNfa);
            }
            throw new InvalidOperationException("Unrecognized Regex Term");
        }

        private INfa Factor(RegexFactor factor)
        {
            var atomNfa = Atom(factor.Atom);
            switch (factor.NodeType)
            {
                case RegexNodeType.RegexFactor:
                    return atomNfa;

                case RegexNodeType.RegexFactorIterator:
                    var regexFactorIterator = factor as RegexFactorIterator;
                    switch (regexFactorIterator.Iterator)
                    {
                        case RegexIterator.ZeroOrMany:
                            return KleeneStar(atomNfa);
                        case RegexIterator.OneOrMany:
                            return KleenePlus(atomNfa);
                        case RegexIterator.ZeroOrOne:
                            return Optional(atomNfa);
                    }
                    break;
            }
            throw new InvalidOperationException("Unrecognized regex factor");
        }

        private INfa Atom(RegexAtom atom)
        {
            switch (atom.NodeType)
            {
                case RegexNodeType.RegexAtomAny:
                    return Any();

                case RegexNodeType.RegexAtomCharacter:
                    var regexAtomCharacter = atom as RegexAtomCharacter;
                    return Character(regexAtomCharacter.Character);

                case RegexNodeType.RegexAtomExpression:
                    var regexAtomExpression = atom as RegexAtomExpression;
                    return Expression(regexAtomExpression.Expression);

                case RegexNodeType.RegexAtomSet:
                    var regexAtomSet = atom as RegexAtomSet;
                    return Set(regexAtomSet);
            }
            throw new InvalidOperationException("Unrecognized regex atom");
        }

        private INfa Set(RegexAtomSet regexAtomSet)
        {
            throw new NotImplementedException();
        }

        private INfa Character(RegexCharacterClassCharacter character)
        {
            var start = new NfaState();
            var end = new NfaState();
            var terminal = new CharacterTerminal(character.Value);
            var transition = new TerminalNfaTransition(
                terminal: terminal,
                target: end);
            start.AddTransistion(transition);
            return new Nfa(start, end);
        }

        private INfa Character(RegexCharacter character)
        {
            var start = new NfaState();
            var end = new NfaState();
            var terminal = new CharacterTerminal(character.Value);
            var transition = new TerminalNfaTransition(
                terminal: terminal,
                target: end);
            start.AddTransistion(transition);
            return new Nfa(start, end);
        }

        private INfa Empty()
        {
            var start = new NfaState();
            var end = new NfaState();
            start.AddTransistion(new NullNfaTransition(end));
            return new Nfa(start, end);
        }

        private INfa Any()
        {
            var start = new NfaState();
            var end = new NfaState();
            start.AddTransistion(new TerminalNfaTransition(new AnyTerminal(), end));
            return new Nfa(start, end);
        }

        private INfa Union(INfa first, INfa second)
        {
            var start = new NfaState();
            start.AddTransistion(new NullNfaTransition(first.Start));
            start.AddTransistion(new NullNfaTransition(second.Start));

            var end = new NfaState();
            var endTransition = new NullNfaTransition(end);
            first.End.AddTransistion(endTransition);
            second.End.AddTransistion(endTransition);

            return new Nfa(start, end);
        }

        private INfa Concatenation(INfa first, INfa second)
        {
            first.End.AddTransistion(new NullNfaTransition(second.Start));
            return new Nfa(first.Start, second.End);
        }

        private INfa KleeneStar(INfa nfa)
        {
            var start = new NfaState();
            var nullToNfaStart = new NullNfaTransition(nfa.Start);

            start.AddTransistion(nullToNfaStart);
            nfa.End.AddTransistion(nullToNfaStart);

            var end = new NfaState();
            var nullToNewEnd = new NullNfaTransition(end);

            start.AddTransistion(nullToNewEnd);
            nfa.End.AddTransistion(nullToNewEnd);

            return new Nfa(start, end);
        }

        private INfa KleenePlus(INfa nfa)
        {
            var end = new NfaState();
            nfa.End.AddTransistion(new NullNfaTransition(end));
            nfa.End.AddTransistion(new NullNfaTransition(nfa.Start));
            return new Nfa(nfa.Start, end);
        }
        
        private INfa Optional(INfa nfa)
        {
            var start = new NfaState();
            var end = new NfaState();
            start.AddTransistion(new NullNfaTransition(nfa.Start));
            start.AddTransistion(new NullNfaTransition(end));
            nfa.End.AddTransistion(new NullNfaTransition(end));
            return new Nfa(start, end);
        }
    }
}