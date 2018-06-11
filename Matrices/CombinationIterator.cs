using System.Collections.Generic;

namespace SystemAnalyzer.Matrices
{
    internal struct CombinationIterator
    {
        private readonly int[] counters;
        private readonly int init;
        private readonly int max;

        public IEnumerable<int[]> Iterations
        {
            get
            {
                do
                {
                    yield return counters;
                } while (!IncrementCounter(loopIndex: counters.Length - 1));
                ResetCounters();
            }
        }

        //TODO: exception
        public CombinationIterator(int init, int max, int loopCount)
        {
            this.init = init;
            this.max = max;

            counters = new int[loopCount];
            ResetCounters();
        }

        private void ResetCounters()
        {
            for (int i = 0; i < counters.Length; i++)
                counters[i] = init + i;
        }

        private bool IncrementCounter(int loopIndex)
        {
            int currentMax = max - (counters.Length - loopIndex);

            if (counters[loopIndex] <= currentMax)
            {
                counters[loopIndex]++;
            }
            else
            {
                if (loopIndex == 0 || IncrementCounter(loopIndex - 1)) return true;

                int previous = counters[loopIndex - 1];
                if (previous < currentMax) counters[loopIndex] = previous + 1;
            }

            return false;
        }

    }
}
