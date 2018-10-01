using System;

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

        public static T[] RemoveItem<T>(this T[] array, T removedItem)
        {
            var newArray = new T[array.Length - 1];

            int j = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(removedItem)) continue;
                newArray[j] = array[i];
                j++;
            }

            return newArray;
        }

        /// <summary>
        /// Инициализирует все элементы массива заданным значением.
        /// </summary>
        public static void InitializeWith<T>(this T[] array, T item)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = item;
            }
        }

        /// <summary>
        /// Определяет, удовлетворяют ли любые два различных элемента массива условию.
        /// Предикат должен быть ассоциативным.
        /// </summary>
        public static bool AnyTwo<T>(this T[] array, Func<T, T, bool> predicate)
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                for (int j = i + 1; j < array.Length; j++)
                {
                    if (predicate(array[i], array[j])) return true;
                }
            }

            return false;
        }
    }
}
