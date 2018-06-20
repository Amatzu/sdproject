using System;
using System.Collections.Generic;
using System.Linq;
using SystemAnalyzer.Graphs.Isomorphism;
using SystemAnalyzer.Matrices;
using SystemAnalyzer.Utils.Extensions;
using QuickGraph;

namespace SystemAnalyzer.Graphs
{
	/// <summary>
	/// Представляет анализатор, выявляющий паттерны в графе и составляющий их библиотеку.
	/// </summary>
	public class GraphAnalyzer
	{
		private readonly Graph graph;
	    private readonly AdjacencyMatrix matrix;
	    private readonly InvariantMap invariants;

		public GraphAnalyzer(Graph graph)
		{
			this.graph = graph;
		    matrix = AdjacencyMatrix.FromGraph(graph);
		    invariants = matrix.FindPotentialPatterns();
		}

	    public List<Pattern> FindPatterns()
	    {
	        var patterns = new List<Pattern>();

	        for (int n = matrix.MaxMinorSize; n > 2; n--)
	        {
	            var newPatterns = PatternsOfSize(n);
                patterns.AddRange(newPatterns);
	        }

	        return FilterPatterns(patterns);
	    }

        //todo: find old patterns
	    private List<Pattern> PatternsOfSize(int n)
	    {
	        var patterns = new List<Pattern>();

	        foreach (int key in invariants[n].Keys)
	        {
	            int count = invariants[n][key].Count;
	            if (count < 2) continue;

	            var subgraphs = GetSubgraphs(n, key);
	            var matrices  = subgraphs.Select(AdjacencyMatrix.FromGraph).ToArray();
	            var edgeCount = invariants[n][key].Select(CountEdges).ToArray();

	            int[] isomorphismGroups = new int[count];
	            for (int i = 0; i < count; i++)
	                isomorphismGroups[i] = -1;

	            for (int i = 0; i < count - 1; i++)
	            {
	                var checker = new IsomorphismChecker(matrices[i]);

	                for (int j = i + 1; j < count; j++)
	                {
	                    if (isomorphismGroups[j] != -1) continue;
	                    if (!edgeCount[i].SequenceEqual(edgeCount[j])) continue;

	                    bool isomorphic = checker.Check(matrices[j]);
	                    if (isomorphic)
	                    {
	                        isomorphismGroups[i] = i;
	                        isomorphismGroups[j] = i;
	                    }
	                }
	            }

	            for (int i = 0; i < count - 1; i++)
	            {
                    if (isomorphismGroups[i] != i) continue;

                    var pattern = new Pattern(key, matrices[i]);
	                for (int j = i + 1; j < count; j++)
	                {
                        if (isomorphismGroups[j] != i) continue;
	                    pattern.Instances.Add(subgraphs[j]);
	                }
                    patterns.Add(pattern);
	            }
	        }

	        return patterns;
	    }

	    private List<Pattern> FilterPatterns(List<Pattern> patterns)
	    {
            throw new NotImplementedException();
	    }

	    private Graph[] GetSubgraphs(int n, int key)
	    {
	        var subgraphs = from m in invariants[n][key]
	                       let vertices = matrix.VertexMap.SelectIndices(m)
	                       select graph.Subgraph(vertices);

	        return subgraphs.ToArray();
	    }

	    private EdgeCount[] CountEdges(int[] vertexIds)
	    {
	        var vertices = matrix.VertexMap.SelectIndices(vertexIds);
	        var edgeCount = from v in vertices
	                        let selfEdges = graph.SelfFlows.Count(e => e.Source.Name == v)
	                        let outEdges  = graph.NonSelfFlows.Count(e => e.Source.Name == v)
	                        let inEdges   = graph.NonSelfFlows.Count(e => e.Target.Name == v)
	                        select new EdgeCount(selfEdges, inEdges, outEdges) into count
	                        orderby count.OutEdges descending,
	                                count.InEdges descending,
	                                count.SelfEdges descending
	                        select count;

	        return edgeCount.ToArray();
	    }
	}
}
