using System;
using System.Collections.Generic;
using System.Text;

namespace Pliant.Captures
{
    public static class CaptureExtensions
    {
        public static bool Grow<T>(this ICapture<T> segment)
        {
            // we can only grow child segments
            if (!segment.CanGrow())
                return false;

            if (segment.IsReadOnly)
                return false;

            segment.Count++;
            return true;
        }

        public static bool CanGrow<T>(this ICapture<T> segment)
        {
            var parent = segment.Parent;
            return parent != null && segment.Offset + segment.Count < parent.Offset + parent.Count;
        }

        public static bool Peek<T>(this ICapture<T> segment, out T value)
        {
            // growing parent segements is the responsibility of the parent segement
            if (!segment.CanGrow())
            {
                value = default;
                return false;
            }

            // calculate the index of the parent where the peek element is located
            var index = segment.Count + segment.Offset;
            value = segment.Parent[index];
            return true;
        }

        /// <summary>
        /// Returns an empty segment at the offset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="segment"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static ICapture<T> Empty<T>(this ICapture<T> segment, int offset) => segment.Slice(offset, 0);

        /// <summary>
        /// Returns an empty segment at offset zero
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="segment"></param>
        /// <returns>A slice at the start of the segment with Count zero.</returns>
        public static ICapture<T> Empty<T>(this ICapture<T> segment) => segment.Empty(0);

        /// <summary>
        /// Gets a slice starting from the offset at the end of the slice.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static ICapture<T> Last<T>(this ICapture<T> segment, int offset)
        {
            return segment.Last(offset, offset);
        }

        public static ICapture<T> Last<T>(this ICapture<T> segment, int offset, int count)
        {
            return segment.Slice(segment.Count + segment.Offset - offset, count);
        }

        public static T LastElement<T>(this ICapture<T> segment)
        {
            return segment[segment.Count - 1];
        }

        public static StringBuilderCapture AsCapture(this StringBuilder builder)
        {
            return new StringBuilderCapture(builder);
        }

        public static StringCapture AsCapture(this string s)
        {
            return new StringCapture(s);
        }
    }
}
