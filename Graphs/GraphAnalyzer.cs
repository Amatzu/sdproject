using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SystemAnalyzer.Matrices;

using Graph = QuickGraph.BidirectionalGraph<string, SystemAnalyzer.Graphs.Flow>;

namespace SystemAnalyzer.Graphs
{
	/// <summary>
	/// Представляет анализатор, выявляющий паттерны в графе и составляющий их библиотеку.
	/// </summary>
	internal class GraphAnalyzer
	{
		private readonly Graph graph;
	    private readonly AdjacencyMatrix matrix;

	    public PotentialPatternMap PotentialPatterns { get; private set; }
		public Dictionary<string, Pattern> PatternLibrary { get; private set; }

		public GraphAnalyzer(Graph graph)
		{
			this.graph = graph;
			PatternLibrary = new Dictionary<string, Pattern>();

		    matrix = AdjacencyMatrix.FromGraph(graph);

            //todo
		    PotentialPatterns = matrix.FindPotentialPatterns();
		}


	}
}
