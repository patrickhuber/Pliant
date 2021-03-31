using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Collections;

namespace Pliant.Tests.Unit.Collections
{
    [TestClass]
    public class FastLookupDictionaryTests
    {
        private class Element
        {
            public int Value { get; private set; }

            public Element(int value)
            {
                Value = value;
            }

            public override bool Equals(object obj)
            {
                if (obj is null)
                    return false;
                var element = obj as Element;
                return Value.Equals(element.Value);
            }

            public override int GetHashCode()
            {
                return Value.GetHashCode();
            }
        }

        [TestMethod]
        public void FastLookupDictionaryTryGetValueShouldReturnValueWithSameHashCode()
        {
            // create two elements with the same hashcode
            var first = new Element(1);
            var second = new Element(1);

            var fastLookupDictionary = new FastLookupDictionary<Element, Element>
            {
                [first] = second
            };

            Assert.IsTrue(fastLookupDictionary.TryGetValue(second, out Element third));
            Assert.IsTrue(ReferenceEquals(third, second));
        }

        [TestMethod]
        public void FastLookupDictionaryTryGetValueShouldContainAllValuesOfLargeList()
        {
            var fastLookupDictionary = new FastLookupDictionary<int, int>();
            for (int i = 0; i < 50; i++)
            {
                fastLookupDictionary.Add(i, i);
                Assert.IsTrue(fastLookupDictionary.TryGetValue(i, out _));
            }
        }


        [TestMethod]
        public void FastLookupDictionaryGetValueShouldContainAllValuesOfLargeList()
        {
            var fastLookupDictionary = new FastLookupDictionary<int, int>();
            for (int i = 0; i < 50; i++)
            {
                fastLookupDictionary.Add(i, i);
                var value = fastLookupDictionary[i];
                Assert.AreEqual(i, value);
            }
        }

        [TestMethod]
        public void FastLookupDictionaryGetValueShouldContainAllValuesOfSmallList()
        {
            var fastLookupDictionary = new FastLookupDictionary<int, int>();
            for (int i = 0; i < 2; i++)
            {
                fastLookupDictionary.Add(i, i);
                var value = fastLookupDictionary[i];
                Assert.AreEqual(i, value);
            }
        }
    }
}
