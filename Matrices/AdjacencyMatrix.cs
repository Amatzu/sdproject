using System;
using System.Collections.Generic;
using System.Linq;
using SystemAnalyzer.Graphs;
using SystemAnalyzer.Graphs.Isomorphism;
using SystemAnalyzer.Utils.Extensions;

namespace SystemAnalyzer.Matrices
{
    public class AdjacencyMatrix
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
                           select v.Name;

            string[] vertexMap = vertices.ToArray();

            var matrix          = new int[vertexMap.Length, vertexMap.Length];
            var adjacencyMatrix = new AdjacencyMatrix(vertexMap, matrix);

            var edgeGroups = from e in graph.InnerFlows
                             group e by Edge.FromFlow(e)
                             into flowGroup
                             select flowGroup;

            foreach (var edgeGroup in edgeGroups)
            {
                Edge edge      = edgeGroup.Key;
                int  edgeCount = edgeGroup.Count();
                adjacencyMatrix[edge.Source.Name, edge.Target.Name] = edgeCount;
            }

            return adjacencyMatrix;
        }

        /// <summary>
        /// Группирует связные подграфы по инвариантам.
        /// </summary>
        	internal InvariantMap GetInvariantGroups()
	    {
	        var cache = new MinorCache(this);
	        var invariantMap = new InvariantMap(this);

	        //Находим миноры размера 2
	        for (int i = 0; i < Vertices; i++)
	        {
	            for (int j = 0; j < Vertices; j++)
	            {
                    if (i == j) continue;

	                int det = Matrix[i, i] * Matrix[j, j] - Matrix[i, j] * Matrix[j, i];
	                cache[i, j] = det;
	            }
	        }

	        //Находим остальные миноры рекурсивно
	        for (int n = 3; n <= MaxMinorSize; n++)
	        {
	            var minors = MinorsOfSize(n);
	            foreach (var minor in minors)
	            {
	                int det = 0;
	                for (int i = 0; i < minor.Length; i++)
	                {
	                    int sign = i % 2 == 0 ? 1 : -1;
	                    int[] lesserMinor = minor.SkipIndex(i);
	                    det += this[minor[i], minor[0]] * cache[lesserMinor] * sign;
	                }

	                cache[minor] = det;
                    if(MinorIsConnected(minor)) invariantMap.Add(minor, det);
	            }
	        }

	        return invariantMap;
	    }

        /// <summary>
        /// Проверяет соответствующий минору подграф на связность.
        /// </summary>
	    private bool MinorIsConnected(int[] minor)
	    {
	        int dfs(int source, bool[] visited)
	        {
	            int visitedVertices = 1;
	            visited[source] = true;

	            for (int i = 1; i < minor.Length; i++)
	            {
                    if (visited[i]) continue;

	                int u = minor[source];
	                int v = minor[i];
	                if (this[u, v] > 0 || this[v, u] > 0)
	                {
	                    visitedVertices += dfs(i, visited);
	                }
	            }

	            return visitedVertices;
	        }

	        var connectedVertices = new bool[minor.Length];
	        return dfs(0, connectedVertices) == minor.Length;
	    }

        /// <summary>
        /// Перечисляет все возможные миноры размера n.
        /// </summary>
	    private IEnumerable<int[]> MinorsOfSize(int n)
	    {
	        var iterator = new CombinationIterator(0, Vertices - 1, n);
	        return iterator.Iterations;
	    }

        private int this[string src, string dest]
        {
            set {
                int srcIndex  = Array.IndexOf(VertexMap, src);
                int destIndex = Array.IndexOf(VertexMap, dest);
                Matrix[srcIndex, destIndex] = value;
            }
        }

        public int this[int i, int j] => Matrix[i, j];
    }
}
