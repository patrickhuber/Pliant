using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Forest
{
    public class ForestNodeReference : IForestNodeReference
    {
        public IForestNode Node { get; set; }
    }
}
