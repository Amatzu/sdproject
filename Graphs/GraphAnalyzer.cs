using System;
using System.Collections.Generic;
using System.Linq;
using SystemAnalyzer.Matrices;

using Graph = QuickGraph.BidirectionalGraph<string, SystemAnalyzer.Graphs.Flow>;
using PotentialPatternMap =
    System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<int[]>>;

namespace SystemAnalyzer.Graphs
{
	/// <summary>
	/// Представляет анализатор, выявляющий паттерны в графе и составляющий их библиотеку.
	/// </summary>
	internal class GraphAnalyzer
	{
		private readonly Graph graph;
	    private readonly AdjacencyMatrix adjacency;
	    private int[,] Matrix => adjacency.Matrix;

	    public PotentialPatternMap[] PotentialPatterns { get; private set; }
		public Dictionary<string, Pattern> PatternLibrary { get; private set; }

		public GraphAnalyzer(Graph graph)
		{
			this.graph = graph;
			PatternLibrary = new Dictionary<string, Pattern>();

		    adjacency = AdjacencyMatrix.FromGraph(graph);

		    PotentialPatterns = new PotentialPatternMap[adjacency.MaxMinorSize - 2];
		    for (int i = 0; i < PotentialPatterns.Length; i++)
		    {
		        PotentialPatterns[i] = new PotentialPatternMap();
		    }

            //todo
		    FindAllMinors();
		}

	    private void FindPotentialPatterns()
	    {
	        var cache = new MinorCache(adjacency);

	        //Находим миноры размера 2
	        var minors = MinorsOfSize(2);
	        foreach (var minor in minors)
	        {
	            int i = minor[0];
	            int j = minor[1];

	            int det = Matrix[i, i] * Matrix[j, j] - Matrix[i, j] * Matrix[j, i];
	            cache[minor] = det;
	        }

	        //Находим остальные миноры рекурсивно
	        for (int n = 3; n <= adjacency.MaxMinorSize; n++)
	        {
	            minors = MinorsOfSize(n);
	            foreach (var minor in minors)
	            {
	                int det = 0;
	                for (int i = 0; i < minor.Length; i++)
	                {
	                    int sign = i % 2 == 0 ? 1 : -1;
	                    det += Matrix[minor[i], 0] * cache[SkipIndex(minor, i)] * sign;
	                }

	                cache[minor] = det;
	                AddPotentialPattern(minor, det);
	            }

                SiftPotentialPatterns(n);
	        }
	    }

	    private void AddPotentialPattern(int[] minor, int det)
	    {
	        int n = minor.Length;

	        //Если минор связный, вносим в словарь потенциальных паттернов
	        if (IsConnected(minor))
	        {
	            if (!PotentialPatterns[n - 3].ContainsKey(det))
	            {
                    var list = new List<int[]>();
	                PotentialPatterns[n - 3].Add(det, list);
	            }

	            var minorCopy = new int[minor.Length];
                Array.Copy(minor, minorCopy, minor.Length);

	            PotentialPatterns[n - 3][det].Insert(0, minorCopy);
	        }
	    }

	    private void SiftPotentialPatterns(int n)
	    {
	        var patternMap = PotentialPatterns[n - 3];
	        var keys = patternMap.Keys.ToArray();

	        for (int i = 0; i < keys.Length; i++)
	        {
	            int key = keys[i];

	            //Отбрасываем миноры с уникальными определителями - это не может быть паттерн
	            if (patternMap[key].Count == 1)
	            {
	                patternMap.Remove(key);
                    continue;
	            }
	        }
	    }

	    private bool AreIntersecting(int[] minor1, int[] minor2)
	    {
	        foreach (int i in minor1)
	        {
	            foreach (int j in minor2)
	            {
	                if (i == j) return true;
	            }
	        }

	        return false;
	    }

	    private bool IsConnected(int[] minor)
	    {
	        foreach (int i in minor)
	        {
	            bool isConnected = false;

	            foreach (int j in minor)
	            {
	                if (i == j) continue;

	                if (Matrix[i, j] > 0 || Matrix[j, i] > 0)
	                {
	                    isConnected = true;
                        break;
	                }
	            }

	            if (!isConnected) return false;
	        }

	        return true;
	    }

	    private IEnumerable<int[]> MinorsOfSize(int n)
	    {
	        var iterator = new CombinationIterator(0, adjacency.MaxMinorSize, n);
	        return iterator.Iterations;
	    }

	    private int[] SkipIndex(int[] minor, int skippedIndex)
	    {
	        var newMinor = new int[minor.Length - 1];

	        int j = 0;
	        for (int i = 0; i < minor.Length; i++)
	        {
	            if (i == skippedIndex) continue;
	            newMinor[j] = minor[i];
	            j++;
	        }

	        return newMinor;
	    }
	}
}
