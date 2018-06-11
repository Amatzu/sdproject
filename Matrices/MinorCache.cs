using System;
using SystemAnalyzer.Utils;

namespace SystemAnalyzer.Matrices
{
    /// <summary>
    /// Структура данных, хранящая определители миноров матрицы.
    /// Инденсация осуществляется по индексам минора.
    /// </summary>
    internal struct MinorCache
    {
        private const int MIN_SIZE = 2;
        private readonly int[][] minors;

        public MinorCache(AdjacencyMatrix matrix)
        {
            if (matrix == null)
                throw new NullReferenceException("Adjacency matrix is null");

            minors = new int[matrix.MaxMinorSize - 1][];

            for (int i = MIN_SIZE; i <= matrix.MaxMinorSize; i++)
            {
                int minorCount = MathUtils.Combinations(matrix.Vertices, i);
                minors[i - MIN_SIZE] = new int[minorCount];
            }
        }

        private int LinearMinorIndex(int[] indices)
        {
            if (indices == null)
                throw new NullReferenceException("Minor indices cannot be null.");

            int maxSize = minors.GetLength(0) + 1;
            if (indices.Length < 2 || indices.Length > maxSize)
            {
                throw new ArgumentOutOfRangeException($"Minor size was {indices.Length} " +
                    $"(must be between {MIN_SIZE} and {maxSize})");
            }

            int sum = 0;
            for (int i = 0; i < indices.Length; i++)
            {
                sum += indices[i] - i;
            }

            return sum;
        }

        public int this[params int[] indices]
        {
            get {
                int index = LinearMinorIndex(indices);
                int size = indices.Length;
                return minors[size - MIN_SIZE][index];
            }

            set {
                int index = LinearMinorIndex(indices);
                int size  = indices.Length;
                minors[size - MIN_SIZE][index] = value;
            }
        }
    }
}
