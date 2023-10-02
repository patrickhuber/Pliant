using Pliant.Utilities;
using System.Collections;
using System.Text;

namespace Pliant.Collections
{
    public class BitMatrix
    {
        private BitArray[] _matrix;

        public BitMatrix(int count)
        {
            _matrix = new BitArray[count];
            for (int i = 0; i < count; i++)
                _matrix[i] = new BitArray(count);
        }

        public BitArray this[int index]
        {
            get { return _matrix[index]; }
        }

        public int Length { get { return _matrix.Length; } }

        public void Clear()
        {
            for (int i = 0; i < _matrix.Length; i++)
                _matrix[i].SetAll(false);
        }

        public BitMatrix TransitiveClosure()
        {
            var clone = Clone();
            for (var k = 0; k < clone.Length; k++)
                for (var j = 0; j < clone.Length; j++)
                    for (var i = 0; i < clone.Length; i++)
                        clone[i][j] = clone[i][j] || (clone[i][k] && clone[k][j]);
            return clone; 
        }

        public BitMatrix Clone()
        {
            var bitMatrix = new BitMatrix(_matrix.Length);
            for (int i = 0; i < _matrix.Length; i++)
            {
                bitMatrix._matrix[i] = new BitArray(_matrix[i]);
            }
            return bitMatrix;
        }

        public override string ToString()
        {
            const string space = " ";
            const string zero = "0";
            const string one = "1";
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < Length; i++)
            {
                if (i > 0)
                    stringBuilder.AppendLine();
                for (int j = 0; j < Length; j++)
                {
                    if (j > 0)
                        stringBuilder.Append(space);
                    stringBuilder.Append(this[i][j] ? one : zero);
                }
            }
            return stringBuilder.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is BitMatrix bitMatrix))
                return false;
            if (bitMatrix.Length != Length)
                return false;
            for (var i = 0; i < bitMatrix.Length; i++)
                if (!bitMatrix[i].Equals(this[i]))
                    return false;
            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            for (var i = 0; i < Length; i++)
                hashCode = HashCode.ComputeIncrementalHash(this[i].GetHashCode(), hashCode, i == 0);            
            return hashCode;
        }
    }
}
