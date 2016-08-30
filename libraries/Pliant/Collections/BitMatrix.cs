using System.Collections;

namespace Pliant.Collections
{
    internal class BitMatrix
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

        public void TransitiveClosure()
        {
            for (int k = 0; k < _matrix.Length; k++)
            {
                for (int i = 0; i < _matrix.Length; i++)
                {
                    for (int j = 0; j < _matrix.Length; j++)
                    {
                        _matrix[i][j] = _matrix[i][j]
                            || (_matrix[i][k] && _matrix[k][j]);
                    }
                }
            }
        }

        public BitMatrix Clone()
        {
            var bitMatrix = new BitMatrix(_matrix.Length);
            for (int i = 0; i < _matrix.Length; i++)
            {
                bitMatrix._matrix[i] = _matrix[i].Clone() as BitArray;
            }
            return bitMatrix;
        }
    }
}
