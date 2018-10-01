using System.Collections.Generic;
using System.Linq;
using SystemAnalyzer.Matrices;

namespace SystemAnalyzer.Graphs.Patterns
{
    public class Pattern
    {
        private const string NAME_BASE = "P";
        private static int totalPatternCount;

        public readonly int Key;
        public readonly AdjacencyMatrix Matrix;
        public List<PatternInstance> Instances { get; private set; }
        public readonly int EdgeCount;

        public readonly string Name;

        public Pattern(int key, AdjacencyMatrix matrix)
        {
            Name = NAME_BASE + totalPatternCount;
            totalPatternCount++;
            Key = key;
            Matrix = matrix;
            Instances = new List<PatternInstance>();

            EdgeCount = 0;
            int n = matrix.Vertices;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    EdgeCount += matrix[i, j];
                }
            }
        }

        /// <summary>
        /// Добавляет экземпляр паттерна.
        /// </summary>
        /// <param name="vertices">Входящие в экземпляр вершины</param>
        public void AddInstance(string[] vertices)
        {
            var instance = new PatternInstance(this, vertices);
            Instances.Add(instance);
        }

        /// <summary>
        /// Удаляет все экземпляры данного паттерна, пересекающиеся с любым экземпляром
        /// второго паттерна.
        /// </summary>
        /// <param name="that">Второй паттерн</param>
        public void RemoveIntersectingInstances(Pattern that)
        {
            Instances = Instances.Where(i => !that.Instances.Any(i.Intersects)).ToList();
        }
    }
}
