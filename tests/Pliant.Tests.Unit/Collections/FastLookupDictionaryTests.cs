using System;
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
                if (obj == null)
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

            var fastLookupDictionary = new FastLookupDictionary<Element, Element>();
            fastLookupDictionary[first] = second;

            Element third = null;
            Assert.IsTrue(fastLookupDictionary.TryGetValue(second, out third));
            Assert.IsTrue(ReferenceEquals(third, second));
        }

        [TestMethod]
        public void FastLookupDictionaryTryGetValueShouldContainAllValuesOfLargeList()
        {
            var fastLookupDictionary = new FastLookupDictionary<int, int>();
            for (int i = 0; i < 50; i++)
            {
                fastLookupDictionary.Add(i, i);
                int value;
                Assert.IsTrue(fastLookupDictionary.TryGetValue(i, out value));            
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
