using System;
using System.Collections.Generic;
using System.Linq;

using static System.Console;

namespace Day22
{
    public class Program
    {
        public static long Depth;

        public static void Main(string[] args)
        {
            (long x, long y)? target = null;
            if (args.Length >= 2)
            {
                target = (int.Parse(args[0]), int.Parse(args[1]));
            }

            // Sample
            //Depth = 510;
            //if (target == null)
            //    target = (x: 10, y: 10);

            // Puzzle input
            Depth = 4002;
            if (target == null)
                target = (x: 5, y: 746);

            if (args.Length >= 3 && args[2].Equals("print", StringComparison.OrdinalIgnoreCase))
            {
                var boundaries = (target.Value.x+1, target.Value.y+1);
                if (args.Length >= 5)
                {
                    boundaries = (long.Parse(args[3]), long.Parse(args[4]));
                }

                Print(target.Value, boundaries);
                return;
            }

            new Program().Run(target);
        }

        public static void Print((long x, long y) target, (long x, long y) boundaries)
        {
            var map = new Map(target);

            for (long y = 0; y < boundaries.y; y++)
            {
                for (long x = 0; x < boundaries.x; x++)
                {
                    if (x == target.x && y == target.y)
                    {
                        Write('T');
                        continue;
                    }

                    var erosion = map.GetErosionLevel(x, y);
                    switch(erosion.type)
                    {
                        case CellType.Narrow:
                            Write('|');
                            break;

                        case CellType.Rocky:
                            Write('.');
                            break;

                        case CellType.Wet:
                            Write('=');
                            break;
                    }
                }
                WriteLine();
            }
        }

        public void Run((long x, long y)? target = null)
        {
            var solution = new Solution();

            WriteLine($"Target: ({target.Value.x}, {target.Value.y})");

            checked
            {
                WriteLine($"Solution: {solution.FindShortestPath(target.Value)}");
            }
        }
    }

    public enum CellType
    {
        Rocky,
        Wet,
        Narrow
    }

    public enum Tools
    {
        Neither,
        ClimbingGear,
        Torch
    }

    public enum Operation
    {
        ChangeToNeither,
        ChangeToClimbingGear,
        ChangeToTorch,

        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight
    }

    public class Solution
    {
        private Dictionary<(long x, long y), long> _visited = new Dictionary<(long x, long y), long>();
        private List<QItem> _prioQueue = new List<QItem>();
        private Map _map;
        private (long x, long y) _target;
        private long _longestDuration;

        private Dictionary<Tools, CellType[]> _availability = new Dictionary<Tools, CellType[]>
        {
            { Tools.Neither,      new[] { CellType.Wet, CellType.Narrow } },
            { Tools.Torch,        new[] { CellType.Rocky, CellType.Narrow } },
            { Tools.ClimbingGear, new[] { CellType.Rocky, CellType.Wet } }
        };

        public long FindShortestPath((long x, long y) target)
        {
            _longestDuration = 0;
            _target = target;
            _map = new Map(target);
            EnqueuePossibleOperations(0, (0L, 0L), Tools.Torch);
            return FindShortestPath();
        }

        private long FindShortestPath()
        {
            var startTime = DateTime.Now;
            int secondsPassed = 0;

            while (true)
            {
                var deQed = DequeueOperation();
                var pos = deQed.Pos;

                if (pos.x == _target.x && pos.y == _target.y)
                {
                    return deQed.Tool == Tools.Torch ? deQed.Duration : deQed.Duration + 7;
                }

                if (IsMovement(deQed.Operation))
                {
                    if (_visited.TryGetValue((pos.x, pos.y), out var visitedDuration) &&
                        visitedDuration < deQed.Duration)
                    { continue; }

                    _visited[(pos.x, pos.y)] = deQed.Duration;
                }

                var lastSecondsPassed = secondsPassed;
                secondsPassed = (int)(DateTime.Now - startTime).TotalSeconds;
                if (lastSecondsPassed != secondsPassed)
                {
                    WriteLine($"{secondsPassed}s");
                }

                if (deQed.Duration > _longestDuration)
                {
                    if (deQed.Duration > _longestDuration)
                        _longestDuration = deQed.Duration;

                    var solutionTime = (DateTime.Now - startTime).TotalSeconds.ToString("0.00");
                    WriteLine($"Longest duration: {_longestDuration}, Solution time: {solutionTime}s");
                }

                var tool = deQed.Tool;

                switch (deQed.Operation)
                {
                    case Operation op when IsMovement(op):
                        break;

                    case Operation op when !IsMovement(op):
                        tool = (Tools)deQed.Operation;
                        break;

                    default:
                        throw new InvalidOperationException("Queued data is corrupt!");
                }

                EnqueuePossibleOperations(deQed.Duration, pos, tool);
            }
        }

