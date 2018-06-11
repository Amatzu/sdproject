using System;
using System.Collections.Generic;
using System.Linq;
using SystemAnalyzer.Graphs;

using Graph = QuickGraph.BidirectionalGraph<string, SystemAnalyzer.Graphs.Flow>;

namespace SystemAnalyzer.Matrices
{
    internal class AdjacencyMatrix
    {
        public string[] VertexMap { get; private set; }
        public int[,] Matrix { get; private set; }
        public int Vertices     => Matrix.GetLength(0);
        public int MaxMinorSize => Vertices / 2;

        private AdjacencyMatrix(string[] vertexMap, int[,] matrix)
        {
            VertexMap = vertexMap;
            Matrix = matrix;
        }

        public static AdjacencyMatrix FromGraph(Graph graph)
        {
            var vertices = from v in graph.Vertices
                           orderby v ascending
                           select v;

            string[] vertexMap = vertices.ToArray();

            var matrix          = new int[vertexMap.Length, vertexMap.Length];
            var adjacencyMatrix = new AdjacencyMatrix(vertexMap, matrix);

            var edgeGroups = from e in graph.Edges
                             group e by Edge.FromFlow(e)
                             into flowGroup
                             select flowGroup;

            foreach (var edgeGroup in edgeGroups)
            {
                Edge edge      = edgeGroup.Key;
                int  edgeCount = edgeGroup.Count();
                adjacencyMatrix[edge.Source, edge.Target] = edgeCount;
            }

            return adjacencyMatrix;
        }

        public int this[string src, string dest]
        {
            get {
                int srcIndex  = Array.BinarySearch(VertexMap, src);
                int destIndex = Array.BinarySearch(VertexMap, dest);
                return Matrix[srcIndex, destIndex];
            }

            set {
                int srcIndex  = Array.BinarySearch(VertexMap, src);
                int destIndex = Array.BinarySearch(VertexMap, dest);
                Matrix[srcIndex, destIndex] = value;
            }
        }
    }
}
