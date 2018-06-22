using System;
using System.Linq;

namespace SystemAnalyzer.Graphs.Isomorphism
{
    internal struct InvariantInstance
    {
        public int Key;
        public string[] Vertices;

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
