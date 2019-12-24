using System;
using System.Collections.Generic;
using System.Linq;

using static System.Console;

namespace Day24
{
    public class Program
    {
        private string _initialState = @"
#..##
##...
.#.#.
#####
####.";
        public static void Main()
        {
            new Program().Run();
        }

        public void Run()
        {
            var state = new State(_initialState
                .Split("\n")
                .Select(x => x.Trim().Replace("\r", ""))
                .Where(x => !string.IsNullOrEmpty(x))
                .ToArray());

            var visitedStates = new HashSet<State>();
            PrintState(state);

            while (!visitedStates.Contains(state))
            {
                visitedStates.Add(state);
                state = NextState(state);
            }

            PrintState(state);
            WriteLine(Rate(state));
        }

        public long Rate(State state)
        {
            var cellValue = 1L;
            var rate = 0L;

            for (int y = 0; y < state.Lines.Length; y++)
            {
                for (int x = 0; x < state.Lines[y].Length; x++)
                {
                    if (state.Lines[y][x] == '#') rate += cellValue;
                    cellValue <<= 1;
                }
            }

            return rate;
        }

        public State NextState(State state)
        {
            var nextState = new State();
            for (int y = 0; y < state.Lines.Length; y++)
            {
                var line = "";
                for (int x = 0; x < state.Lines[y].Length; x++)
                {
                    var neighbors = GetNeighbors(state, x, y);

                    if (state.Lines[y][x] == '#')
                    {
                        line += neighbors == 1 ? '#' : '.';
                    }
                    else if (state.Lines[y][x] == '.' && neighbors == 1 || neighbors == 2) line += '#';
                    else line += state.Lines[y][x];
                }
                nextState.Lines[y] = line;
            }

            return nextState;
        }

        private int GetNeighbors(State state, int x, int y)
        {
            var neigbors = 0;
            foreach (var cell in new [] { (x: x+1, y: y), (x: x-1, y: y),
                                          (x: x, y: y+1), (x: x, y: y-1) })
            {
                if (cell.x < 0 || cell.y < 0 || cell.x >= 5 || cell.y >= 5) continue;
                if (state.Lines[cell.y][cell.x] == '#') neigbors++;
            }

            return neigbors;
        }

        public void PrintState(State state)
        {
            for (int l = 0; l < state.Lines.Length; l++)
            {
                WriteLine($"[{l}] |{state.Lines[l]}|");
            }
            WriteLine();
        }
    }

    public class State
    {
        public State()
        {
            Lines = new string[5];
        }

        public State(string[] lines)
        {
            Lines = new string[lines.Length];
            for (int l = 0; l < lines.LongLength; l++) Lines[l] = lines[l];
        }

        public string[] Lines { get; set; }

        public override bool Equals(object obj)
        {
            var other = (State)obj;

            for (int l = 0; l < Lines.Length; l++)
                if (other.Lines[l] != Lines[l]) return false;

            return true;
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            unchecked {
            foreach (var line in Lines)
            {
                hashCode += line.GetHashCode();
            }
            }
            return hashCode;
        }
    }
}