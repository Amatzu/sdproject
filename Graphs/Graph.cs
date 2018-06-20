using System.Collections.Generic;
using System.Linq;
using QuickGraph;

namespace SystemAnalyzer.Graphs
{
    public class Graph : BidirectionalGraph<Stock, Flow>
    {
        public readonly Stock DefaultStock;

        public Graph(bool allowParallelEdges, Stock defaultStock) : base(allowParallelEdges)
        {
            DefaultStock = defaultStock;
        }

        public new IEnumerable<Stock> Vertices
        {
            get => base.Vertices.Where(stock => !Equals(stock, DefaultStock));
        }

        public IEnumerable<Flow> InnerFlows
        {
            get => Edges.Where(flow => !flow.IsAdjacent(DefaultStock));
        }

        public IEnumerable<Flow> SelfFlows
        {
            get => InnerFlows.Where(flow => flow.IsSelfEdge<Stock, Flow>());
        }

        public IEnumerable<Flow> NonSelfFlows
        {
            get => InnerFlows.Where(flow => !flow.IsSelfEdge<Stock, Flow>());
        }

        public Graph Subgraph(string[] includedVertices)
        {
            var vertices = Vertices.Where(v => includedVertices.Contains(v.Name)).ToArray();
            var edges = from edge in InnerFlows
                        where vertices.Contains(edge.Source) &&
                              vertices.Contains(edge.Target)
                        select edge;

            var subgraph = new Graph(true, DefaultStock);
            subgraph.AddVertexRange(vertices);
            subgraph.AddEdgeRange(edges);

            return subgraph;
        }

        public IEnumerable<Stock> Neighbors(Stock stock)
        {
            var neighbors = from flow in InnerFlows
                            where !flow.IsSelfEdge<Stock, Flow>() &&
                                  flow.IsAdjacent(stock)
                            select flow.GetOtherVertex(stock);

            return neighbors;
        }
    }
}
