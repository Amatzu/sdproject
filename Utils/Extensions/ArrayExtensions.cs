namespace SystemAnalyzer.Utils.Extensions
{
    internal static class ArrayExtensions
    {
        public static T[] SkipIndex<T>(this T[] array, int skippedIndex)
        {
            var newArray = new T[array.Length - 1];

            int j = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (i == skippedIndex) continue;
                newArray[j] = array[i];
                j++;
            }

            return newArray;
        }
    }
}
