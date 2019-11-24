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
            Depth = 510;
            if (target == null)
                target = (x: 10, y: 10);

            // Puzzle input
            //Depth = 4002;
            //if (target == null)
            //    target = (x: 5, y: 746);

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

    public enum Movement
    {
        Up,
        Down,
        Left,
        Right
    }

    public class Solution
    {
        private List<QItem> _prioQueue = new List<QItem>();
        private Map _map;
        private (long x, long y) _target;
        private long _furthestDistance;
        private long _longestDuration;

        private Dictionary<Tools, CellType[]> _availability = new Dictionary<Tools, CellType[]>
        {
            { Tools.Neither,      new[] { CellType.Wet, CellType.Narrow } },
            { Tools.Torch,        new[] { CellType.Rocky, CellType.Narrow } },
            { Tools.ClimbingGear, new[] { CellType.Rocky, CellType.Wet } }
        };

        public long FindShortestPath((long x, long y) target)
        {
            _furthestDistance = 0;
            _longestDuration = 0;
            var path = new (long x, long y, Tools tool)[] { (0L, 0L, Tools.Torch) };
            _target = target;
            _map = new Map(target);
            EnqueuePossibleOperations(0, path);
            return FindShortestPath() - 1;
        }

        private long FindShortestPath()
        {
            var startTime = DateTime.Now;
            int secondsPassed = 0;

            while (true)
            {
                var deQed = DequeueOperation();
                var pos = deQed.Path.Last();
                if (pos.x == _target.x && pos.y == _target.y)
                {
                    return pos.tool == Tools.Torch ? deQed.Duration : deQed.Duration + 7;
                }

                var lastSecondsPassed = secondsPassed;
                secondsPassed = (int)(DateTime.Now - startTime).TotalSeconds;
                if (lastSecondsPassed != secondsPassed)
                {
                    WriteLine($"{secondsPassed}s");
                }

                var pathClone = deQed.Path.ClonePath(true);
                if (pathClone.Length > _furthestDistance || deQed.Duration > _longestDuration)
                {
                    if (pathClone.Length > _furthestDistance)
                        _furthestDistance = pathClone.Length;

                    if (deQed.Duration > _longestDuration)
                        _longestDuration = deQed.Duration;
                    var solutionTime = (DateTime.Now - startTime).TotalSeconds.ToString("0.00");
                    WriteLine($"Longest path: {_furthestDistance}, Longest duration: {_longestDuration}, Solution time: {solutionTime}s");
                }

                switch (deQed.Operation)
                {
                    case Movement move when move == Movement.Up:
                        pathClone[deQed.Path.Length] = (pos.x, pos.y-1, pos.tool);
                        break;

                    case Movement move when move == Movement.Down:
                        pathClone[deQed.Path.Length] = (pos.x, pos.y+1, pos.tool);
                        break;

                    case Movement move when move == Movement.Left:
                        pathClone[deQed.Path.Length] = (pos.x-1, pos.y, pos.tool);
                        break;

                    case Movement move when move == Movement.Right:
                        pathClone[deQed.Path.Length] = (pos.x+1, pos.y, pos.tool);
                        break;

                    case Tools tool:
                        pathClone[deQed.Path.Length] = (pos.x, pos.y, tool);
                        break;

                    default:
                        throw new InvalidOperationException("Queued data is corrupt!");
                }

                EnqueuePossibleOperations(deQed.Duration, pathClone);
            }
        }

        private QItem DequeueOperation()
        {
            var dq = _prioQueue.First();
            _prioQueue.RemoveAt(0);
            return dq;
        }

        private void EnqueuePossibleOperations(long duration, (long x, long y, Tools tool)[] path)
        {
            var pos = path.Last();

            if (pos.x > 0 && _availability[pos.tool].Contains(_map.GetErosionLevel(pos.x-1, pos.y).type) && !path.Any(p => p.x == pos.x-1 && p.y == pos.y))
                EnQ(duration+1, path, (pos.x-1, pos.y), Movement.Left);
            if (pos.y > 0 && _availability[pos.tool].Contains(_map.GetErosionLevel(pos.x, pos.y-1).type) && !path.Any(p => p.x == pos.x && p.y == pos.y-1))
                EnQ(duration+1, path, (pos.x, pos.y-1), Movement.Up);
            if (_availability[pos.tool].Contains(_map.GetErosionLevel(pos.x+1, pos.y).type) && !path.Any(p => p.x == pos.x+1 && p.y == pos.y))
                EnQ(duration+1, path, (pos.x+1, pos.y), Movement.Right);
            if (_availability[pos.tool].Contains(_map.GetErosionLevel(pos.x, pos.y+1).type) && !path.Any(p => p.x == pos.x && p.y == pos.y+1))
                EnQ(duration+1, path, (pos.x, pos.y+1), Movement.Down);

            if (pos.tool != Tools.ClimbingGear) EnQ(duration+7, path, (pos.x, pos.y), Tools.ClimbingGear);
            if (pos.tool != Tools.Neither) EnQ(duration+7, path, (pos.x, pos.y), Tools.Neither);
            if (pos.tool != Tools.Torch) EnQ(duration+7, path, (pos.x, pos.y), Tools.Torch);
        }

        private void EnQ(long duration, (long x, long y, Tools tool)[] path, (long x, long y) coord, object operation)
        {
            int index = 0;
            var newQ = new QItem { Duration = duration, Path = path.ClonePath(), Operation = operation };
            newQ.CalculatePriority(_target);
            while (index < _prioQueue.Count && _prioQueue[index].Prio < newQ.Prio)
            {
                index++;
            }

            _prioQueue.Insert(index, newQ);
        }
    }

    public struct QItem
    {
        public long Prio { get; set; }
        public long DistanceToTarget { get; set; }
        public long Duration { get; set; }
        public object Operation { get; set; }
        public (long x, long y, Tools tool)[] Path { get; set; }

        public override string ToString()
        {
            var last = Path.Last();
            return $"({last.x}, {last.y})  D: {Duration}  P: {Prio}  T: {last.tool}  OP: {Operation.ToString()}";
        }

        public void CalculatePriority((long x, long y) target)
        {
            var pos = Path.Last();
            DistanceToTarget = (long)Math.Sqrt(Pow2(pos.x - target.x) + Pow2(pos.y - target.y));
            Prio = Duration + DistanceToTarget*2;
            if (Operation is Tools) Prio += 7;
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