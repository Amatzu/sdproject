using System;
using System.Linq;
using SystemAnalyzer.Utils;

namespace SystemAnalyzer.Graphs.Patterns
{
    public readonly struct PatternInstance
    {
        public readonly string Name;
        public readonly Pattern Pattern;
        public readonly string[] Vertices;

        public PatternInstance(Pattern pattern, string[] vertices)
        {
            Name = pattern.Name + " #" + pattern.Instances.Count();
            Pattern = pattern;
            Vertices = vertices;
        }

        public static DisjointSet GetIntersectionGroups(PatternInstance[] instances)
        {
            var disjointSet = new DisjointSet(instances.Length);
            for (int i = 0; i < instances.Length; i++)
                disjointSet.MakeSet(i);

            for (int i = 0; i < instances.Length - 1; i++)
            {
                for (int j = 1; j < instances.Length; j++)
                {
                    if (instances[i].Intersects(instances[j]))
                    {
                        disjointSet.Union(i, j);
                    }
                }
            }

            return disjointSet;
        }

        public bool Intersects(PatternInstance other)
        {
            return Vertices.Any(v => Array.IndexOf(other.Vertices, v) != -1);
        }
    }
}
