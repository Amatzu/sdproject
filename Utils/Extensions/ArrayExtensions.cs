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

        public static T[] SelectIndices<T>(this T[] array, int[] selectedIndices)
        {
            var newArray = new T[selectedIndices.Length];

            for (int i = 0; i < selectedIndices.Length; i++)
            {
                int j = selectedIndices[i];
                newArray[i] = array[j];
            }

            return newArray;
        }
    }
}
