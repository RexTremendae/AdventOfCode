using static System.Console;

public enum Amphipod
{
    A1, A2,
    B1, B2,
    C1, C2,
    D1, D2
};

namespace AoC
{
    using State = Dictionary<Amphipod, string>;
    using MovementGraph = Dictionary<string, (string dst, int len, Amphipod[] filter, string[] passThrough)[]>;

    public static class StateExtensions
    {
        public static bool IsSameAs(this State state, State other)
        {
            var statePositions = new Dictionary<char, List<string>>();
            var otherPositions = new Dictionary<char, List<string>>();

            foreach (var type in "ABCD")
            {
                statePositions[type] = new();
                otherPositions[type] = new();
            }

            foreach (var amphi in Enum.GetValues<Amphipod>())
            {
                var type = amphi.ToString()[0];
                statePositions[type].Add(state[amphi]);
                otherPositions[type].Add(other[amphi]);
            }

            foreach (var type in "ABCD")
            {
                var stp = statePositions[type];
                var otp = otherPositions[type];

                stp.Sort();
                otp.Sort();

                if (otp.Count != stp.Count) throw new InvalidOperationException();

                for (int i = 0; i < stp.Count; i++)
                {
                    if (stp[i] != otp[i]) return false;
                }
            }

            return true;
        }

        public static long GetHeuristic(this State state)
        {
            // 0.1.2.3.4.5.6
            //    A B C D

            var heuristicKey = "01A2B3C4D56";
            var heuristic = 0;

            foreach (var (key, value) in state)
            {
                var amphi = key.ToString()[0];
                var amphiIndex = heuristicKey.IndexOf(amphi);
                var positionIndex = heuristicKey.IndexOf(value[0]);
                var amphiValue =
                    amphi switch
                    {
                        'A' => 1,
                        'B' => 10,
                        'C' => 100,
                        'D' => 1000,
                        _ => throw new InvalidOperationException()
                    };
                heuristic += Math.Abs(amphiIndex - positionIndex) * amphiValue;
            }

            return heuristic;
        }

        public static bool IsGoal(this State state)
        {
            return GetHeuristic(state) == 0;
        }
    }

    public class Day23
    {
        public void Part1()
        {
            // #############
            // #...........#
            // ###B#C#B#D###
            //   #A#D#C#A#  
            //   #########  

            // 0 1  2  3  4  5 6
            //    A1 B1 C1 D1
            //    A2 B2 C2 D2

            var state = new State
            {
                { Amphipod.B1, "A1" }, { Amphipod.C1, "B1" }, { Amphipod.B2, "C1" }, { Amphipod.D2, "D1" },
                { Amphipod.A1, "A2" }, { Amphipod.D1, "B2" }, { Amphipod.C2, "C2" }, { Amphipod.A2, "D2" }

                //{ Amphipod.B1, "A1" }, { Amphipod.B2, "B1" }, { Amphipod.D1, "C1" }, { Amphipod.C1, "D1" },
                //{ Amphipod.A1, "A2" }, { Amphipod.A2, "B2" }, { Amphipod.C2, "C2" }, { Amphipod.D2, "D2" }
            };

            Clock.Tick();
            var sln = Solve(state);
            WriteLine(sln);
            WriteLine($"{Clock.Tock().TotalSeconds:0.00} s");
        }

        private long Solve(State state)
        {
            Clock.Tick();

            var visited = new List<State>();
            var q = new List<(long heuristic, State state, string src, string dst, long energy, (string from, string to)[] path, State[] visited)>
            {
                (0, state, "", "", 0, new (string, string)[]{}, new State[] {})
            };

            var movementGraph = GetMovementGraph();

            var lastSeconds = 0;

            while (q.Any())
            {
                var seconds = (int)Clock.Tock().TotalSeconds;
                if (seconds > lastSeconds)
                {
                    CursorLeft = 0;
                    Write($"{seconds.ToString(),3} s - {q[0].heuristic}");
                    lastSeconds = seconds;
                }

                var dq = q.First();
                q.RemoveAt(0);

                foreach (var (amphi, src, dst, len) in GetPossibleMoves(dq.state, movementGraph))
                {
                    var energy = dq.energy + (amphi.ToString()[0] switch
                    {
                        'A' => 1,
                        'B' => 10,
                        'C' => 100,
                        'D' => 1000,
                        _ => throw new InvalidOperationException()
                    }) * len;

                    state = dq.state.ToDictionary(_ => _.Key, _ => _.Value);
                    state.Remove(amphi);
                    state[amphi] = dst;

                    var alreadyVisited = false;
                    foreach (var visit in dq.visited)
                    {
                        if (visit.IsSameAs(state))
                        {
                            alreadyVisited = true;
                            break;
                        }
                    }

                    if (alreadyVisited) continue;

                    var path = ((string from, string to)[])dq.path.Concat(new[] {(src, dst)}).ToArray();
                    if (state.IsGoal())
                    {
                        WriteLine($"[{string.Join(" ", path.Select(_ => $"[{_.from} => {_.to}]"))}]");
                        return energy;
                    }

                    var heuristic = state.GetHeuristic() + energy;
                    var idx = 0;
                    while (idx < q.Count && q[idx].heuristic < heuristic) idx++;

                    q.Insert(idx, (heuristic, state, src, dst, energy, path, dq.visited.Concat(new[] { state }).ToArray()));
                }
            }

            return -1;
        }

