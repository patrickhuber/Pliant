using System;

namespace Pliant.Diagnostics
{
    internal static class Assert
    {
        internal static void IsNotNull(object instance, string propertyName)
        {
            if (instance == null)
                throw new ArgumentNullException(propertyName, $"{propertyName} can not be null.");
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
                var message = $"{propertyName} can not be null or empty.";
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
                    $"{paramterName} can not be less than zero.");
        }

        internal static void IsTrue(bool condition, string message)
        {
            if (condition)
                throw new Exception(message);
        }

        internal static void IsTrue(bool condition)
        {
            IsTrue(condition, "Condition is expected to be true.");
        }

        internal static void IsFalse(bool condition, string message)
        {
            if (!condition)
                throw new Exception(message);
        }

        internal static void IsFalse(bool condition)
        {
            IsFalse(condition, "Condition is expected to be false.");
        }
    }
}