using Pliant.Grammars;
using System;

namespace Pliant.Builders
{
    public class ProductionReferenceBuilder : BaseBuilder
    {
        public IGrammar Grammar { get; private set; }
        public INonTerminal Reference { get; private set; }
        public override ISymbol Symbol { get { return Reference; } }

        public ProductionReferenceBuilder(IGrammar grammar)
        {
            Grammar = grammar;
            Reference = grammar.Start;
        }

        public ProductionReferenceBuilder(IGrammar grammar, INonTerminal reference)
        {
            ValidateParamters(grammar, reference);
            Grammar = grammar;
            Reference = reference;
        }

        private static void ValidateParamters(IGrammar grammar, INonTerminal reference)
        {
            var rules = grammar.RulesFor(reference);

            // PERF: Avoid LINQ Any due to Lambda allocation.
            var anyRules = false;
            foreach (var rule in rules)
            {
                anyRules = true;
                break;
            }

            if (!anyRules)
                throw new ArgumentException(
                    "Referenced non terminal must belong to specified grammar.", 
                    nameof(reference));
        }
    }
}
