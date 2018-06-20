namespace SystemAnalyzer.Graphs.Isomorphism
{
    /// <summary>
    /// Хранит информацию о количестве петель, входящих и выходящих рёбер в вершине графа.
    /// Используется как инвариант подграфа.
    /// </summary>
    internal readonly struct EdgeCount
    {
        public readonly int SelfEdges;
        public readonly int InEdges;
        public readonly int OutEdges;

        public EdgeCount(int selfEdges, int inEdges, int outEdges)
        {
            SelfEdges = selfEdges;
            InEdges = inEdges;
            OutEdges = outEdges;
        }
    }
}
