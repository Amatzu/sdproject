using System;
using System.Linq;

namespace SystemAnalyzer.Graphs.Isomorphism
{
    /// <summary>
    /// Содержит подграф и определитель его матрицы смежности.
    /// </summary>
    internal readonly struct InvariantInstance
    {
        public readonly int Key;
        public readonly string[] Vertices;

        public InvariantInstance(int key, string[] vertices)
        {
            Key = key;
            Vertices = vertices;
        }

        public bool Intersects(InvariantInstance other)
        {
            return Vertices.Any(v => Array.IndexOf(other.Vertices, v) != -1);
        }
    }
}
