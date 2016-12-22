using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using static System.Console;

namespace Day11
{
    public class State
    {
        public byte[] Generators;
        public byte[] Microchips;
        public byte ElevatorPosition;

        public override bool Equals(object obj)
        {
            var other = obj as State;

            if (ElevatorPosition != other.ElevatorPosition) return false;

            for (int i = 0; i < Generators.Length; i++)
            {
                if (Generators[i] != other.Generators[i]) return false;
                if (Microchips[i] != other.Microchips[i]) return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return Tuple.Create(Generators, Microchips, ElevatorPosition).GetHashCode();
        }

        public State Clone()
        {
            var newState = new State();

            newState.Generators = new byte[Generators.Length];
            for (int i = 0; i < Generators.Length; i++)
                newState.Generators[i] = Generators[i];

            newState.Microchips = new byte[Microchips.Length];
            for (int i = 0; i < Microchips.Length; i++)
                newState.Microchips[i] = Microchips[i];

            newState.ElevatorPosition = ElevatorPosition;

            return newState;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"E: {ElevatorPosition}, G: [");
            foreach (var g in Generators)
            {
                sb.Append($"{g}, ");
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append("], M: [");
            foreach (var m in Microchips)
            {
                sb.Append($"{m}, ");
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append("] ");

            return sb.ToString();
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            new Program().Run(args);
        }

        public void Run(string[] args)
        {
            IReader reader;
            if (args.Length > 0)
            {
                reader = new FileReader(args[0]);
            }
            else
            {
                reader = new ConsoleReader();
            }


            // ===  Small example  ===
            var _smallExample = new State();

            _smallExample.Generators = new byte[2];
            _smallExample.Microchips = new byte[2];

            _smallExample.Generators[0] = 1;
            _smallExample.Generators[1] = 2;
            // =======================


            // ===  Bigger Example  ===
            var _biggerExample = new State();

            _biggerExample.Generators = new byte[3];
            _biggerExample.Microchips = new byte[3];

            _biggerExample.Microchips[0] = 1;
            _biggerExample.Microchips[1] = 1;
            // =======================


            // ===  Problem input  ===
            var _problemInput = new State();

            _problemInput.Generators = new byte[5];
            _problemInput.Microchips = new byte[5];

            _problemInput.Microchips[0] = 1;
            _problemInput.Microchips[1] = 1;
            // =======================


            var solver = new Solver();
            solver.Solve(_biggerExample);

            WriteLine(solver.Solution.Count);
            WriteLine();
            WriteSolution(solver.Solution);
        }

        public void WriteSolution(List<State> solution)
        {
            int stateCounter = 0;

            foreach (var state in solution)
            {
                WriteLine($"[{stateCounter}]");
                for (int i = 0; i < Solver.Floors; i++)
                {
                    string row = string.Empty;
                    int currentIndex = Solver.Floors - i - 1;

                    row += "F";
                    row += (currentIndex + 1);
                    row += " ";

                    if (state.ElevatorPosition == currentIndex)
                        row += "E";
                    else
                        row += " ";

                    row += " ";

                    for (int j = 0; j < state.Generators.Length; j++)
                    {
                        if (state.Generators[j] == currentIndex)
                            row += $"G{j}";
                        else
                            row += ". ";
                        
                        row += " ";

                        if (state.Microchips[j] == currentIndex)
                            row += $"M{j}";
                        else
                            row += ". ";

                        row += " ";
                    }
                    WriteLine(row);
                }
                WriteLine();
                stateCounter++;
            }
        }
    }

    public class Solver
    {
        public const byte Floors = 4;
        public List<State> Solution;

        private enum StateOfState
        {
            Neutral,
            Finished,
            Burned,
            Visited
        }

        Queue<Tuple<State, List<State>>> _moves;

        public void Solve(State state)
        {
            _moves = new Queue<Tuple<State, List<State>>>();
            EnqueuePossibleMoves(state, new List<State>());

            int lastLength = 0;

            while (Solution == null)
            {
                var dequeued = _moves.Dequeue();
                if (dequeued.Item2.Count != lastLength)
                {
                    string lastLengthAsString = lastLength.ToString();
                    if (lastLengthAsString.Length == 1)
                        lastLengthAsString = " " + lastLengthAsString;
                    WriteLine($"Dequeueing ({lastLengthAsString} :: {_moves.Count})...");
                    lastLength = dequeued.Item2.Count;
                }
                EnqueuePossibleMoves(dequeued.Item1, dequeued.Item2);
            }
        }

        private void EnqueuePossibleMoves(State currentState, List<State> visited)
        {
            for (byte x1 = 0; x1 < currentState.Generators.Length * 2; x1++)
            {
                EnqueueElevatorMoves(currentState, visited, new[] { x1 });
                if (Solution != null) return;

                for (byte x2 = (byte)(x1 + 1); x2 < currentState.Generators.Length * 2; x2++)
                {
                    EnqueueElevatorMoves(currentState, visited, new[] { x1, x2 });
                }
            }
        }

        private void EnqueueElevatorMoves(State currentState, List<State> visited, byte[] selection)
        {
            var newStates = CreateStates(currentState, selection);
            foreach (var state in newStates)
            {
                if (state == null) continue;

                var stateOfState = CheckState(state, visited);
                if (stateOfState == StateOfState.Finished)
                {
                    Solution = visited.Clone();
                    Solution.Add(currentState);
                    Solution.Add(state);
                }
                else if (stateOfState == StateOfState.Neutral)
                {
                    var visitedClone = visited.Clone();
                    visitedClone.Add(currentState);
                    _moves.Enqueue(Tuple.Create(state, visitedClone));
                }
            }
        }

        private State CreateState(State state, byte[] selection, int elevatorDelta)
        {
            if (state.ElevatorPosition + elevatorDelta < 0) return null;
            if (state.ElevatorPosition + elevatorDelta >= Floors) return null;

            var newState = state.Clone();

            foreach (var b in selection)
            {
                int chipIndex = b - newState.Generators.Length;
                if (chipIndex >= 0)
                {
                    if (newState.Microchips[chipIndex] != newState.ElevatorPosition)
                    {
                        return null;
                    }

                    newState.Microchips[chipIndex] = (byte)(newState.Microchips[chipIndex] + elevatorDelta);
                }
                else
                {
                    if (newState.Generators[b] != newState.ElevatorPosition)
                    {
                        return null;
                    }

                    newState.Generators[b] = (byte)(newState.Generators[b] + elevatorDelta);
                }
            }

            newState.ElevatorPosition = (byte)(newState.ElevatorPosition + elevatorDelta);

            return newState;
        }

        private State[] CreateStates(State state, byte[] selection)
        {
            State s1 = CreateState(state, selection, 1);
            State s2 = CreateState(state, selection, -1);

            return new State[] { s1, s2 };
        }

        private StateOfState CheckState(State newState, List<State> visited)
        {
            if (visited.Contains(newState)) return StateOfState.Visited;

            bool isFinished = true;
            foreach (var g in newState.Generators)
                if (g != Floors-1) isFinished = false;
            foreach (var m in newState.Microchips)
                if (m != Floors-1) isFinished = false;

            if (isFinished) return StateOfState.Finished;

            for (int i = 0; i < newState.Microchips.Length; i++)
            {
                if (newState.Microchips[i] == newState.Generators[i]) continue;

                for (int j = 0; j < newState.Generators.Length; j++)
                {
                    if (newState.Generators[j] == newState.Microchips[i]) return StateOfState.Burned;
                }
            }

            return StateOfState.Neutral;
        }
    }

    public static class ListExtension
    {
        public static List<T> Clone<T>(this List<T> original)
        {
            List<T> clone = new List<T>();
            clone.AddRange(original);

            return clone;
        }
    }

    interface IReader
    {
        string ReadLine();
        bool EndOfStream { get; }
    }

    public class ConsoleReader : IReader
    {
        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public bool EndOfStream => false;
    }

    public class FileReader : IReader
    {
        StreamReader _reader;

        public FileReader(string filename)
        {
            _reader = new StreamReader(filename);
        }

        public string ReadLine()
        {
            return _reader.ReadLine();
        }

        public bool EndOfStream
        {
            get
            {
                return _reader.EndOfStream;
            }
        }
    }
}