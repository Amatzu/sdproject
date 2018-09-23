using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemAnalyzer.Utils.Extensions
{
    internal static class EnumerableExtensions
    {
        public static bool None<T>(this IEnumerable<T> enumerable, Predicate<T> predicate)
        {
            return enumerable.All(item => !predicate(item));
        }
    }
}
