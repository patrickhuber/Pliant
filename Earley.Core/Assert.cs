using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Earley
{
    internal class Assert
    {
        internal static void IsNotNull(object instance, string propertyName)
        {
            IsNotNull(instance, propertyName, string.Format("{0} can not be null.", propertyName));
        }

        internal static void IsNotNull(object instance, string propertyName, string message)
        {
            if (instance == null)
                throw new ArgumentNullException(propertyName, message);
        }

        internal static void IsNotNullOrEmpty(Array array, string propertyName)
        {
            IsNotNullOrEmpty(array, propertyName, string.Format("{0} can not be null or empty.", propertyName));
        }

        internal static void IsNotNullOrEmpty(Array array, string propertyName, string message)
        {
            if (array == null)
                throw new ArgumentNullException(propertyName, message);
            if (array.Length == 0)
                throw new ArgumentOutOfRangeException(propertyName, message);
        }

        internal static void IsGreaterThanZero(int integer, string paramterName)
        {
            if (integer < 0)
                throw new ArgumentOutOfRangeException(
                    "paramterName", 
                    string.Format("{0} can not be less than zero.", paramterName));
        }
    }
}
