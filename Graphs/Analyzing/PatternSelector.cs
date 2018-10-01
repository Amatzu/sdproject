using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SystemAnalyzer.Graphs.Patterns;
using SystemAnalyzer.Utils.Extensions;

namespace SystemAnalyzer.Graphs.Analyzing
{
    /// <summary>
    /// Выбирает наибольшее возможное количество непересекающихся экземпляров паттернов.
    /// </summary>
    internal class PatternSelector
    {
        private readonly PatternMap patterns;

        public PatternSelector(PatternMap patterns)
        {
            this.patterns = patterns;
        }

        /// <summary>
        /// Из каждой группы пересекающихся паттернов выбирает как можно большее количество.
        /// </summary>
        public void SelectInstances(int size)
        {
            if (patterns[size].Count == 0) return;

            var allInstances      = patterns[size].SelectMany(p => p.Instances).ToArray();
            var selectedInstances = new List<PatternInstance>();

            var instanceGroups = GroupInstances(allInstances);

            foreach (var group in instanceGroups)
            {
                var instances = SelectInstancesFromGroup(group);
                selectedInstances.AddRange(instances);
            }

            foreach (var pattern in patterns[size])
            {
                pattern.Instances.RemoveAll(i => !selectedInstances.Contains(i));
            }
        }

        /// <summary>
        /// Группирует экземпляры паттернов по пересечениям.
        /// </summary>
        private IEnumerable<PatternInstance[]> GroupInstances(PatternInstance[] instances)
        {
            var disjointSet = PatternInstance.GetIntersectionGroups(instances);

            var instancesByGroup = instances.Select((x, i) => new {
                Value = x,
                Group = disjointSet.Find(i)
            });
            var groups      = instancesByGroup.GroupBy(i => i.Group);
            var groupValues = groups.Select(g => g.Select(h => h.Value).ToArray());

            return groupValues;
        }

        /// <summary>
        /// Выбирает максимально возможное количество непересекающихся экземпляров паттернов
        /// из группы.
        /// </summary>
        private PatternInstance[] SelectInstancesFromGroup(PatternInstance[] instances)
        {
            Debug.Assert(instances != null);
            Debug.Assert(instances.Length > 0);

            if (instances.Length == 1) return instances;

            while (instances.AnyTwo((i, j) => i.Intersects(j)))
            {
                var sortedInstances = from i in instances
                                      orderby instances.Count(i.Intersects) descending,
                                              i.Pattern.EdgeCount ascending
                                      select i;

                instances = instances.RemoveItem(sortedInstances.First());
            }

            return instances;
        }
    }
}
