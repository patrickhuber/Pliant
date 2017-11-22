using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Builders
{
    public abstract class SettingModel
    {
        public string Name { get; private set; }

        public string Value { get; private set; }

        protected SettingModel(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
