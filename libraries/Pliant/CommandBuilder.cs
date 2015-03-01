using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class CommandBuilder : ICommandBuilder
    {
        private IList<string> _ignoreList;
        public CommandBuilder()
        {
            _ignoreList = new List<string>();
        }

        public ICommandBuilder Ignore(string item)
        {
            _ignoreList.Add(item);
            return this;
        }

        public IList<string> GetIgnoreList() { return _ignoreList; }
    }
}
