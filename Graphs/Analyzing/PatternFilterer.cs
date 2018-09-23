using System.Collections.Generic;
using System.Linq;
using SystemAnalyzer.Graphs.Patterns;
using SystemAnalyzer.Matrices;

namespace SystemAnalyzer.Graphs.Analyzing
{
    /// <summary>
    /// Позволяет фильтровать паттерны
    /// </summary>
    internal class PatternFilterer
    {
        private readonly PatternMap patternMap;
        private readonly AdjacencyMatrix matrix;

        public PatternFilterer(AdjacencyMatrix matrix, PatternMap patternMap)
        {
            this.matrix = matrix;
            this.patternMap = patternMap;
        }

        /// <summary>
        /// Удаляет паттерны, пересекающиеся с паттернами большего размера.
        /// </summary>
        /// <param name="size">Размер паттернов для удаления</param>
        public void RemoveIntersectingWithBiggerPatterns(int size)
        {
            foreach (var smallPattern in patternMap[size])
            {
                for (int m = size + 1; m <= patternMap.MaxPatternSize; m++)
                {
                    foreach (var bigPattern in patternMap[m])
                    {
                        smallPattern.RemoveIntersectingInstances(bigPattern);
                    }
                }
            }

        }

        /// <summary>
        /// Удаляет паттерны, имеющие менее двух экземпляров.
        /// </summary>
        /// <param name="size">Размер паттернов для удаления</param>
        public void RemoveSingletons(int size)
        {
            patternMap[size].RemoveAll(p => p.Instances.Count < 2);
        }
    }
}
