using System;
using System.Collections.Generic;
using System.Text;

namespace Pliant.Forest
{
    public static class IInternalForestNodeExtensions
   {
        public static IForestNode GetLeftSubtree(this IInternalForestNode node)        
        {
            if (node == null)
                return null;
            if (node.Children.Count == 0)
                return null;            
            var packed = node.Children[0];
            if (packed.Children.Count == 0)
                return null;

            return packed.Children[0];
        }

        public static IForestNode GetRightSubtree(this IInternalForestNode node)
        {
            if (node == null)
                return null;
            if (node.Children.Count == 0)
                return null;
            var packed = node.Children[0];
            if (packed.Children.Count < 2)
                return null;
            return packed.Children[1];            
        }
    }
}
