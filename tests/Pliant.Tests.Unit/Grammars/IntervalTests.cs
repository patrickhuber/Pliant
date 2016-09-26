using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Grammars;

namespace Pliant.Tests.Unit.Grammars
{
    [TestClass]
    public class IntervalTests
    {
        [TestMethod]
        public void IntervalShouldOverlapWithOtherIntervalWhenMaxAndOtherMinOverlap()
        {
            var first = new Interval('a', 'c');
            var second = new Interval('b', 'f');

            Assert.IsTrue(first.Overlaps(second));
        }

        [TestMethod]
        public void IntervalShouldOverlapWithOtherIntervalWhenMinAndOtherMaxOverlap()
        {
            var first = new Interval('b', 'f');
            var second = new Interval('a', 'c');

            Assert.IsTrue(first.Overlaps(second));
        }

        [TestMethod]
        public void IntervalShouldOverlapWithOtherIntervalWhenFirstContainsSecond()
        {
            var first = new Interval('a', 'f');
            var second = new Interval('b', 'c');

            Assert.IsTrue(first.Overlaps(second));
        }

        [TestMethod]
        public void IntervalShouldOverlapWithOtherIntervalWhenSecondContainsFirst()
        {
            var first = new Interval('b', 'c');
            var second = new Interval('a', 'f');

            Assert.IsTrue(first.Overlaps(second));
        }

        [TestMethod]
        public void IntervalShouldNotOverlapWithOtherIntervalWhenMaxLessThanOtherMin()
        {
            var first = new Interval('a', 'b');
            var second = new Interval('c', 'd');
            Assert.IsFalse(first.Overlaps(second));
        }

        [TestMethod]
        public void IntervalShouldNotOverlapWithOtherIntervalWhenMinGreaterThanOtherMax()
        {
            var first = new Interval('c', 'd');
            var second = new Interval('a', 'b');
            Assert.IsFalse(first.Overlaps(second));
        }

        [TestMethod]
        public void IntervalShouldEqualOtherIntervalWhenMinAndMaxEqual()
        {
            var first = new Interval('m', 'x');
            var second = new Interval('m', 'x');

            Assert.IsTrue(first.CompareTo(second) == 0);
        }

        [TestMethod]
        public void IntervalShouldBeLessThanOtherIntervalWhenMinEqualAndFirstMaxLessThanSecondMax()
        {
            var first = new Interval('m', 'p');
            var second = new Interval('m', 'x');

            Assert.IsTrue(first.CompareTo(second) < 0);
        }

        [TestMethod]
        public void IntervalShouldBeGreaterThanOtherIntervalWhenMinEqualAndFirstMaxGreaterThanSecondMax()
        {
            var first = new Interval('m', 'x');
            var second = new Interval('m', 'p');

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [TestMethod]
        public void IntervalShouldBeLessThanOtherIntervalWhenMinLessThanSecondMin()
        {

            var first = new Interval('a', 'z');
            var second = new Interval('l', 'r');

            Assert.IsTrue(first.CompareTo(second)< 0);
        }

        [TestMethod]
        public void IntervalShouldBeGreaterThanOtherIntervalWhenMinGreaterThanSecondMin()
        {
            var first = new Interval('k', 'z');
            var second = new Interval('f', 'h');

            Assert.IsTrue(first.CompareTo(second) > 0);
        }

        [TestMethod]
        public void IntervalJoinShouldReturnOneIntervalWhenOverlap()
        {
            var first = new Interval('1', '3');
            var second = new Interval('2', '5');

            var joins = Interval.Join(first, second);
            Assert.AreEqual(1, joins.Count);
            Assert.AreEqual('1', joins[0].Min);
            Assert.AreEqual('5', joins[0].Max);
        }

        [TestMethod]
        public void IntervalJoinShouldReturnTwoIntervalsWhenTwoIntervalsDoNotOverlap()
        {
            var first = new Interval('1', '3');
            var second = new Interval('5', '6');

            var joins = Interval.Join(first, second);

            Assert.AreEqual(2, joins.Count);

            Assert.AreSame(first, joins[0]);
            Assert.AreSame(second, joins[1]);
        }

        [TestMethod]
        public void IntervalJoinShouldReturnOneIntervalWhenTwoIntervalsAreTheSame()
        {
            var first = new Interval('1', '3');
            var second = new Interval('1', '3');

            var joins = Interval.Join(first, second);

            Assert.AreEqual(1, joins.Count);
            Assert.AreEqual(0, first.CompareTo(joins[0]));
        }

        [TestMethod]
        public void IntervalSplitShoudlReturnOneInteralWhenTwoIntervalsAreTheSame()
        {
            var first = new Interval('1', '3');
            var second = new Interval('1', '3');

            var splits = Interval.Split(first, second);

            Assert.AreEqual(1, splits.Count);
            Assert.AreEqual(first.Min, splits[0].Min);
            Assert.AreEqual(first.Max, splits[0].Max);
        }

        [TestMethod]
        public void IntervalSplitShouldReturnTwoIntervalsWhenTwoIntervalsDoNotOverlap()
        {
            var first = new Interval('1', '3');
            var second = new Interval('5', '6');

            var splits = Interval.Split(first, second);

            Assert.AreEqual(2, splits.Count);


            Assert.AreEqual(first.Min, splits[0].Min);
            Assert.AreEqual(first.Max, splits[0].Max);
            Assert.AreEqual(second.Min, splits[1].Min);
            Assert.AreEqual(second.Max, splits[1].Max);
        }

        [TestMethod]
        public void IntervalSplitShouldReturnTwoIntervalsWhenSameMinAndDifferentMaxes()
        {
            var first = new Interval('1', '3');
            var second = new Interval('1', '9');

            var splits = Interval.Split(first, second);

            Assert.AreEqual(2, splits.Count);

            Assert.AreEqual('1', splits[0].Min);
            Assert.AreEqual('3', splits[0].Max);
            Assert.AreEqual('4', splits[1].Min);
            Assert.AreEqual('9', splits[1].Max);
        }

        [TestMethod]
        public void IntervalSplitShouldReturnTwoIntervalsWhenSameMaxAndDifferentMins()
        {
            var first = new Interval('1', '9');
            var second = new Interval('5', '9');

            var splits = Interval.Split(first, second);

            Assert.AreEqual(2, splits.Count);
            Assert.AreEqual('1', splits[0].Min);
            Assert.AreEqual('4', splits[0].Max);
            Assert.AreEqual('5', splits[1].Min);
            Assert.AreEqual('9', splits[1].Max);
        }
    }
}
