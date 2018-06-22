using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SystemAnalyzer.Graphs.Isomorphism;
using SystemAnalyzer.Graphs.Patterns;
using SystemAnalyzer.Matrices;
using SystemAnalyzer.Utils.Extensions;

namespace SystemAnalyzer.Graphs
{
	/// <summary>
	/// Представляет анализатор, выявляющий паттерны в графе и составляющий их библиотеку.
	/// </summary>
	public class GraphAnalyzer
	{
        private const int INIT_GROUP = -1;
        private const int REMOVED_GROUP    = -2;

		private readonly Graph graph;
	    private readonly AdjacencyMatrix matrix;
	    private readonly InvariantMap invariants;
	    public PatternMap Patterns { get; private set; }

		public GraphAnalyzer(Graph graph, PatternMap patterns = null)
		{
			this.graph = graph;
		    matrix = AdjacencyMatrix.FromGraph(graph);
		    invariants = matrix.GetInvariantGroups();

            Patterns = patterns ?? new PatternMap(matrix);
            if (patterns != null) Patterns.ClearInstances();
		}

        /// <summary>
        /// Находит новые паттерны и вхождения существующих.
        /// </summary>
	    public bool FindPatterns(out PatternMap patterns)
	    {
	        patterns = new PatternMap(matrix);

	        var coveredVertices = new List<string>();
	        for (int n = matrix.MaxMinorSize; n > 2; n--)
	        {
                patterns[n] = PatternsOfSize(n, coveredVertices);
                patterns.RemoveSelfIntersecting(n);
                SelectInstances(patterns, n);
	            FilterInstances(patterns, n);

	            var vertices = patterns[n].SelectMany(p => p.Instances).SelectMany(i => i.Vertices);
	            coveredVertices.AddRange(vertices);

	            patterns[n].AddRange(Patterns[n]);
	        }

	        return !patterns.IsEmpty;
	    }

        //todo: find old patterns
	    private List<Pattern> PatternsOfSize(int size, IEnumerable<string> coveredVertices)
	    {
	        var newPatterns = new List<Pattern>();

	        var group = invariants.GetAllOfSize(size)
	                              .Where(i => !i.Vertices.Any(coveredVertices.Contains))
	                              .ToArray();
	        if (group.Length < 2) return newPatterns;

	        var matrices = group.Select(g => graph.SubgraphMatrix(g.Vertices)).ToArray();

	        int[] isomorphismGroups = new int[group.Length];
	        isomorphismGroups.InitializeWith(INIT_GROUP);

	        foreach (var pattern in Patterns[size])
	        {
                var checker = new IsomorphismChecker(pattern.Matrix);

	            for (int i = 0; i < group.Length; i++)
	            {
	                if (group[i].Key != pattern.Key)  continue;

	                bool isomorphic = checker.Check(matrices[i]);
	                if (isomorphic)
	                {
	                    isomorphismGroups[i] = REMOVED_GROUP;
                        pattern.AddInstance(group[i].Vertices);
	                }
	            }
	        }

	        var edgeCount = group.Select(g => CountEdges(g.Vertices)).ToArray();
	        var checkers  = matrices.Select(m => new IsomorphismChecker(m)).ToArray();

	        for (int i = 0; i < group.Length - 1; i++)
	        {
	            for (int j = i + 1; j < group.Length; j++)
	            {
	                if (isomorphismGroups[j] > INIT_GROUP) continue;
	                if (group[i].Key != group[j].Key) continue;
                    if (group[i].Intersects(group[j])) continue;
	                if (!edgeCount[i].SequenceEqual(edgeCount[j])) continue;

	                bool isomorphic = checkers[i].Check(matrices[j]);
	                if (isomorphic)
	                {
	                    isomorphismGroups[i] = i;
	                    isomorphismGroups[j] = i;
	                }
	            }
	        }

	        for (int i = 0; i < group.Length; i++)
	        {
                if (isomorphismGroups[i] != i) continue;

                var pattern = new Pattern(group[i].Key, matrices[i]);
	            pattern.AddInstance(group[i].Vertices);

	            for (int j = i + 1; j < group.Length; j++)
	            {
                    if (isomorphismGroups[j] != i) continue;
	                pattern.AddInstance(group[j].Vertices);
	            }
                newPatterns.Add(pattern);
	        }

	        return newPatterns;
	    }

