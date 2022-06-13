namespace Pliant.Samples.WithPdl.Ast
{
    public class Term
    {
        public Factor Factor { get; set; }
    }

    public class TermOperatorFactor : Term 
    { 
        public Term Term { get; set; }
        public Operator Operator { get; set; }
    }
}