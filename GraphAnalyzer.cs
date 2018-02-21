using Graph = QuickGraph.BidirectionalGraph<string, sdproject.Flow>;
using System.Collections.Generic;

namespace sdproject
{
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
