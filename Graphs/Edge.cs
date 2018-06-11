namespace SystemAnalyzer.Graphs
{
    internal struct Edge
    {
        public readonly string Source;
        public readonly string Target;

        private Edge(string source, string target)
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
