using System.Collections.Generic;

namespace Day11
{
    public interface ICombinator
    {
        IEnumerable<int[]> GetCombinations(State state, int elevatorYPos);
    }
}
