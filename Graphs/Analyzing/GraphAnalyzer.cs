using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SystemAnalyzer.Graphs.Isomorphism;
using SystemAnalyzer.Graphs.Patterns;
using SystemAnalyzer.Matrices;
using SystemAnalyzer.Utils.Extensions;

namespace SystemAnalyzer.Graphs.Analyzing
{
	/// <summary>
	/// Представляет анализатор, выявляющий паттерны в графе и составляющий их библиотеку.
	/// </summary>
	public class GraphAnalyzer
	{
        private const int INIT_GROUP    = -2;
        private const int REMOVED_GROUP = -1;

		private readonly Graph graph;
	    private readonly AdjacencyMatrix matrix;
	    private readonly InvariantMap invariants;
	    private readonly PatternFilterer filterer;
	    private readonly PatternSelector selector;
	    public PatternMap KnownPatterns { get; private set; }

		public GraphAnalyzer(Graph graph, PatternMap patterns = null)
		{
			this.graph = graph;
		    matrix = AdjacencyMatrix.FromGraph(graph);
		    invariants = matrix.GetInvariantGroups();

            KnownPatterns = patterns ?? new PatternMap(matrix);
            if (patterns != null) KnownPatterns.ClearInstances();

            filterer = new PatternFilterer(matrix, KnownPatterns);
            selector = new PatternSelector(KnownPatterns, filterer);
		}

        /// <summary>
        /// Находит новые паттерны и вхождения существующих.
        /// </summary>
	    public bool FindPatterns(out PatternMap newPatterns)
	    {
	        newPatterns = new PatternMap(matrix);

	        var coveredVertices = new List<string>();
	        for (int n = matrix.Vertices - 1; n > 2; n--)
	        {
                newPatterns[n] = PatternsOfSize(n, coveredVertices);
                KnownPatterns[n].AddRange(newPatterns[n]);

                selector.SelectInstances(n);

	            filterer.RemoveIntersectingWithBiggerPatterns(n);
                filterer.RemoveSingletons(n);

	            var vertices = (from pattern in newPatterns[n]
	                            from instance in pattern.Instances
	                            from vertex in instance.Vertices
	                            select vertex).Distinct();

	            coveredVertices.AddRange(vertices);

	        }

	        return !newPatterns.IsEmpty;
	    }

	    /// <summary>
	    /// Находит все паттерны заданного размера.
	    /// </summary>
	    /// <param name="size">Размер паттерна</param>
	    /// <param name="coveredVertices">Покрытые паттернами большего размера вершины</param>
	    /// <returns>Список найденных паттернов</returns>
	    private List<Pattern> PatternsOfSize(int size, IEnumerable<string> coveredVertices)
	    {
	        var potentialPatterns = invariants.OfSize(size)
	                                          .Where(i => i.Vertices.None(coveredVertices.Contains))
	                                          .ToArray();

	        if (potentialPatterns.Length < 2) return new List<Pattern>();

	        var matrices = potentialPatterns.Select(g => graph.SubgraphMatrix(g.Vertices)).ToArray();

	        var isomorphismGroups = new int[potentialPatterns.Length];
	        isomorphismGroups.InitializeWith(INIT_GROUP);
	        FindKnownPatternInstances(isomorphismGroups, size, potentialPatterns, matrices);

	        if (size > matrix.Vertices) return new List<Pattern>();

	        SetIsomorphismGroups(isomorphismGroups, potentialPatterns, matrices);
	        var patterns = CreateNewPatterns(potentialPatterns, matrices, isomorphismGroups);
	        return patterns;
	    }

	    private void FindKnownPatternInstances(int[] isomorphismGroups, int size, InvariantInstance[] potentialPatterns, AdjacencyMatrix[] matrices)
        {
	        foreach (var oldPattern in KnownPatterns[size])
	        {
	            for (int i = 0; i < potentialPatterns.Length; i++)
	            {
	                if (potentialPatterns[i].Key != oldPattern.Key) continue;

	                var checker = new IsomorphismChecker(oldPattern.Matrix);
	                bool isomorphic = checker.Check(matrices[i]);
	                if (isomorphic)
	                {
	                    isomorphismGroups[i] = REMOVED_GROUP;
	                    oldPattern.AddInstance(potentialPatterns[i].Vertices);
	                }
	            }
	        }
	    }

	    private void SetIsomorphismGroups(int[] isomorphismGroups, InvariantInstance[] potentialPatterns, AdjacencyMatrix[] matrices)
	    {
	        var edgeCount = potentialPatterns.Select(g => CountEdges(g.Vertices)).ToArray();
	        var checkers = matrices.Select(m => new IsomorphismChecker(m)).ToArray();

	        for (int i = 0; i < potentialPatterns.Length - 1; i++)
	        {
	            for (int j = i + 1; j < potentialPatterns.Length; j++)
	            {
	                if (isomorphismGroups[j] > INIT_GROUP) continue;
	                if (potentialPatterns[i].Key != potentialPatterns[j].Key) continue;
	                if (potentialPatterns[i].Intersects(potentialPatterns[j])) continue;
	                if (!edgeCount[i].SequenceEqual(edgeCount[j])) continue;

	                bool isomorphic = checkers[i].Check(matrices[j]);
	                if (isomorphic)
	                {
	                    isomorphismGroups[i] = i;
	                    isomorphismGroups[j] = i;
	                }
	            }
	        }
	    }

	    private List<Pattern> CreateNewPatterns(InvariantInstance[] potentialPatterns, AdjacencyMatrix[] matrices, int[] isomorphismGroups)
	    {
	        var newPatterns = new List<Pattern>();

	        for (int i = 0; i < potentialPatterns.Length; i++)
	        {
	            if (isomorphismGroups[i] != i) continue;

	            var pattern = new Pattern(potentialPatterns[i].Key, matrices[i]);
	            pattern.AddInstance(potentialPatterns[i].Vertices);

	            for (int j = i + 1; j < potentialPatterns.Length; j++)
	            {
	                if (isomorphismGroups[j] != i) continue;
	                pattern.AddInstance(potentialPatterns[j].Vertices);
	            }
	            newPatterns.Add(pattern);
	        }

	        return newPatterns;
	    }

        /// <summary>
        /// Для каждой вершины находит количество исходящих рёбер, входящих рёбер и дуг.
        /// Результат сортируется по убыванию этих параметров.
        /// </summary>
        /// <param name="vertices">Массив вершин</param>
        /// <returns>Количество исходящих рёбер, входящих рёбер и дуг для каждой вершины.</returns>
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
