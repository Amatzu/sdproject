using System;
using System.Collections.Generic;
using System.Linq;
using SystemAnalyzer.Graphs.Patterns;
using SystemAnalyzer.Matrices;
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

        public void ReplacePatterns(PatternMap patterns)
        {
            for (int n = patterns.MaxPatternSize; n > 2; n--)
            {
                foreach (var pattern in patterns[n])
                {
                    int id = 0;
                    foreach (var instance in pattern.Instances)
                    {
                        var patternStock = new Stock("{" + pattern.Name + " #" + id + "}");

                        AddVertex(patternStock);

                        RemoveEdgeIf(e => instance.Vertices.Contains(e.Source.Name) &&
                                          instance.Vertices.Contains(e.Target.Name));

                        var flows = Edges.ToList();
                        foreach (var flow in flows)
                        {
                            if (instance.Vertices.Contains(flow.Source.Name))
                            {
                                RemoveEdge(flow);
                                flow.Source = patternStock;
                                AddEdge(flow);
                            }
                            if (instance.Vertices.Contains(flow.Target.Name))
                            {
                                RemoveEdge(flow);
                                flow.Target = patternStock;
                                AddEdge(flow);
                            }
                        }

                        RemoveVertexIf(v => instance.Vertices.Contains(v.Name));

                        id++;
                    }
                }
            }
        }

        public AdjacencyMatrix SubgraphMatrix(string[] includedVertices)
        {
            var subgraph = Subgraph(includedVertices);
            return AdjacencyMatrix.FromGraph(subgraph);
        }
    }
}
