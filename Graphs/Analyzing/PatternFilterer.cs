using SystemAnalyzer.Graphs.Patterns;

namespace SystemAnalyzer.Graphs.Analyzing
{
    /// <summary>
    /// Представляет фильтратор, отсеивающий паттерны с низким приоритетом.
    /// </summary>
    internal class PatternFilterer
    {
        private readonly PatternMap patternMap;

        public PatternFilterer(PatternMap patternMap)
        {
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
