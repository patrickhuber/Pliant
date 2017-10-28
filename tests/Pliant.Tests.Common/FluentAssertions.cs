using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Pliant.Tests.Common
{
    public static class FluentAssertions
    {
        public static TValue AssertInBoundsAndNavigate<T, TValue>(this T value, Func<T, IReadOnlyList<TValue>> property, int index)
        {
            var collection = property(value);
            Assert.IsTrue(index < collection.Count);
            return collection[index];
        }

        public static TCast AssertIsInstanceOfTypeAndCast<TCast>(this object value)
            where TCast : class
        {
            Assert.IsInstanceOfType(value, typeof(TCast));
            return value as TCast;
        }
    }
}