using System.Collections.Generic;

namespace Day11
{
    public class OptimizedCombinator : ICombinator
    {
        private class Pair
        {
            public int Generator;
            public int Microship;

            public bool Equals(Pair otherPair, Dictionary<int, Pair> pairs, State state)
            {
                if (otherPair == null) return false;

                return state.Storage[Generator].YPos == state.Storage[otherPair.Generator].YPos &&
                       state.Storage[Microship].YPos == state.Storage[otherPair.Microship].YPos;
            }

            public override string ToString()
            {
                return $"G{Generator} M{Microship}";
            }
        }

        private class Combination
        {
            public Combination(int[] combo, State state, Dictionary<int, Pair> pairs)
            {
                Combo = combo;
                _state = state;
                _pairs = pairs;

                Pair1 = pairs[combo[0]];
                if (combo.Length > 1)
                {
                    Pair2 = pairs[combo[1]];
                }
            }

            private State _state;
            private Dictionary<int, Pair> _pairs;
            public Pair Pair1, Pair2;
            public int[] Combo;

            public bool IsSameAs(Combination otherCombo)
            {
                if (Pair2 == null)
                {
                    if (otherCombo.Pair2 != null) return false;

                    return Pair1.Equals(otherCombo.Pair1, _pairs, _state) &&
                           Combo[0] % 2 == otherCombo.Combo[0] % 2;
                }
                else
                {
                    if (Pair1.Equals(otherCombo.Pair1, _pairs, _state) &&
                        Pair2.Equals(otherCombo.Pair2, _pairs, _state) &&
                        Combo[0] % 2 == otherCombo.Combo[0] % 2 &&
                        Combo[1] % 2 == otherCombo.Combo[1] % 2)
                    {
                        return true;
                    }

                    if (Pair1.Equals(otherCombo.Pair2, _pairs, _state) &&
                        Pair2.Equals(otherCombo.Pair1, _pairs, _state) &&
                        Combo[0] % 2 == otherCombo.Combo[1] % 2 &&
                        Combo[1] % 2 == otherCombo.Combo[0] % 2)
                    {
                        return true;
                    }

                    return false;
                }
            }
        }

        public IEnumerable<int[]> GetCombinations(State state, int elevatorYPos)
        {
            var previousCombinations = new List<Combination>();
            var pairs = new Dictionary<int, Pair>();

            for (int x = 1; x < state.Width; x+=2)
            {
                Pair p = new Pair { Generator = x, Microship = x+1 };
                pairs.Add(x, p);
                pairs.Add(x+1, p);
            }

            for (int x = 1; x < state.Width; x++)
            {
                if (state.GetData(x, elevatorYPos) == string.Empty) continue;
                bool combinationAlreadyExists = false;
                Combination combo = new Combination(new[] { x }, state, pairs);

                foreach (var c in previousCombinations)
                {
                    if (c.IsSameAs(combo))
                    {
                        combinationAlreadyExists = true;
                        break;
                    }
                }

                if (!combinationAlreadyExists)
                {
                    yield return combo.Combo;
                    previousCombinations.Add(combo);
                }

                for (int x2 = x + 1; x2 < state.Width; x2++)
                {
                    if (state.GetData(x2, elevatorYPos) == string.Empty) continue;

                    combinationAlreadyExists = false;
                    combo = new Combination(new[] { x, x2 }, state, pairs);
                    foreach (var c in previousCombinations)
                    {
                        if (c.IsSameAs(combo))
                        {
                            combinationAlreadyExists = true;
                            break;
                        }
                    }

                    if (!combinationAlreadyExists)
                    {
                        previousCombinations.Add(combo);
                        yield return combo.Combo;
                    }
                }
            }
        }

        private bool IsValidCombination(int[] combo, List<int[]> previousCombinations, State state)
        {
            var rawState = state.Expand();

            bool comboIsSamePair = IsSamePair(combo);

            foreach (var prevCombo in previousCombinations)
            {
                bool prevComboIsSamePair = IsSamePair(prevCombo);

                if (comboIsSamePair && prevComboIsSamePair)
                {
                    if (state.Storage[combo[0]].YPos == state.Storage[prevCombo[0]].YPos) return false;
                }
            }

            return true;
        }

        private bool IsSamePair(int[] combo)
        {
            int cx_1 = combo[0];
            int cx_2 = combo.Length > 1 ? combo[1] : 0;

            return (cx_1 % 2 != 0) && (cx_2 == cx_1 + 1);
        }
    }
}
