using System;
using System.Linq;
using Graph = QuickGraph.BidirectionalGraph<string, SystemAnalyzer.Core.Flow>;

namespace SystemAnalyzer.Core
{
    class AdjacencyMatrix
    {
        private readonly string[] vertexMap;
        private readonly int[,] matrix;

        private AdjacencyMatrix(string[] vertexMap)
        {
            this.vertexMap = vertexMap;
            matrix = new int[vertexMap.Length, vertexMap.Length];
        }

        public static AdjacencyMatrix FromGraph(Graph graph)
        {
            var vertices = from v in graph.Vertices
                           orderby v ascending
                           select v;

            string[] vertexMap = vertices.ToArray();

            var matrix = new AdjacencyMatrix(vertexMap);

            var edgeGroups = from e in graph.Edges
                             group e by Edge.FromFlow(e)
                             into flowGroup
                             select flowGroup;

            foreach (var edgeGroup in edgeGroups)
            {
                Edge edge = edgeGroup.Key;
                int edgeCount = edgeGroup.Count();
                matrix[edge.Source, edge.Target] = edgeCount;
            }

            return matrix;
        }

        public int this[string src, string dest]
        {
            get {
                int srcIndex  = Array.BinarySearch(vertexMap, src);
                int destIndex = Array.BinarySearch(vertexMap, dest);
                return matrix[srcIndex, destIndex];
            }

            set {
                int srcIndex  = Array.BinarySearch(vertexMap, src);
                int destIndex = Array.BinarySearch(vertexMap, dest);
                matrix[srcIndex, destIndex] = value;
            }
        }
    }
}