        private IEnumerable<(Amphipod amphi, string src, string dst, int len)> GetPossibleMoves(State state,
            Dictionary<string, (string dst, int len, Amphipod[] filter, string[] passThrough)[]> movementGraph)
        {
            var occupied = state.Values.ToHashSet();
            foreach (var amphi in Enum.GetValues<Amphipod>())
            {
                var src = state[amphi];
                foreach (var (dst, len, filter, passThrough) in movementGraph[src])
                {
                    var amphiType = amphi.ToString()[0];

                    if (amphiType == src[0])
                    {
                        if (src[1] == '2')
                        {
                            continue;
                        }

                        if (state.Any(_ =>
                                _.Value[0] == amphiType && _.Value[1] == '2' && _.Key.ToString()[0] == amphiType))
                        {
                            continue;
                        }
                    }

                    if (!filter.Contains(amphi))
                    {
                        continue;
                    }

                    if (passThrough.Any(_ => occupied.Contains(_)))
                    {
                        continue;
                    }

                    if (occupied.Contains(dst))
                    {
                        continue;
                    }

                    yield return (amphi, src, dst, len);
                }
            }
        }

        private MovementGraph GetMovementGraph()
        {
            // 0 1  2  3  4  5 6
            //    A1 B1 C1 D1
            //    A2 B2 C2 D2

            //  01 2 3 4 56
            // #...........#
            // ###.#.#.#.###
            //   #.#.#.#.#
            //   #########
            //    A B C D

            var possibleMoves = new (string src, string dst, int len, string[] passThrough)[]
            {
                ("0", "A1",  3, new[] { "1" }),
                ("0", "A2",  4, new[] { "1", "A1" }),
                ("0", "B1",  5, new[] { "1", "2" }),
                ("0", "B2",  6, new[] { "1", "2", "B1" }),
                ("0", "C1",  7, new[] { "1", "2", "3" }),
                ("0", "C2",  8, new[] { "1", "2", "3", "C1" }),
                ("0", "D1",  9, new[] { "1", "2", "3", "4" }),
                ("0", "D2", 10, new[] { "1", "2", "3", "4", "D1" }),

                ("1", "A1",  2, new string[] {}),
                ("1", "A2",  3, new[] { "A1" }),
                ("1", "B1",  4, new[] { "2" }),
                ("1", "B2",  5, new[] { "2", "B1" }),
                ("1", "C1",  6, new[] { "2", "3" }),
                ("1", "C2",  7, new[] { "2", "3", "C1" }),
                ("1", "D1",  8, new[] { "2", "3", "4" }),
                ("1", "D2",  9, new[] { "2", "3", "4", "D1" }),

                ("2", "A1",  2, new string[] {}),
                ("2", "A2",  3, new[] { "A1" }),
                ("2", "B1",  2, new string[] {}),
                ("2", "B2",  3, new[] { "B1" }),
                ("2", "C1",  4, new[] { "3" }),
                ("2", "C2",  5, new[] { "3", "C1" }),
                ("2", "D1",  6, new[] { "3", "4" }),
                ("2", "D2",  7, new[] { "3", "4", "D1" }),

                ("3", "A1",  4, new[] { "2" }),
                ("3", "A2",  5, new[] { "2", "A1" }),
                ("3", "B1",  2, new string[] {}),
                ("3", "B2",  3, new[] { "B1" }),
                ("3", "C1",  2, new string[] {}),
                ("3", "C2",  3, new[] { "C1" }),
                ("3", "D1",  4, new[] { "4" }),
                ("3", "D2",  5, new[] { "4", "D1" }),

                ("4", "A1",  6, new[] { "2", "3" }),
                ("4", "A2",  7, new[] { "2", "3", "A1" }),
                ("4", "B1",  4, new[] { "3" }),
                ("4", "B2",  5, new[] { "3", "B1" }),
                ("4", "C1",  2, new string[] {}),
                ("4", "C2",  3, new[] { "C1" }),
                ("4", "D1",  2, new string[] {}),
                ("4", "D2",  3, new[] { "D1" }),

                ("5", "A1",  8, new[] { "2", "3", "4" }),
                ("5", "A2",  9, new[] { "2", "3", "4", "A1" }),
                ("5", "B1",  6, new[] { "3", "4" }),
                ("5", "B2",  7, new[] { "3", "4", "B1" }),
                ("5", "C1",  4, new[] { "4" }),
                ("5", "C2",  5, new[] { "4", "C1" }),
                ("5", "D1",  2, new string[] {}),
                ("5", "D2",  3, new[] { "D1" }),

                ("6", "A1",  9, new[] { "2", "3", "4", "5" }),
                ("6", "A2", 10, new[] { "2", "3", "4", "5", "A1" }),
                ("6", "B1",  7, new[] { "3", "4", "5" }),
                ("6", "B2",  8, new[] { "3", "4", "5", "B1" }),
                ("6", "C1",  5, new[] { "4", "5" }),
                ("6", "C2",  6, new[] { "4", "5", "C1" }),
                ("6", "D1",  3, new[] { "5" }),
                ("6", "D2",  4, new[] { "5", "D1" }),

                ("A1", "B1",  4, new string[] { "2" }),
                ("A2", "B1",  5, new string[] { "A1", "2" }),
                ("A1", "B2",  5, new string[] { "2", "B1" }),
                ("A2", "B2",  6, new string[] { "A1", "2", "B1" }),
                ("A1", "C1",  6, new string[] { "2", "3" }),
                ("A2", "C1",  7, new string[] { "A1", "2", "3" }),
                ("A1", "C2",  7, new string[] { "2", "3", "C1" }),
                ("A2", "C2",  8, new string[] { "A1", "2", "3", "C1" }),
                ("A1", "D1",  8, new string[] { "2", "3", "4" }),
                ("A2", "D1",  9, new string[] { "A1", "2", "3", "4" }),
                ("A1", "D2",  9, new string[] { "2", "3", "4", "D1" }),
                ("A2", "D2", 10, new string[] { "A1", "2", "3", "4", "D1" }),

                ("B1", "C1",  4, new string[] { "3" }),
                ("B2", "C1",  5, new string[] { "B1", "3" }),
                ("B1", "C2",  5, new string[] { "3", "C1" }),
                ("B2", "C2",  6, new string[] { "B1", "3", "C1" }),
                ("B1", "D1",  6, new string[] { "3", "4" }),
                ("B2", "D1",  7, new string[] { "B1", "3", "4" }),
                ("B1", "D2",  7, new string[] { "3", "4", "D1" }),
                ("B2", "D2",  8, new string[] { "B1", "3", "4", "D1" }),

                ("C1", "D1",  4, new string[] { "4" }),
                ("C2", "D1",  5, new string[] { "C1", "4" }),
                ("C1", "D2",  5, new string[] { "4", "D1" }),
                ("C2", "D2",  6, new string[] { "C1", "4", "D1" }),
            }.ToList();

            var possibleMovesList = possibleMoves.Select(_ => (_.src, _.dst, _.len,
                filter: Enum.GetValues<Amphipod>()
                    .Where(a => a.ToString()[0] == _.dst[0])
                    .ToArray(),
                _.passThrough))
                .ToList();

            foreach (var (src, dst, len, passThrough) in possibleMoves.ToList())
            {
                possibleMovesList.Add((dst, src, len,
                    filter: Enum.GetValues<Amphipod>()
                        .Where(_ => char.IsNumber(src[0]) || _.ToString()[0] == src[0]) // yes, src - it's backwards!
                        .ToArray(),
                passThrough.ToArray()));
            }

            return possibleMovesList
                .GroupBy(_ => _.src)
                .ToDictionary(key => key.Key, value => value
                    .Select(_ => (_.dst, _.len, _.filter, _.passThrough))
                    .ToArray());
        }
    }
}
