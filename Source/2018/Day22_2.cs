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

            // Sample - solution: 45
            //Depth = 510;
            //if (target == null)
            //    target = (x: 10, y: 10);

            // Puzzle input
            Depth = 4002;
            if (target == null)
                target = (x: 5, y: 23);

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
                var pathNode = solution.FindShortestPath(target.Value);
                var duration = pathNode.Duration;
                var path = new List<PathNode>();
                var toolChanges = new List<PathNode>();

                var finish = pathNode;
                while (pathNode != null)
                {
                    if (pathNode.Operation.IsToolChange()) toolChanges.Insert(0, pathNode);
                    path.Insert(0,pathNode);
                    pathNode = pathNode.Prev;
                }

                var last = path.First();
                foreach (var node in path.Skip(1))
                {
                    if (node.Operation.IsMovement())
                    {
                        WriteLine($"{node.Operation} ({last.Pos.x}, {last.Pos.y}) -> ({node.Pos.x}, {node.Pos.y}) [{node.Duration}]");
                    }
                    else
                    {
                        WriteLine($"{node.Operation} ({last.Pos.x}, {last.Pos.y}) [{node.Duration}]");
                    }
                    last = node;
                }

                WriteLine($"Solution: {duration}");
            }
        }
    }

    public class PathNode
    {
        public PathNode Prev { get; set; }
        public long Duration { get; set; }
        public Tools Tool { get; set; }
        public Operation Operation { get; set; }
        public (long x, long y) Pos { get; set; }
    }

    public enum CellType
    {
        Rocky,  // .
        Wet,    // =
        Narrow  // |
    }

    public enum Tools
    {
        Neither = 1,
        ClimbingGear,
        Torch
    }

    public enum Operation
    {
        None = 0,

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
        private Dictionary<(long x, long y), List<PathNode>> _visited = new Dictionary<(long x, long y), List<PathNode>>();
        private List<QItem> _prioQueue = new List<QItem>();
        private Map _map;
        private (long x, long y) _target;
        private long _shortestDistanceLeft;
        private Dictionary<long, long> _durationsQueuedCount = new Dictionary<long, long>();
        private long _minimalDurationQueued = long.MaxValue;

        private Dictionary<Tools, CellType[]> _availability = new Dictionary<Tools, CellType[]>
        {
            { Tools.Neither,      new[] { CellType.Wet /* = */, CellType.Narrow  /* | */ } },
            { Tools.Torch,        new[] { CellType.Rocky /* . */, CellType.Narrow /* | */ } },
            { Tools.ClimbingGear, new[] { CellType.Rocky /* . */, CellType.Wet /* = */ } }
        };

        public PathNode FindShortestPath((long x, long y) target)
        {
            _shortestDistanceLeft = long.MaxValue;
            _target = target;
            _map = new Map(target);
            EnqueuePossibleOperations(new PathNode {
                Tool = Tools.Torch, Pos = (0L, 0L),
                Duration = 0, Operation = Operation.None, Prev = null });
            return FindShortestPath();
        }

        private PathNode FindShortestPath()
        {
            var startTime = DateTime.Now;
            int secondsPassed = 0;

            PathNode shortest = null;
            var deQed = DequeueOperation();

            while (shortest == null || shortest.Duration > _minimalDurationQueued)
            {
                var pos = deQed.PathNode.Pos;
                var tool = deQed.PathNode.Tool;

                if (pos.x == _target.x && pos.y == _target.y)
                {
                    var sln = deQed.PathNode;
                    if (tool != Tools.Torch)
                        sln = new PathNode { Prev = sln, Duration = sln.Duration + 7, Tool = Tools.Torch, Operation = Operation.ChangeToTorch, Pos = sln.Pos };

                    if (shortest == null || shortest.Duration > sln.Duration)
                        shortest = sln;
                }

                if (deQed.PathNode.Operation.IsMovement())
                {
                    if (_visited.TryGetValue((pos.x, pos.y), out var visitedData))
                    {
                        if (visitedData.Any(x => x.Duration <= deQed.PathNode.Duration && x.Tool == tool))
                            {
                                deQed = DequeueOperation();
                                continue;
                            }
                    }
                    else
                    {
                        visitedData = new List<PathNode>();
                        _visited[(pos.x, pos.y)] = visitedData;
                    }

                    var sameTool = visitedData.SingleOrDefault(x => x.Tool == tool);
                    if (sameTool != null) visitedData.Remove(sameTool);
                    visitedData.Add(deQed.PathNode);
                }

                var lastSecondsPassed = secondsPassed;
                secondsPassed = (int)(DateTime.Now - startTime).TotalSeconds;
                if (lastSecondsPassed != secondsPassed)
                {
                    WriteLine($"{secondsPassed}s");
                }

                if (deQed.DistanceToTarget < _shortestDistanceLeft)
                {
                    _shortestDistanceLeft = deQed.DistanceToTarget;

                    var solutionTime = (DateTime.Now - startTime).TotalSeconds.ToString("0.00");
                    WriteLine($"Distance left: {_shortestDistanceLeft}, Solution time: {solutionTime}s");
                }

                EnqueuePossibleOperations(deQed.PathNode);
                deQed = DequeueOperation();
            }

            return shortest;
        }

        private QItem DequeueOperation()
        {
            var dq = _prioQueue.First();
            _prioQueue.RemoveAt(0);
            _durationsQueuedCount[dq.PathNode.Duration]--;
            if (_durationsQueuedCount[dq.PathNode.Duration] == 0 && _minimalDurationQueued == dq.PathNode.Duration)
            {
                var minDuration = _minimalDurationQueued + 1;
                while(true)
                {
                    if (_durationsQueuedCount.TryGetValue(minDuration, out var value) && value > 0)
                    {
                        _minimalDurationQueued = minDuration;
                        break;
                    }

                    minDuration ++;
                }
            }
            return dq;
        }

        private void EnqueuePossibleOperations(PathNode pathNode)
        {
            var tool = pathNode.Tool;
            var pos = pathNode.Pos;
            var duration = pathNode.Duration;

            if (pos.x > 0 && _availability[tool].Contains(_map.GetErosionLevel(pos.x - 1, pos.y).type))
            {
                EnQ(new PathNode { Prev = pathNode, Duration = duration + 1,
                    Tool = tool, Operation = Operation.MoveLeft, Pos = (pos.x - 1, pos.y) });
            }
            if (pos.y > 0 && _availability[tool].Contains(_map.GetErosionLevel(pos.x, pos.y - 1).type))
            {
                EnQ(new PathNode { Prev = pathNode, Duration = duration + 1,
                    Tool = tool, Operation = Operation.MoveUp, Pos = (pos.x, pos.y - 1) });
            }
            if (_availability[tool].Contains(_map.GetErosionLevel(pos.x + 1, pos.y).type))
            {
                EnQ(new PathNode { Prev = pathNode, Duration = duration + 1,
                    Tool = tool, Operation = Operation.MoveRight, Pos = (pos.x + 1, pos.y) });
            }
            if (_availability[tool].Contains(_map.GetErosionLevel(pos.x, pos.y + 1).type))
            {
                EnQ(new PathNode { Prev = pathNode, Duration = duration + 1,
                    Tool = tool, Operation = Operation.MoveDown, Pos = (pos.x, pos.y + 1) });
            }

            if (pathNode.Prev?.Operation.IsMovement() ?? false)
            {
                if (tool != Tools.ClimbingGear)
                {
                    EnQ(new PathNode { Prev = pathNode, Duration = duration + 7, Tool = (Tools)Operation.ChangeToClimbingGear,
                        Operation = Operation.ChangeToClimbingGear, Pos = pos });
                }
                if (tool != Tools.Neither)
                {
                    EnQ(new PathNode { Prev = pathNode, Duration = duration + 7, Tool = (Tools)Operation.ChangeToNeither,
                        Operation = Operation.ChangeToNeither, Pos = pos });
                }
                if (tool != Tools.Torch)
                {
                    EnQ(new PathNode { Prev = pathNode, Duration = duration + 7, Tool = (Tools)Operation.ChangeToTorch,
                        Operation = Operation.ChangeToTorch, Pos = pos });
                }
            }
        }

        private void EnQ(PathNode pathNode)
        {
            int index = 0;
            var newQ = new QItem { PathNode = pathNode };
            newQ.CalculatePriority(_target);
            while (index < _prioQueue.Count && _prioQueue[index].Prio < newQ.Prio)
            {
                index++;
            }

            _prioQueue.Insert(index, newQ);
            if (_durationsQueuedCount.ContainsKey(newQ.PathNode.Duration))
            {
                _durationsQueuedCount[newQ.PathNode.Duration]++;
            }
            else
            {
                _durationsQueuedCount[newQ.PathNode.Duration] = 1;
            }

            if (_minimalDurationQueued > newQ.PathNode.Duration) _minimalDurationQueued = newQ.PathNode.Duration;
        }
    }

    public struct QItem
    {
        public long Prio { get; set; }
        public long DistanceToTarget { get; set; }
        public PathNode PathNode { get; set; }

        public override string ToString()
        {
            return $"({PathNode.Pos.x}, {PathNode.Pos.y})  D: {PathNode.Duration}  P: {Prio}  T: {PathNode.Tool}  OP: {PathNode.Operation.ToString()}";
        }

        public void CalculatePriority((long x, long y) target)
        {
            DistanceToTarget = Math.Abs(target.x - PathNode.Pos.x) + Math.Abs(target.y - PathNode.Pos.y);
            Prio = PathNode.Duration + DistanceToTarget;
        }
    }

    public static class Extensions
    {
        public static bool IsToolChange(this Operation operation)
        {
            return !operation.IsMovement() && operation != Operation.None;
        }

        public static bool IsMovement(this Operation operation)
        {
            return (operation == Operation.MoveUp ||
                    operation == Operation.MoveDown ||
                    operation == Operation.MoveRight ||
                    operation == Operation.MoveLeft);
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