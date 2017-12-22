using System;
using System.Collections.Generic;
using System.Linq;

using static System.Console;

namespace Day11
{
    public class Program
    {
        private static Dictionary<string, double> _measurements;

        public static void Main()
        {
            new Program().Run();
        }

        static Program()
        {
            _measurements = new Dictionary<string, double>();
        }

        public void Run()
        {
            var small = new State(0, new[] { 1,0,  2,0 });
            // F4  |  .  .    .  .
            // F3  |  .  .    LG .
            // F2  |  HG .    .  .
            // F1  E  .  HM   .  LM

            var mid = new State(0, new[] { 0,1,  0,0,  0,1,  0,0 });
            // F4  |   .   .     .   .     .   .     .   .
            // F3  |   .   .     .   .     .   .     .   .
            // F2  |   .  PoM    .   .     .  PrM    .   .
            // F1  E  PoG  .     TG  TM   PrG  .     RG  RM

            var large = new State(0, new[] { 0,1,  0,0,  0,1,  0,0,  0,0 });
            // F4  |   .   .     .   .     .   .     .   .     .   .
            // F3  |   .   .     .   .     .   .     .   .     .   .
            // F2  |   .  PoM    .   .     .  PrM    .   .     .   .
            // F1  E  PoG  .     TG  TM   PrG  .     RG  RM    CG  CM

            var verylarge = new State(0, new[] { 0,1,  0,0,  0,1,  0,0,  0,0,  0,0,  0,0 });
            // F4  |   .   .     .   .     .   .     .   .     .   .     .   .     .   . 
            // F3  |   .   .     .   .     .   .     .   .     .   .     .   .     .   .
            // F2  |   .  PoM    .   .     .  PrM    .   .     .   .     .   .     .   .
            // F1  E  PoG  .     TG  TM   PrG  .     RG  RM    CG  CM    EG  EM    DG  DM

            WriteLine($"Small:     {Measure<int>(nameof(small), () => Solve(small))} moves");
            WriteLine($"Mid:       {Measure<int>(nameof(mid), () => Solve(mid))} moves");
            WriteLine($"Large:     {Measure<int>(nameof(large), () => Solve(large))} moves");
            WriteLine($"veryLarge: {Measure<int>(nameof(verylarge), () => Solve(verylarge))} moves");

            WriteLine();
            WriteLine("Measurements");
            WriteLine("============");

            var longest = 0;
            foreach (var key in _measurements.Keys)
            {
                if (key.Length > longest) longest = key.Length;
            }

            foreach (var kvp in _measurements.OrderByDescending(x => x.Value))
            {
                WriteLine($"{kvp.Key.PadRight(longest+1)}: {kvp.Value:0.00}s");
            }
        }

        public int Solve(State initial)
        {
            var open = new StatePrioQ();
            Measure("EnQ", () => open.EnQ(initial.Heuristic, initial));

            var closed = new Dictionary<State, int>();

            for (;;)
            {
                if (open.IsEmpty()) return -1;
                var state = Measure("DeQ", () => open.DeQ());
                closed.Add(state, state.TotalMovement);

                /*Write("DeQ: ");
                foreach (var st in state.Data)
                {
                    Write($"{st} ");
                }
                WriteLine();
                */
                if (state.IsGoal)
                {
                    return state.TotalMovement;
                    /*
                    int cost = state.TotalMovement;
                    var visitedStates = new List<string>();
                    while(state != null)
                    {
                        visitedStates.Insert(0, state.ToString());
                        state = state.Prev;
                    }

                    return cost;
                    */
                }

                var nextStatesResult = Measure("GetNextStates", () => GetNextStates(state));
                foreach (var next in nextStatesResult)
                {
                    if (next.IsFried) continue;

                    // var inOpen = open.GetCost(next).HasValue;
                    // var inClosed = closed.ContainsKey(next);

                    var existingOpenCost = Measure("GetCost", () => open.GetCost(next));
                    if (existingOpenCost.HasValue && existingOpenCost.Value > next.TotalMovement)
                        Measure("Remove", () => open.Remove(next));

                    if (closed.TryGetValue(next, out var existingClosedCost) && existingClosedCost > next.TotalMovement)
                    {
                        Measure("Remove", () => closed.Remove(next));
                    }

                    var cost = Measure("GetCost", () => open.GetCost(next));
                    if (!cost.HasValue && !closed.ContainsKey(next))
                    {
                        Measure("EnQ", () => open.EnQ(next.TotalMovement + next.Heuristic, next));
                        /*Write("EnQ: ");
                        foreach (var st in next.Data)
                        {
                            Write($"{st} ");
                        }
                        WriteLine();
                        */
                    }
                }
            }
        }

        public static IEnumerable<State> GetNextStates(State state)
        {
            for (int x1 = 0; x1 < state.Data.Length; x1++)
            {
                (var up, var down) = GetNextStates(state, x1);
                if (up == null && down == null) continue;
                if (up != null) yield return up;
                if (down != null) yield return down;

                for (int x2 = 0; x2 < state.Data.Length; x2++)
                {
                    if (x1 == x2) continue;

                    (up, down) = GetNextStates(state, x1, x2);
                    if (up != null) yield return up;
                    if (down != null) yield return down;
                }
            }
        }

        public static (State up, State down) GetNextStates(State state, int x1)
        {
            State up = null;
            State down = null;

            if (state.Data[x1] != state.Elevator) return (up, down);

            if (state.Elevator > 0)
            {
                down = Clone(state, -1, new[]{x1});
            }
            if (state.Elevator < 3)
            {
                up = Clone(state, +1, new[]{x1});
            }

            return (up, down);
        }

