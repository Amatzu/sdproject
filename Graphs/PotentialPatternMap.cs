using System.Collections.Generic;
using System.Linq;
using SystemAnalyzer.Matrices;

using Container = System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<int[]>>;

namespace SystemAnalyzer.Graphs
{
    internal class PotentialPatternMap
    {
        private AdjacencyMatrix matrix;
        private readonly Container[] container;

        public PotentialPatternMap(AdjacencyMatrix matrix)
        {
            this.matrix = matrix;
            container = new Container[matrix.MaxMinorSize - 2];
            for (int i = 0; i < container.Length; i++)
            {
                container[i] = new Container();
            }
        }

        /// <summary>
        /// Добавляет минор матрицы смежности в каталог потенциальных паттернов.
        /// </summary>
        /// <param name="minor">Минор матрицы смежности</param>
        /// <param name="det">Определитель минора</param>
        public void Add(int[] minor, int det)
        {
            //todo: proper hashcode
            if (!this[minor.Length].ContainsKey(det))
            {
                var list = new List<int[]>();
                this[minor.Length].Add(det, list);
            }

            var minorCopy = new int[minor.Length];
            minor.CopyTo(minorCopy, 0);

            this[minor.Length][det].Insert(0, minorCopy);
        }

        /// <summary>
        /// Отбрасывает группы потенциальных паттернов, где все подграфы имеют
        /// какую-либо общую вершину.
        /// </summary>
        /// <param name="order">Порядок для просеивания</param>
        public void Sift(int order)
        {
            var hashes = this[order].Keys.ToArray();

            foreach (var hash in hashes)
            {
                var minors = this[order][hash];

                for (int i = 0; i < matrix.Vertices; i++)
                {
                    if (minors.All(m => m.Contains(i)))
                    {
                        this[order].Remove(hash);
                        break;
                    }
                }
            }
        }

        public Container this[int size] => container[size - 3];
    }
}
