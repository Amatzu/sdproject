using System.Collections.Generic;
using System.Linq;
using SystemAnalyzer.Matrices;
using SystemAnalyzer.Utils.Extensions;
using Container = System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<string[]>>;

namespace SystemAnalyzer.Graphs.Isomorphism
{
    /// <summary>
    /// Группирует потенциальные паттерны по инвариантам.
    /// </summary>
    internal class InvariantMap
    {
        private readonly AdjacencyMatrix matrix;
        private readonly Container[] container;

        public InvariantMap(AdjacencyMatrix matrix)
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
            if (!this[minor.Length].ContainsKey(det))
            {
                var list = new List<string[]>();
                this[minor.Length].Add(det, list);
            }

            string[] vertices = matrix.VertexMap.SelectIndices(minor);
            this[minor.Length][det].Insert(0, vertices);
        }

        public InvariantInstance[] GetAllOfSize(int size)
        {
            var x = this[size].SelectMany(pair => pair.Value.Select(v => new InvariantInstance(pair.Key, v)));
            return x.ToArray();
        }

        public Container this[int size] => container[size - 3];
    }
}
