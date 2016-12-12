using System;
using System.Collections.Generic;
using System.Linq;

namespace Day11
{
    public enum MovementDirection { Up, Down }

    public static class ListMovementExtension
    {
        public static List<Movement> Clone(this List<Movement> input)
        {
            List<Movement> clone = new List<Movement>();
            foreach (var item in input)
                clone.Add(item.Clone());

            return clone;
        }
    }

    public class Solver
    {
        private Queue<Tuple<Movement, List<Movement>>> _movementQueue;
        private string[][] _initialState;
        public IEnumerable<Movement> Solution { get; private set; }

        public Solver(string[][] initialState)
        {
            _initialState = initialState;
            _movementQueue = new Queue<Tuple<Movement, List<Movement>>>();
        }

        public void EnqueuePossibleMovements(string[][] state, int elevatorYPos, List<Movement> visited)
        {
            int cols = state[0].Length;
            //var elevatorY = FindElevatorYPos(initialState);

            for (int x = 1; x < cols; x++)
            {
                if (state[elevatorYPos][x] == string.Empty) continue;

                EnqueueMovements(state, elevatorYPos, new[] { x }, visited);

                for (int x2 = x + 1; x2 < cols; x2++)
                {
                    if (state[elevatorYPos][x2] == string.Empty) continue;

                    EnqueueMovements(state, elevatorYPos, new[] { x, x2 }, visited);
                }
            }
        }

        public void EnqueueMovements(string[][] state, int elevatorY, int[] selectionXPositions, List<Movement> visited)
        {
            Movement movement = new Movement()
            {
                State = Extensions.Clone(state),
                ElevatorYPosBefore = elevatorY,
                SelectionXPositions = selectionXPositions,
                MovementDirection = MovementDirection.Up
            };

            if (!visited.Any(x => x.Equals(movement)) && movement.ElevatorYPosBefore > 0)
            {
                _movementQueue.Enqueue(Tuple.Create(movement, visited.Clone()));
            }

            movement = movement.Clone();
            movement.MovementDirection = MovementDirection.Down;
            if (!visited.Any(x => x.Equals(movement)) && movement.ElevatorYPosBefore < state.Length - 1)
            {
                _movementQueue.Enqueue(Tuple.Create(movement, visited.Clone()));
            }
        }

        public static bool CheckForFinishedState(string[][] state)
        {
            for (int i = 1; i < state[0].Length; i++)
            {
                if (state[0][i] == string.Empty)
                {
                    return false;
                }
            }

            return true;
        }

        public static int[] CheckForFriedMicrochips(string[][] state, int yPos)
        {
            int height = state[yPos].Length;
            List<int> friedChips = new List<int>();
            List<string> rowItems = new List<string>();
            for (int x = 1; x < height; x++)
            {
                rowItems.Add(state[yPos][x]);
            }

            for (int x = 1; x < state[yPos].Length; x++)
            {
                string item = state[yPos][x];
                if (item.Last() != "M")
                    continue;

                string id = item.AllButLast();

                bool friendlyGenerator = false;
                bool hostileGenerator = false;

                foreach (var v in rowItems)
                {
                    if (v.Last() == "G")
                    {
                        if (v.AllButLast() == id)
                            friendlyGenerator = true;
                        else
                            hostileGenerator = true;
                    }
                }

                if (hostileGenerator && !friendlyGenerator)
                {
                    friedChips.Add(x);
                }
            }

            return friedChips.ToArray();
        }

        public int FindElevatorYPos(string[][] state)
        {
            for (int y = 0; y < state.Length; y++)
                for (int x = 0; x < state[0].Length; x++)
                    if (state[y][x] == "E") return y;

            return -1;
        }

        public bool Solve()
        {
            Solution = null;
            EnqueuePossibleMovements(_initialState, _initialState.Length-1, new List<Movement>());

            while (Solution == null)
            {
                var dequeued = _movementQueue.Dequeue();
                var movement = dequeued.Item1;
                var visited = dequeued.Item2;

                var state = Extensions.Clone(movement.State);

                int dY;
                if (movement.MovementDirection == MovementDirection.Up)
                {
                    dY = -1;
                }
                else
                {
                    dY = 1;
                }

                foreach (int x in movement.SelectionXPositions)
                {
                    state[movement.ElevatorYPosBefore + dY][x] = state[movement.ElevatorYPosBefore][x];
                    state[movement.ElevatorYPosBefore][x] = string.Empty;

                    state[movement.ElevatorYPosBefore][0] = string.Empty;
                    state[movement.ElevatorYPosBefore + dY][0] = "E";
                }

                List<int> friedChips = new List<int>();
                friedChips.AddRange(CheckForFriedMicrochips(state, movement.ElevatorYPosBefore));
                friedChips.AddRange(CheckForFriedMicrochips(state, movement.ElevatorYPosBefore + dY));

                if (friedChips.Count != 0)
                {
                    continue;
                }
                else
                {
                    var visitedClone = visited.Clone();
                    visitedClone.Add(movement);
                    EnqueuePossibleMovements(state, movement.ElevatorYPosBefore + dY, visitedClone);

                    if (CheckForFinishedState(state))
                    {
                        visited.Add(movement);
                        Solution = visited;
                    }
                }
            }

            return Solution != null;
        }
    }
}
