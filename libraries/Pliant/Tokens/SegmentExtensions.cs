using System;
using System.Collections.Generic;
using System.Text;

namespace Pliant.Tokens
{
    public static class SegmentExtensions
    {
        public static bool Grow<T>(this ISegment<T> segment)
        {
            // we can only grow child segments
            if (!CanGrow(segment))
                return false;

            if (segment.IsReadOnly)
                return false;

            segment.Count ++;
            return true;
        }

        public static bool CanGrow<T>(this ISegment<T> segment)
        {
            var parent = segment.Parent;
            return parent != null && segment.Offset + segment.Count < parent.Offset + parent.Count;
        }

        public static bool Peek<T>(this ISegment<T> segment, out T value)
        {
            // growing parent segements is the responsibility of the parent segement
            if (!segment.CanGrow())
            {
                value = default;
                return false;
            }

            // calculate the index of the parent where the peek element is located
            var index = segment.Count - segment.Offset + 1;
            value = segment.Parent[index];
            return true;
        }
    }
}