        /// <summary>
        /// Отбрасывает паттерны, пересекающиеся с паттернами большего размера или
        /// имеющие менее двух экземпляров.
        /// </summary>
	    private void FilterInstances(PatternMap patterns, int n)
	    {
	        foreach (var smallPattern in patterns[n])
	        {
	            for (int m = n + 1; m <= patterns.MaxPatternSize; m++)
	            {
	                foreach (var bigPattern in patterns[m])
	                {
	                    smallPattern.RemoveIntersectingInstances(bigPattern);
	                }
	            }
	        }

	        patterns.RemoveUniques(n);
	    }

        /// <summary>
        /// Из каждой группы пересекающихся паттернов выбирает как можно большее количество.
        /// </summary>
	    private void SelectInstances(PatternMap patterns, int size)
	    {
	        FilterInstances(patterns, size);
	        if (patterns[size].Count == 0) return;

	        var allInstances = patterns[size].SelectMany(p => p.Instances).ToArray();
	        var selectedInstances = new List<PatternInstance>();

	        var instanceGroups = GroupInstances(allInstances);

	        foreach (var group in instanceGroups)
	        {
	            var instances = SelectInstancesFromGroup(group);
	            selectedInstances.AddRange(instances);
	        }

	        foreach (var pattern in patterns[size])
	        {
	            pattern.Instances.RemoveAll(i => !selectedInstances.Contains(i));
	        }
	    }

        /// <summary>
        /// Группирует экземпляры паттернов по пересечениям.
        /// </summary>
	    private IEnumerable<PatternInstance[]> GroupInstances(PatternInstance[] instances)
	    {
	        var disjointSet = PatternInstance.GetIntersectionGroups(instances);

	        var instancesByGroup = instances.Select((x, i) => new {
	                                          Value = x,
	                                          Group = disjointSet.Find(i)
	                                      });
	        var groups = instancesByGroup.GroupBy(i => i.Group);
            var groupValues = groups.Select(g => g.Select(h => h.Value).ToArray());

	        return groupValues;
	    }

        /// <summary>
        /// Выбирает максимально возможное количество непересекающихся экземпляров паттернов
        /// из группы.
        /// </summary>
	    private PatternInstance[] SelectInstancesFromGroup(PatternInstance[] instances)
	    {
            Debug.Assert(instances != null);
            Debug.Assert(instances.Length > 0);

            if (instances.Length == 1) return instances;

	        while (instances.AnyTwo((i, j) => i.Intersects(j)))
	        {
	            var sortedInstances = from i in instances
	                                  orderby instances.Count(i.Intersects) descending,
	                                          i.Pattern.Name descending
	                                  select i;

	            instances = instances.RemoveItem(sortedInstances.First());
	        }

	        return instances;
	    }

        /// <summary>
        /// Для каждой вершины находит количество исходящих рёбер, входящих рёбер и дуг.
        /// Результат сортируется по убыванию этих параметров.
        /// </summary>
	    private EdgeCount[] CountEdges(string[] vertices)
	    {
	        var edgeCount =
	            from v in vertices
	            let selfEdges = graph.SelfFlows.Count(e => e.Source.Name == v)
	            let outEdges  = graph.NonSelfFlows.Count(e => e.Source.Name == v
	                                                           && vertices.Contains(e.Target.Name))
                let inEdges   = graph.NonSelfFlows.Count(e => e.Target.Name == v
	                                                           && vertices.Contains(e.Source.Name))
	            select new EdgeCount(selfEdges, inEdges, outEdges) into count
	            orderby count.OutEdges descending,
	                    count.InEdges descending,
	                    count.SelfEdges descending
	            select count;

	        return edgeCount.ToArray();
	    }
	}
}
