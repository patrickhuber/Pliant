using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Samples.WithPdl.Ast
{
    public class Expression
    {
        public Term Term { get; set; }
    }

    public class ExpressionOperatorTerm : Expression
    {
        public Expression Expression { get; set; }
        public Operator Operator { get; set; }
    }
}
