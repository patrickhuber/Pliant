using System;
using System.Collections.Generic;
using System.Text;

namespace Pliant.Captures
{
    public interface ICapture<T>
    {
        /// <summary>
        /// Gets the number of elements in the range delimited by the segment.
        /// </summary>
        int Count { get; set; }

        /// <summary>
        /// Gets the element at the specified index
        /// </summary>
        /// <param name="index">the zero-based index of the element to get.</param>
        /// <returns>the element at the index</returns>
        T this[int index] { get; }

        /// <summary>
        /// Gets the position of the first element in the range delimited by the segment, relative to the start of the parent segment.
        /// </summary>
        int Offset { get; set; }

        /// <summary>
        /// Forms a slice out of the current segment starting at the specified index.
        /// </summary>
        /// <param name="index">The index at which to begin the slice.</param>
        /// <returns>An segment that consists of all elements of the current segment from index to the end of the segment.</returns>
        ICapture<T> Slice(int index);

        /// <summary>
        /// Forms a slice of the specified length out of the current segment starting at the specified index.
        /// </summary>
        /// <param name="index">The index at which to begin the slice.</param>
        /// <param name="count">The desired length of the slice.</param>
        /// <returns>A segment of count elements starting at index.</returns>
        ICapture<T> Slice(int index, int count);

        /// <summary>
        /// Returns the parent of this segment. If the segment has no parent, returns null.
        /// </summary>
        ICapture<T> Parent { get; }

        bool IsReadOnly { get; }

        string ToString(int offset, int count);
    }
}
