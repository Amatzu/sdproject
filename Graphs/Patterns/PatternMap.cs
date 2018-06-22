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
            patterns = new List<Pattern>[matrix.MaxMinorSize - 2];
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

        public void RemoveUniques(int size)
        {
            this[size].RemoveAll(p => p.Instances.Count < 2);
        }

        public void RemoveSelfIntersecting(int size)
        {
            var patternsToDelete = new List<Pattern>();
            foreach (var pattern in this[size])
            {
                foreach (var v in matrix.VertexMap)
                {
                    if (pattern.Instances.All(i => i.Vertices.Contains(v)))
                    {
                        patternsToDelete.Add(pattern);
                        break;
                    }
                }
            }

            this[size].RemoveAll(patternsToDelete.Contains);
        }

        public bool IsEmpty
        {
            get {
                for (int n = matrix.MaxMinorSize; n > 2; n--)
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
