using System;
using System.Collections.Generic;
using System.Linq;

using static System.Console;

namespace Day24
{
    public class Program
    {
        // Puzzle input
        private string _initialState =
            """
            #..##
            ##...
            .#.#.
            #####
            ####.
            """;
/*
        // Sample input
        private string _initialState =
            """
            ....#
            #..#.
            #..##
            ..#..
            #....
            """;
*/

        public static void Main()
        {
            var start = DateTime.Now;
            new Program().Run();
            var duration = DateTime.Now-start;
            Console.WriteLine($"{duration.TotalSeconds:0.00}s");
        }

        private static State CreateEmptyState(State? inner = null, State? outer = null)
        {
            var state = new State(new[] { ".....", ".....", ".....", ".....", "....." });
            state.InnerState = inner;
            state.OuterState = outer;
            return state;
        }

        public void Run()
        {
            var state = new State(_initialState
                .Split("\n")
                .Select(x => x.Trim().Replace("\r", ""))
                .Where(x => !string.IsNullOrEmpty(x))
                .ToArray());

            //PrintState(state, 0);

            for (int i = 0; i < 200; i ++)
            {
                state = NextState(state);
                //PrintState(state, i+1);
            }

            Console.WriteLine(state.CountTotalBugs());
        }

        public State NextState(State state)
        {
            var nextState0 = CreateNextState(state);
            var nextState = nextState0;

            // OuterState -, InnerState +
            var walk = state;
            while (walk.OuterState != null)
            {
                walk = walk.OuterState;
                nextState.OuterState = CreateNextState(walk, inner: nextState);
                nextState = nextState.OuterState;
            }

            if (!nextState.IsEmpty())
            {
                nextState.OuterState = CreateNextState(CreateEmptyState(inner: walk), inner: nextState);
            }

            walk = state;
            nextState = nextState0;
            while (walk.InnerState != null)
            {
                walk = walk.InnerState;
                nextState.InnerState = CreateNextState(walk, outer: nextState);
                nextState = nextState.InnerState;
            }

            if (!nextState.IsEmpty())
            {
                nextState.InnerState = CreateNextState(CreateEmptyState(outer: walk), outer: nextState);
            }

            return nextState0;
        }

        private State CreateNextState(State state, State? inner = null, State? outer = null)
        {
            var nextState = CreateEmptyState();

            for (int y = 0; y < state.Lines.Length; y++)
            {
                var line = "";
                for (int x = 0; x < state.Lines[y].Length; x++)
                {
                    if (x == 2 && y == 2)
                    {
                        line += '.';
                        continue;
                    }

                    var neighbors = GetNeighbors(state, x, y);

                    if (state.Lines[y][x] == '#')
                        line += neighbors == 1 ? '#' : '.';
                    else
                        line += (neighbors == 1 || neighbors == 2) ? '#' : '.';
                }
                nextState.Lines[y] = line;
            }

            nextState.InnerState = inner;
            nextState.OuterState = outer;

            return nextState;
        }

        private int GetNeighbors(State state, int x, int y)
        {
            // OuterState -, InnerState +

            var neigbors = 0;
            foreach (var cell in new [] { (x: x+1, y: y), (x: x-1, y: y),
                                          (x: x, y: y+1), (x: x, y: y-1) })
            {
                if (cell.x == 2 && cell.y == 2)
                {
                    if (x == 1)
                    {
                        for (int yy = 0; yy < 5; yy++)
                        {
                            if ((state.InnerState?.Lines[yy][0] ?? '.') == '#')
                                neigbors++;
                        }
                    }
                    else if (x == 3)
                    {
                        for (int yy = 0; yy < 5; yy++)
                        {
                            if ((state.InnerState?.Lines[yy][4] ?? '.') == '#')
                                neigbors++;
                        }
                    }
                    else if (y == 1)
                    {
                        for (int xx = 0; xx < 5; xx++)
                        {
                            if ((state.InnerState?.Lines[0][xx] ?? '.') == '#')
                                neigbors++;
                        }
                    }
                    else if (y == 3)
                    {
                        for (int xx = 0; xx < 5; xx++)
                        {
                            if ((state.InnerState?.Lines[4][xx] ?? '.') == '#')
                                neigbors++;
                        }
                    }
                }
                else if (cell.x < 0)
                {
                    if ((state.OuterState?.Lines[2][1] ?? '.') == '#')
                        neigbors++;
                }
                else if (cell.y < 0)
                {
                    if ((state.OuterState?.Lines[1][2] ?? '.') == '#')
                        neigbors++;
                }
                else if (cell.x >= 5)
                {
                    if ((state.OuterState?.Lines[2][3] ?? '.') == '#')
                        neigbors++;
                }
                else if (cell.y >= 5)
                {
                    if ((state.OuterState?.Lines[3][2] ?? '.') == '#')
                        neigbors++;
                }
                else if (state.Lines[cell.y][cell.x] == '#')
                {
                    neigbors++;
                }
            }

            return neigbors;
        }

        public void PrintState(State state, int minutes)
        {
            WriteLine($"After {minutes} minutes:");
            WriteLine();

            // OuterState -, InnerState +
            var walk = state;
            var depth = 0;
            while (walk.OuterState != null)
            {
                depth--;
                walk = walk.OuterState;
            }

            while (walk != null)
            {
                WriteLine($"Depth {depth++}");
                for (int l = 0; l < walk.Lines.Length; l++)
                {
                    WriteLine($"[{l}] |{walk.Lines[l]}|");
                }
                WriteLine();

                walk = walk.InnerState;
            }
        }
    }

    public class State
    {
        public State? InnerState { get; set; }
        public State? OuterState { get; set; }

        public State()
        {
            Lines = new string[5];
        }

        public State(string[] lines)
        {
            Lines = new string[lines.Length];
            for (int l = 0; l < lines.LongLength; l++) Lines[l] = lines[l];
        }

        public int CountTotalBugs()
        {
            var state = this;
            while (state.InnerState != null) state = state.InnerState;

            var sum = 0;
            while (state.OuterState != null)
            {
                sum += string.Join("", state.Lines).Count(_ => _ == '#');
                state = state.OuterState;
            }

            return sum;
        }

        public bool IsEmpty()
        {
            foreach (var line in Lines)
            {
                if (line.Contains('#')) return false;
            }

            return true;
        }

        public string[] Lines { get; set; }
    }
}