        private QItem DequeueOperation()
        {
            var dq = _prioQueue.First();
            _prioQueue.RemoveAt(0);
            return dq;
        }

        private void EnqueuePossibleOperations(long duration, (long x, long y) pos, Tools tool)
        {
            if (pos.x > 0 && _availability[tool].Contains(_map.GetErosionLevel(pos.x-1, pos.y).type))
                EnQ(duration+1, (pos.x-1, pos.y), tool, Operation.MoveLeft);
            if (pos.y > 0 && _availability[tool].Contains(_map.GetErosionLevel(pos.x, pos.y-1).type))
                EnQ(duration+1, (pos.x, pos.y-1), tool, Operation.MoveUp);
            if (_availability[tool].Contains(_map.GetErosionLevel(pos.x+1, pos.y).type))
                EnQ(duration+1, (pos.x+1, pos.y), tool, Operation.MoveRight);
            if (_availability[tool].Contains(_map.GetErosionLevel(pos.x, pos.y+1).type))
                EnQ(duration+1, (pos.x, pos.y+1), tool, Operation.MoveDown);

            if (tool != Tools.ClimbingGear) EnQ(duration+7, (pos.x, pos.y), tool, Operation.ChangeToClimbingGear);
            if (tool != Tools.Neither) EnQ(duration+7, (pos.x, pos.y), tool, Operation.ChangeToNeither);
            if (tool != Tools.Torch) EnQ(duration+7, (pos.x, pos.y), tool, Operation.ChangeToTorch);
        }

        private void EnQ(long duration, (long x, long y) pos, Tools tool, Operation operation)
        {
            int index = 0;
            var newQ = new QItem { Duration = duration, Operation = operation, Pos = pos, Tool = tool };
            newQ.CalculatePriority(_target);
            while (index < _prioQueue.Count && _prioQueue[index].Prio < newQ.Prio)
            {
                index++;
            }

            _prioQueue.Insert(index, newQ);
        }

        private bool IsMovement(Operation operation)
        {
            return (operation == Operation.MoveUp ||
                    operation == Operation.MoveDown ||
                    operation == Operation.MoveRight ||
                    operation == Operation.MoveLeft);
        }
    }

    public struct QItem
    {
        public long Prio { get; set; }
        public long DistanceToTarget { get; set; }
        public long Duration { get; set; }
        public Operation Operation { get; set; }
        public (long x, long y) Pos { get; set; }
        public Tools Tool { get; set; }

        public override string ToString()
        {
            return $"({Pos.x}, {Pos.y})  D: {Duration}  P: {Prio}  T: {Tool}  OP: {Operation.ToString()}";
        }

        public void CalculatePriority((long x, long y) target)
        {
            DistanceToTarget = (long)Math.Sqrt(Pow2(Pos.x - target.x) + Pow2(Pos.y - target.y));
            Prio = Duration + DistanceToTarget*2;
            if (Operation is Tools) Prio += 4;
        }

        private long Pow2(long input)
        {
            return input * input;
        }
    }

    public static class PathExtensions
    {
        public static (long x, long y, Tools tool)[] ClonePath(this (long x, long y, Tools)[] path, bool increaseCapacity = false)
        {
            var cloneLength = path.Length;
            if (increaseCapacity) cloneLength++;
            var clone = new (long x, long y, Tools)[cloneLength];
            for (int i = 0; i < path.Length; i++)
                clone[i] = path[i];

            return clone;
        }
    }

    public class Map
    {
        private readonly (long x, long y) _target;
        private Dictionary<(long x, long y), (long, CellType)> _map = new Dictionary<(long x, long y), (long, CellType)>();

        public Map((long x, long y) target)
        {
            _target = target;
        }

        public (long erosionLevel, CellType type) GetErosionLevel(long x, long y)
        {
            var coord = (x, y);
            if (_map.TryGetValue(coord, out var data))
            {
                return data;
            }

            var geoIdx = GetGeologicIndex(x, y);
            long erosionLevel = (geoIdx + Program.Depth) % 20183;
            var mod = erosionLevel % 3;
            var type = mod == 0 ? CellType.Rocky :
                       mod == 1 ? CellType.Wet :
                                  CellType.Narrow;

            data = (erosionLevel, type);
            _map[(x, y)] = data;
            return data;
        }

        private long GetGeologicIndex(long x, long y)
        {
            if (x == 0 && y == 0) return 0;
            if (x == _target.x && y == _target.y) return 0;
            else if (y == 0) return x * 16807;
            else if (x == 0) return y * 48271;

            return GetErosionLevel(x-1, y).erosionLevel * GetErosionLevel(x, y-1).erosionLevel;
        }
    }
}