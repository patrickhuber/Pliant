using System.Collections.Generic;

namespace Pliant.Builders
{
    public class BaseBuilderList : List<BaseBuilder>
    {
        public BaseBuilderList CreateAlterations()
        {
            var otherBuilderList = new BaseBuilderList();
            foreach (var element in this)
            {
                otherBuilderList.Add(element);
                otherBuilderList.Add(element);
            }
            return otherBuilderList;
        }
    }
}