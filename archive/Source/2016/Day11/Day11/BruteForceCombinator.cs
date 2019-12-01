using System.Collections.Generic;

namespace Day11
{
    public class BruteForceCombinator : ICombinator
    {
        public string Name => "BruteForceCombinator";

        public IEnumerable<int[]> GetCombinations(State state, int elevatorYPos)
        {
            for (int x = 1; x < state.Width; x++)
            {
                if (state.GetData(x, elevatorYPos) == string.Empty) continue;

                yield return new[] { x };

                for (int x2 = x + 1; x2 < state.Width; x2++)
                {
                    if (state.GetData(x2, elevatorYPos) == string.Empty) continue;

                    yield return new[] { x, x2 };
                }
            }
        }
    }
}
