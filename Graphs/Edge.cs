namespace SystemAnalyzer.Graphs
{
    internal struct Edge
    {
        public readonly Stock Source;
        public readonly Stock Target;

        private Edge(Stock source, Stock target)
        {
            Source = source;
            Target = target;
        }

        public static Edge FromFlow(Flow flow)
        {
            return new Edge(flow.Source, flow.Target);
        }
    }
}
