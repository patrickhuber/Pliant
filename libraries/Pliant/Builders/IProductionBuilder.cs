using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Builders
{
    public interface IProductionBuilder
    {
        IProductionBuilder Production(string name, Action<IRuleBuilder> rules);
    }
}
