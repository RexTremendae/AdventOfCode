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

    public class Progress
    {
        public Progress(int steps = 0, long queueSize = 0)
        {
            Steps = steps;
            QueueSize = queueSize;
        }

        public int Steps { get; }
        public long QueueSize { get; }
    }

    public class Solver
    {
        private Queue<Tuple<Movement, List<Movement>>> _movementQueue;
        private State _initialState;
        private ICombinator _combinator;
        public IEnumerable<Movement> Solution { get; private set; }
        private int _cols;
        private int _rows;
                
        public event EventHandler<Progress> ReportProgress;
        Progress _currentProgress;

        public Solver(string[][] initialState, ICombinator combinator)
        {
            _combinator = combinator;
            _initialState = new State(initialState);
            _movementQueue = new Queue<Tuple<Movement, List<Movement>>>();
            _currentProgress = new Progress();
            _cols = initialState[0].Length;
            _rows = initialState.Length;
        }

        public void EnqueuePossibleMovements(State state, int elevatorYPos, List<Movement> visited)
        {
            foreach (var combination in _combinator.GetCombinations(state, elevatorYPos))
            {
                EnqueueMovements(state, elevatorYPos, combination, visited);
            }
        }

        public void EnqueueMovements(State state, int elevatorY, int[] selectionXPositions, List<Movement> visited)
        {
            var lastMove = visited.LastOrDefault();

            Movement movement = new Movement()
            {
                State = state.Clone(),
                ElevatorYPosBefore = elevatorY,
                SelectionXPositions = selectionXPositions,
                MovementDirection = MovementDirection.Up
            };

            if (!visited.Any(x => x.Equals(movement)) && movement.ElevatorYPosBefore > 0 && !movement.IsReverseOf(lastMove))
            {
                _movementQueue.Enqueue(Tuple.Create(movement, visited.Clone()));
            }

            movement = movement.Clone();
            movement.MovementDirection = MovementDirection.Down;
            if (!visited.Any(x => x.Equals(movement)) && movement.ElevatorYPosBefore < _rows && !movement.IsReverseOf(lastMove))
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
            EnqueuePossibleMovements(_initialState, _rows-1, new List<Movement>());

            while (Solution == null)
            {
                var dequeued = _movementQueue.Dequeue();
                var movement = dequeued.Item1;
                var visited = dequeued.Item2;

                Progress newProgress = new Progress(visited.Count, _movementQueue.Count);
                int queueSizeGranularity = 10000;
                if (newProgress.Steps > _currentProgress.Steps || newProgress.QueueSize > _currentProgress.QueueSize + queueSizeGranularity)
                {
                    _currentProgress = new Progress(newProgress.Steps, newProgress.QueueSize - newProgress.QueueSize % queueSizeGranularity);
                    ReportProgress?.Invoke(this, newProgress);
                }

                var state = movement.State.Clone();

                int dY;
                if (movement.MovementDirection == MovementDirection.Up)
                {
                    dY = -1;
                }
                else
                {
                    dY = 1;
                }

                var newYPos = movement.ElevatorYPosBefore + dY;
                if (newYPos < 0 || newYPos >= _rows)
                    continue;

                foreach (int x in movement.SelectionXPositions)
                {
                    string tmp = state.GetData(x, movement.ElevatorYPosBefore);
                    state.MoveOrAdd(x, newYPos, tmp);
                    state.MoveOrAdd(0, newYPos, "E");
                }

                var rawState = state.Expand();
                List<int> friedChips = new List<int>();
                friedChips.AddRange(CheckForFriedMicrochips(rawState, movement.ElevatorYPosBefore));
                friedChips.AddRange(CheckForFriedMicrochips(rawState, newYPos));

                if (friedChips.Count != 0)
                {
                    continue;
                }
                else
                {
                    var visitedClone = visited.Clone();
                    visitedClone.Add(movement);
                    EnqueuePossibleMovements(state, newYPos, visitedClone);

                    if (CheckForFinishedState(rawState))
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
