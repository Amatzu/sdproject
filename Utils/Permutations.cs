using System.Collections.Generic;

namespace SystemAnalyzer.Utils
{
    internal static class Permutations
    {
        private static void Swap (ref int a, ref int b)
        {
            if(a == b) return;

            int temp = a;
            a = b;
            b = temp;
        }

        public static IEnumerable<int[]> Find(int[] array)
        {
            int last = array.Length - 1;
            return Iterate(array, 0, last);
        }

        private static IEnumerable<int[]> Iterate (int[] array, int k, int m)
        {
            if (k == m) yield return array;
            else
            {
                for (int i = k; i <= m; i++)
                {
                    Swap (ref array[k], ref array[i]);
                    foreach (var perm in Iterate (array, k+1, m))
                    {
                        yield return perm;
                    }
                    Swap (ref array[k], ref array[i]);
                }
            }
        }
    }
}
