using System.Collections.Generic;

namespace Pliant.Builders.Models
{
    public class AlterationModel
    {
        public IList<SymbolModel> Symbols { get; private set; }

        public AlterationModel()
        {
            Symbols = new List<SymbolModel>();
        }
    }
}
