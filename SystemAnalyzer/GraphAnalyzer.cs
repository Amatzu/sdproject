using Graph = QuickGraph.BidirectionalGraph<string, SystemAnalyzer.Flow>;
using System.Collections.Generic;
using QuickGraph;

namespace SystemAnalyzer
{
	/// <summary>
	/// Представляет анализатор, выявляющий паттерны в графе и составляющий их библиотеку.
	/// </summary>
	internal class GraphAnalyzer
	{
		private readonly Graph graph;
		public Dictionary<string, Pattern> PatternLibrary { get; private set; }

		public GraphAnalyzer(Graph graph)
		{
			this.graph = graph;
			PatternLibrary = new Dictionary<string, Pattern>();
		}

		public void FindPatterns()
		{
			int maxPatternSize = graph.VertexCount / 2;

			//TODO: incindency matrix
		}
	}
}
