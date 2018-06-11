namespace SystemAnalyzer.Utils
{
    internal static class MathUtils
    {
        public static long Factorial(int n)
        {
            long fact = 1;
            for (int i = 2; i <= n; i++)
            {
                fact *= i;
            }

            return fact;
        }

        /// <summary>
        /// Возвращает число сочетаний из n по k.
        /// </summary>
        /// <param name="n">Общее кол-во элементов в множестве</param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static int Combinations(int n, int m)
        {
            return (int) (Factorial(n) / (Factorial(m) * Factorial(n - m)));
        }
    }
}
