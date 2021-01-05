using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Captures;
using System.Text;

namespace Pliant.Tests.Unit.Captures
{
    [TestClass]
    public class CaptureTests
    {
        [TestMethod]
        public void NewStringBuilderSegmentWrapsStringBuilder()
        {
            var input = "fast";
            var builder = new StringBuilder(input);
            var segment = new StringBuilderCapture(builder);
            Assert.IsNull(segment.Parent);
            Assert.AreEqual(0, segment.Offset);
            Assert.AreEqual(input.Length, segment.Count);
        }

        [TestMethod]
        public void AppendingToStringBuilderShouldIncreaseSizeOfSegement()
        {
            var input = "fast";
            var builder = new StringBuilder();
            var segment = new StringBuilderCapture(builder);
            Assert.IsNull(segment.Parent);
            Assert.AreEqual(0, segment.Offset);
            Assert.AreEqual(0, segment.Count);
            builder.Append(input);
            Assert.AreEqual(0, segment.Offset);
            Assert.AreEqual(input.Length, segment.Count);
        }

        [TestMethod]
        public void SliceReturnsSubsetOfParent()
        {
            var input = "fast";
            var builder = new StringBuilder(input);
            var segment = new StringBuilderCapture(builder);
            var slice = segment.Slice(2);
            Assert.AreEqual(segment, slice.Parent);
            Assert.AreEqual(2, slice.Offset);
            Assert.AreEqual(2, slice.Count);
        }

        [TestMethod]
        public void SliceCanGrowWhenParentGrows()
        {
            var input = "fast";
            var builder = new StringBuilder(input);
            var segment = new StringBuilderCapture(builder);
            var slice = segment.Slice(2);
            builder.Append("t");
            Assert.IsTrue(slice.CanGrow());
        }

        [TestMethod]
        public void SliceCanNotGrowPastParentSize()
        {
            var input = "fast";
            var builder = new StringBuilder(input);
            var segment = new StringBuilderCapture(builder);
            var slice = segment.Slice(2);
            Assert.IsFalse(slice.CanGrow());
        }

        [TestMethod]
        public void PeekReturnsFalseWhenSegmentHasNoParent()
        {
            var input = "fast";
            var builder = new StringBuilder(input);
            var segment = new StringBuilderCapture(builder);
            Assert.IsFalse(segment.Peek(out _));
        }

        [TestMethod]
        public void PeekReturnsTrueWhenSegmentHasParent()
        {
            // input = {'f','a','s','t','e','r'}
            // slice = {'s'}
            // peek  = {'s'} 't'
            var input = "faster";
            var builder = new StringBuilder(input);
            var segment = new StringBuilderCapture(builder);
            var slice = segment.Slice(2, 1);
            Assert.IsTrue(slice.Peek(out char c));
            Assert.AreEqual('t', c);
        }

        [TestMethod]
        public void CanPeekWhenSliceHasZeroCount()
        {
            // input = {'a'}
            // slice = {}
            // peek  = {'a'}
            var input = "a";
            var segment = new StringBuilder(input).AsCapture();
            var slice = segment.Slice(0, 0);
            Assert.IsTrue(slice.Peek(out char c));
            Assert.AreEqual('a', c);
        }

        [TestMethod]
        public void GrandChildSliceShouldBeOffsetRelevantToParent()
        {
            var input = "fast";
            var builder = new StringBuilder(input);
            var segment = new StringBuilderCapture(builder);
            var child = segment.Slice(1);
            var grandChild = child.Slice(1);
            Assert.AreEqual(2, grandChild.Count);
            Assert.AreEqual(2, grandChild.Offset);
        }

        [TestMethod]
        public void ChildGrowShouldIncreaseCount()
        {
            var input = "fast";
            var builder = new StringBuilder(input);
            var segment = new StringBuilderCapture(builder);
            var child = segment.Slice(0);
            Assert.IsFalse(child.Grow());
            var preCount = child.Count;
            builder.Append('e');
            Assert.IsTrue(child.Grow());
            Assert.IsTrue(preCount < child.Count);
        }

        [TestMethod]
        public void LastShouldReturnSliceOfSizeOffset()
        {
            var input = "fast";
            var builder = new StringBuilder(input);
            var segment = new StringBuilderCapture(builder);
            var slice = segment.Last(1);
            Assert.AreEqual(3, slice.Offset);
            Assert.AreEqual(1, slice.Count);
        }

        [TestMethod]
        public void LastShouldReturnSizeOfSpecifiedSizeAtReverseOffset()
        {
            var input = "fast";
            var builder = new StringBuilder(input);
            var segment = new StringBuilderCapture(builder);
            var slice = segment.Last(1, 0);
            Assert.AreEqual(3, slice.Offset);
            Assert.AreEqual(0, slice.Count);
        }

        [TestMethod]
        public void SliceCanAccessSubsetOfParent()
        {
            var input = new StringCapture("fast");
            var slice = input.Slice(1, 2);
            Assert.AreEqual('a', slice[0]);
            Assert.AreEqual('s', slice[1]);
        }

        [TestMethod]
        public void EqualWhenContainSameContent()
        {
            var input = "faster";
            var builder = new StringBuilder(input).AsCapture();
            var str = input.AsCapture();
            Assert.AreEqual(builder, str);
        }

        [TestMethod]
        public void EqualWhenDifferentOffsetsButSameContent()
        {
            var input = "runrun".AsCapture();
            var first = input.Slice(0, 3);
            var second = input.Slice(3);
            Assert.AreEqual(first, second);
        }
    }
}
