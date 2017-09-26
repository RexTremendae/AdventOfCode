using System.Collections.Generic;

namespace Day11
{
    public interface ICombinator
    {
        string Name { get; }
        IEnumerable<int[]> GetCombinations(State state, int elevatorYPos);
    }
}
