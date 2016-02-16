using System;

namespace Pliant
{
    internal static class Assert
    {
        internal static void IsNotNull(object instance, string propertyName)
        {
            if (instance == null)
                throw new ArgumentNullException(propertyName, string.Format("{0} can not be null.", propertyName));
        }

        internal static void IsNotNull(object instance, string propertyName, string message)
        {
            // PERF: Remove string format from method invocation unless the value is actually null
            if (instance == null)
                throw new ArgumentNullException(propertyName, message);
        }

        internal static void IsNotNullOrEmpty(Array array, string propertyName)
        {
            // PERF: Remove string format from method invocation unless the value is actually null or empty
            if (array == null || array.Length == 0)
            {
                var message = string.Format("{0} can not be null or empty.", propertyName);
                if (array == null)
                    throw new ArgumentNullException(propertyName, message);
                if (array.Length == 0)
                    throw new ArgumentOutOfRangeException(propertyName, message);
            }
        }

        internal static void IsNotNullOrEmpty(Array array, string propertyName, string message)
        {
            if (array == null)
                throw new ArgumentNullException(propertyName, message);
            if (array.Length == 0)
                throw new ArgumentOutOfRangeException(propertyName, message);
        }

        internal static void IsGreaterThanEqualToZero(int integer, string paramterName)
        {
            if (integer < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(paramterName),
                    string.Format("{0} can not be less than zero.", paramterName));
        }
    }
}