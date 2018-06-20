using System.Collections.Generic;
using SystemAnalyzer.Matrices;

namespace SystemAnalyzer.Graphs
{
    public class Pattern
    {
        public readonly int Determinant;
        public readonly AdjacencyMatrix Matrix;
        public List<Graph> Instances { get; private set; }

        public int Size => Matrix.Vertices;

        public Pattern(int determinant, AdjacencyMatrix matrix)
        {
            Determinant = determinant;
            Matrix = matrix;
            Instances = new List<Graph>();
        }
    }
}
