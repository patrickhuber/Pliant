using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pliant.Tests.Unit.Tokens
{
    [TestClass]
    public class SegmentTests
    {
        [TestMethod]
        public void NewStringBuilderSegmentWrapsStringBuilder()
        {
            var input = "test";
            var builder = new StringBuilder(input);
            var segment = new StringBuilderSegment(builder);
            Assert.IsNull(segment.Parent);
            Assert.AreEqual(0, segment.Offset);
            Assert.AreEqual(input.Length, segment.Count);
        }

        [TestMethod]
        public void AppendingToStringBuilderShouldIncreaseSizeOfSegement()
        {
            var input = "test";
            var builder = new StringBuilder();
            var segment = new StringBuilderSegment(builder);
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
            var input = "test";
            var builder = new StringBuilder(input);
            var segment = new StringBuilderSegment(builder);
            var slice = segment.Slice(2);
            Assert.AreEqual(segment, slice.Parent);
            Assert.AreEqual(2, slice.Offset);
            Assert.AreEqual(2, slice.Count);
        }

        [TestMethod]
        public void SliceCanGrowWhenParentGrows()
        {
            var input = "test";
            var builder = new StringBuilder(input);
            var segment = new StringBuilderSegment(builder);
            var slice = segment.Slice(2);
            builder.Append("t");
            Assert.IsTrue(slice.CanGrow());
        }

        [TestMethod]
        public void SliceCanNotGrowPastParentSize()
        {
            var input = "test";
            var builder = new StringBuilder(input);
            var segment = new StringBuilderSegment(builder);
            var slice = segment.Slice(2);
            Assert.IsFalse(slice.CanGrow());
        }

        [TestMethod]
        public void PeekReturnsFalseWhenSegmentHasNoParent()
        {
            var input = "test";
            var builder = new StringBuilder(input);
            var segment = new StringBuilderSegment(builder);
            Assert.IsFalse(segment.Peek(out _));
        }

        [TestMethod]
        public void PeekReturnsTrueWhenSegmentHasParent()
        {
            var input = "test";
            var builder = new StringBuilder(input);
            var segment = new StringBuilderSegment(builder);
            var slice = segment.Slice(2, 1);
            Assert.IsTrue(slice.Peek(out char c));
            Assert.AreEqual('t', c);
        }

        [TestMethod]
        public void GrandChildSliceShouldBeOffsetRelevantToParent()
        {
            var input = "test";
            var builder = new StringBuilder(input);
            var segment = new StringBuilderSegment(builder);
            var child = segment.Slice(1);
            var grandChild = child.Slice(1);
            Assert.AreEqual(2, grandChild.Count);
            Assert.AreEqual(2, grandChild.Offset);
        }

        [TestMethod]
        public void ChildGrowShouldIncreaseCount()
        {
            var input = "test";
            var builder = new StringBuilder(input);
            var segment = new StringBuilderSegment(builder);
            var child = segment.Slice(0);
            Assert.IsFalse(child.Grow());
            var preCount = child.Count;
            builder.Append('a');
            Assert.IsTrue(child.Grow());
            Assert.IsTrue(preCount < child.Count);
        }
    }
}
