using System.Linq;
using SystemAnalyzer.Matrices;
using SystemAnalyzer.Utils;
using QuickGraph;

namespace SystemAnalyzer.Graphs.Isomorphism
{
    /// <summary>
    /// Проверяет графы на изоморфизм.
    /// </summary>
    internal class IsomorphismChecker
    {
        private readonly AdjacencyMatrix mainMatrix;

        public IsomorphismChecker(AdjacencyMatrix matrix)
        {
            mainMatrix = matrix;
        }

        public bool Check(AdjacencyMatrix matrix)
        {
            var array = Enumerable.Range(0, matrix.Vertices).ToArray();
            var perms = Permutations.Find(array);

            foreach (int[] alpha in perms)
            {
                bool result = true;

                for (int i = 0; i < matrix.Vertices; i++)
                {
                    for (int j = 0; j < matrix.Vertices; j++)
                    {
                        int u = alpha[i];
                        int v = alpha[j];

                        if (mainMatrix[i, j] != matrix[u, v])
                        {
                            result = false;
                            goto outerLoop;
                        }
                    }
                }
                outerLoop:

                if (result) return true;
            }

            return false;
        }
    }
}
