using System.Collections.Generic;

namespace Day11
{
    public class OptimizedCombinator : ICombinator
    {
        public IEnumerable<int[]> GetCombinations(State state, int elevatorYPos)
        {
            bool[] skip = new bool[state.Width];

            for (int x = 1; x < state.Width; x+=2)
            {
                for (int x2 = x + 2; x2 < state.Width; x2 += 2)
                    if (state.Storage[x].YPos == state.Storage[x2].YPos &&
                        state.Storage[x + 1].YPos == state.Storage[x2 + 1].YPos)
                    {
                        skip[x2] = true;
                        skip[x2+1] = true;
                    }
            }

            for (int x = 1; x < state.Width; x++)
            {
                if (state.GetData(x, elevatorYPos) == string.Empty) continue;

                if (!skip[x]) yield return new[] { x };

                for (int x2 = x + 1; x2 < state.Width; x2++)
                {
                    if (state.GetData(x2, elevatorYPos) == string.Empty) continue;

                    if (!skip[x] && !skip[x2])
                        yield return new[] { x, x2 };
                }
            }
        }
    }
}
