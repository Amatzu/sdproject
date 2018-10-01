using System;
using System.Collections.Generic;
using System.Linq;
using SystemAnalyzer.Matrices;

namespace SystemAnalyzer.Graphs.Patterns
{
    public class PatternMap
    {
        private readonly AdjacencyMatrix matrix;
        private readonly List<Pattern>[] patterns;

        public int MaxPatternSize => patterns.Length + 2;

        public PatternMap(AdjacencyMatrix matrix)
        {
            this.matrix = matrix;
            patterns = new List<Pattern>[matrix.Vertices - 2];
            for (int i = 0; i < patterns.Length; i++)
            {
                patterns[i] = new List<Pattern>();
            }
        }

        public int TotalInstances
        {
            get {
                int count = 0;
                for (int i = 3; i <= MaxPatternSize; i++)
                {
                    count += this[i].Sum(p => p.Instances.Count);
                }

                return count;
            }
        }

        public bool IsEmpty
        {
            get {
                for (int n = matrix.Vertices; n > 2; n--)
                {
                    if (this[n].Count > 0) return false;
                }

                return true;
            }
        }

        public void ClearInstances()
        {
            for (int n = 3; n <= MaxPatternSize; n++)
            {
                this[n].Clear();
            }
        }

        public List<Pattern> this[int size]
        {
            get => patterns[size - 3];
            set => patterns[size - 3] = value;
        }

    }
}