        public static (State up, State down) GetNextStates(State state, int x1, int x2)
        {
            State up = null;
            State down = null;

            if (state.Data[x1] != state.Elevator || state.Data[x2] != state.Elevator) return (up, down);

            if (state.Elevator > 0)
            {
                down = Clone(state, -1, new[]{x1, x2});
            }
            if (state.Elevator < 3)
            {
                up = Clone(state, +1, new[]{x1, x2});
            }

            return (up, down);
        }

        private static State Clone(State state, int delta, int[] indices)
        {
            if (indices.Length == 2 && indices[0] == indices[1])
                throw new InvalidOperationException();

            var cloneData = new int[state.Data.Length];
            for (int d = 0; d < state.Data.Length; d++) cloneData[d] = state.Data[d];

            foreach (var idx in indices)
                cloneData[idx] += delta;

            var cloneState = new State(state.Elevator + delta, cloneData)
            {
                TotalMovement = state.TotalMovement + 1,
                Prev = state
            };

            return cloneState;
        }

        private void Measure(string name, Action action)
        {
            Measure<bool>(name, () => {
                action();
                return true;
            });
        }

        private T Measure<T>(string name, Func<T> action)
        {
            var start = DateTime.Now;
            var x = action();
            var end = DateTime.Now;
            var durationPart = (end - start).TotalSeconds; //.ToString("0.00");

            if (_measurements.TryGetValue(name, out var duration))
            {
                _measurements[name] = duration + durationPart;
            }
            else
            {
                _measurements[name] = durationPart;
            }
            //name = (name + ":").PadRight(8);
            //WriteLine($"{name} solution = {x}, duration = {duration}s");

            return x;
        }
    }

    public class State
    {
        public int[] Data { get; }
        public int[] SortedData { get; private set; }
        public int Elevator { get; }
        public State Prev { get; set; }
        public int TotalMovement { get; set; }

        public bool IsFried { get; private set; }
        public bool IsGoal { get; private set; }
        public int Heuristic { get; private set; }

        public State(int elevator, params int[] data)
        {
            if (data.Any(x => x > 3))
                throw new InvalidOperationException();

            Data = data;
            Elevator = elevator;

            CalculateHeuristic();
            CalculateIsGoal();
            CalculateIsFried();
            SortData();
        }

        private void SortData()
        {
            SortedData = new int[Data.Length];
            var temp = new (int, int)[Data.Length/2];
            for (int i = 0; i < Data.Length/2; i++)
            {
                temp[i] = (Data[i*2], Data[i*2+1]);
            }
            int idx = 0;
            foreach (var tuple in temp.OrderBy(x => x.Item1).ThenBy(y => y.Item2))
            {
                SortedData[idx] = tuple.Item1;
                idx++;
                SortedData[idx] = tuple.Item2;
                idx++;
            }
        }

        private void CalculateHeuristic()
        {
            Heuristic = Data.Sum(x => (3 - x));
        }

        private void CalculateIsGoal()
        {
            IsGoal = (Heuristic == 0);
        }

        private void CalculateIsFried()
        {
            IsFried = false;

            for (int mx = 1; mx < Data.Length; mx+=2)
            {
                for (int gx = 0; gx < Data.Length; gx+=2)
                {
                    if (Data[gx] == Data[mx] &&    // Any g's at the same floor as the current m
                        Data[mx] != Data[mx-1])    // The m's g is not protecting
                    {
                        IsFried = true;
                        return;
                    }
                }
            }
        }

        public override int GetHashCode()
        {
            return (Elevator, SortedData.Sum()).GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (!(other is State state)) return false;

            if (state.Elevator != Elevator) return false;

            for (int i = 0; i < SortedData.Length; i++)
            {
                if (SortedData[i] != state.SortedData[i]) return false;
            }

            return true;
        }

        public override string ToString()
        {
            var state = "";

            state += "E:" + Elevator + " | ";
            for (int d = 0; d < Data.Length; d++)
            {
                state += $"{Data[d]} ";
                if (d%2 != 0) state += " ";
            }

            return state.Trim();
        }
    }

    public class PrioQ<T>
    {
        private readonly List<(int prio, T value)> _queue;

        public PrioQ()
        {
            _queue = new List<(int, T)>();
        }

        public void EnQ(int prio, T value)
        {
            int idx = _queue.Count - 1;

            for (; idx >= 0; idx--)
            {
                if (_queue[idx].prio <= prio)
                {
                    idx ++;
                    break;
                }
            }

            if (idx < 0) idx = 0;

            _queue.Insert(idx, (prio, value));
        }

        public T DeQ()
        {
            var retVal = _queue.First();
            _queue.RemoveAt(0);
            return retVal.value;
        }

        public bool IsEmpty()
        {
            return !_queue.Any();
        }

        public void Remove(T value)
        {
            for (int idx = 0; idx < _queue.Count; idx++)
            {
                if (_queue[idx].value.Equals(value))
                {
                    _queue.RemoveAt(idx);
                    return;
                }
            }
        }
    }

    public class StatePrioQ : PrioQ<State>
    {
        private readonly Dictionary<State, int> _enqueuedStates;

        public StatePrioQ()
        {
            _enqueuedStates = new Dictionary<State, int>();
        }

        public new void EnQ(int prio, State value)
        {
            base.EnQ(prio, value);
            _enqueuedStates.Add(value, value.TotalMovement);
        }

        public new State DeQ()
        {
            var state = base.DeQ();
            if (_enqueuedStates.ContainsKey(state))
                _enqueuedStates.Remove(state);

            return state;
        }

        public int? GetCost(State state)
        {
            if (!_enqueuedStates.TryGetValue(state, out var cost))
                return null;

            return cost;
        }

        public new void Remove(State next)
        {
            if (_enqueuedStates.ContainsKey(next))
            {
                _enqueuedStates.Remove(next);
                base.Remove(next);
            }
        }
    }
}