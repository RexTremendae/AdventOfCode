using System;
using System.Collections.Generic;
using System.Linq;

using static System.Console;

namespace Day22
{
    public class Program
    {
        Map _map;

        public static void Main(string[] args)
        {
            var target = (x: 5L, y: 746L);
            var maxSearch = (x: 1000L, y: 1000L);

            if (args.Length >= 2)
            {
                target = (int.Parse(args[0]), int.Parse(args[1]));
            }
            new Program().Run(target: target, maxSearch: maxSearch, 4002);
        }

        public void Run((long x, long y) target, (long x, long y) maxSearch, long depth)
        {
            WriteLine("Looking for the shortest path...");
            _map = new Map(target: target, maxSearch: maxSearch, depth);
            //Print(target: target, size: (16, 16));
            WriteLine(new Solver(_map).FindShortestPath(printPath: false));
        }

        public void Print((long x, long y) target, (long width, long height) size)
        {
            for (int y = 0; y < size.height; y++)
            {
                for (int x = 0; x < size.width; x++)
                {
                    var pos = (x, y);
                    if (pos == (0, 0)) { Write("X"); continue; }
                    if (pos == target) { Write("T"); continue; }

                    Write(_map.GetType((x, y)));
                }

                WriteLine();
            }
        }
    }

    public enum Tool
    {
        Neither,
        ClimbingGear,
        Torch
    }

    public class Solver
    {
        private Map _map;
        private List<QItem> _queue;
        private Dictionary<Tool, Dictionary<(long x, long y), long>> _visited;
        private long _shortestDistance;
        private long _maxX, _maxY;

        public Solver(Map map)
        {
            _visited = new Dictionary<Tool, Dictionary<(long x, long y), long>>();
            _visited.Add(Tool.Neither, new Dictionary<(long x, long y), long>());
            _visited.Add(Tool.Torch, new Dictionary<(long x, long y), long>());
            _visited.Add(Tool.ClimbingGear, new Dictionary<(long x, long y), long>());
            _map = map;
            _queue = new List<QItem>();
        }

        public long FindShortestPath(bool printPath)
        {
            var startTime = DateTime.Now;
            var lastElapsed = DateTime.Now - startTime;

            _shortestDistance = _map.Target.x + _map.Target.y;
            var q = new QItem { Tool = Tool.Torch };
            EnQNexts(q);

            Write("00:00");
            while(q.Pos != _map.Target || q.Tool != Tool.Torch)
            {
                q = DeQ();
                EnQNexts(q);

                var manhattan = (Math.Abs(_map.Target.x - q.Pos.x) + Math.Abs(_map.Target.y - q.Pos.y));
                _shortestDistance = Math.Min(_shortestDistance, manhattan);
                _maxX = Math.Max(q.Pos.x, _maxX);
                _maxY = Math.Max(q.Pos.y, _maxY);

                var elapsed = DateTime.Now - startTime;
                if ((int)elapsed.TotalSeconds != (int)lastElapsed.TotalSeconds)
                {
                    Write("\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b");
                    Write(
                        $"{elapsed.Minutes.ToString().PadLeft(2, '0')}:" +
                        $"{elapsed.Seconds.ToString().PadLeft(2, '0')}" +
                        $" {_shortestDistance.ToString().PadLeft(4, ' ')}" +
                        $" ({_maxX.ToString().PadLeft(3, ' ')}, {_maxY.ToString().PadLeft(3, ' ')})");

                    lastElapsed = elapsed;
                }
            }

            WriteLine();
            if (printPath) PrintPath(q);
            return q.Duration;
        }

        private void PrintPath(QItem q)
        {
            var moves = new List<string>();

            while (q.Prev != null)
            {
                var move = $"[{q.Duration}] ";
                if (q.Pos.x == q.Prev.Pos.x + 1) move += "Move right";
                else if (q.Pos.x == q.Prev.Pos.x - 1) move += "Move left";
                else if (q.Pos.y == q.Prev.Pos.y + 1) move += "Move down";
                else if (q.Pos.y == q.Prev.Pos.y - 1) move += "Move up";
                else move += $"Change to {q.Tool}";

                moves.Insert(0, move);
                q = q.Prev;
            }

            foreach (var m in moves) WriteLine(m);
        }

        private void EnQNexts(QItem q)
        {
            if (_visited[q.Tool].TryGetValue(q.Pos, out var duration)) return;
            _visited[q.Tool][q.Pos] = q.Duration;

            QItem nq;

            nq = q.GenerateNew(_map.Target, newX: -1);
            if (CanEnter(nq)) EnQ(nq);

            nq = q.GenerateNew(_map.Target, newY: -1);
            if (CanEnter(nq)) EnQ(nq);

            nq = q.GenerateNew(_map.Target, newX: 1);
            if (CanEnter(nq)) EnQ(nq);

            nq = q.GenerateNew(_map.Target, newY: 1);
            if (CanEnter(nq)) EnQ(nq);

            nq = q.GenerateNew(_map.Target, newTool: Tool.Neither);
            if (!q.IsToolChange && q.Tool != nq.Tool && CanChange(q, nq)) EnQ(nq);

            nq = q.GenerateNew(_map.Target, newTool: Tool.ClimbingGear);
            if (!q.IsToolChange && q.Tool != nq.Tool && CanChange(q, nq)) EnQ(nq);

            nq = q.GenerateNew(_map.Target, newTool: Tool.Torch);
            if (!q.IsToolChange && q.Tool != nq.Tool && CanChange(q, nq)) EnQ(nq);
        }

        private bool CanChange(QItem current, QItem next)
        {
            var regionType = _map.GetType(current.Pos);
            switch (next.Tool)
            {
                case Tool.Neither:
                    return regionType == Map.Wet || regionType == Map.Narrow;

                case Tool.ClimbingGear:
                    return regionType == Map.Wet || regionType == Map.Rocky;

                case Tool.Torch:
                    return regionType == Map.Narrow || regionType == Map.Rocky;
            }

            throw new InvalidOperationException("Invalid tool or region type.");
        }

        private bool CanEnter(QItem q)
        {
            if (q.Pos.x < 0 || q.Pos.y < 0) return false;
            if (q.Pos.x > _map.MaxSearch.x || q.Pos.y > _map.MaxSearch.y) return false;

            var regionType = _map.GetType(q.Pos);
            switch (q.Tool)
            {
                case Tool.Neither:
                    return regionType == Map.Wet || regionType == Map.Narrow;

                case Tool.ClimbingGear:
                    return regionType == Map.Wet || regionType == Map.Rocky;

                case Tool.Torch:
                    return regionType == Map.Narrow || regionType == Map.Rocky;
            }

            throw new InvalidOperationException("Invalid tool or region type.");
        }

        private void EnQ(QItem qItem)
        {
            if (_visited[qItem.Tool].TryGetValue(qItem.Pos, out var duration)
                && duration < qItem.Duration) return;

            int idx = 0;
            while (idx < _queue.Count)
            {
                if (_queue[idx].Prio >= qItem.Prio) break;
                idx++;
            }

            _queue.Insert(idx, qItem);
        }

        private QItem DeQ()
        {
            var q = _queue[0];
            _queue.RemoveAt(0);
            return q;
        }
    }

    public class QItem
    {
        public long Duration { get; set; }
        public Tool Tool { get; set; }
        public (long x, long y) Pos { get; set; }
        public long Prio { get; set; }
        public bool IsToolChange { get; set; }
        public QItem Prev { get; set; }

        public QItem GenerateNew((long x, long y) target, long? newX = null, long? newY = null, Tool? newTool = null)
        {
            var q = new QItem
            {
                Duration = Duration,
                Tool = Tool,
                Pos = Pos,
                Prev = this
            };

            if (newX != null)
            {
                q.Pos = (q.Pos.x + newX.Value, q.Pos.y);
                q.Duration ++;
            }
            if (newY != null)
            {
                q.Pos = (q.Pos.x, q.Pos.y + newY.Value);
                q.Duration ++;
            }
            if (newTool != null)
            {
                q.Tool = newTool.Value;
                q.IsToolChange = true;
                q.Duration += 7;
            }

            q.Prio = q.Duration + Heuristic(pos: q.Pos, target: target);

            return q;
        }

        private long Heuristic((long x, long y) pos, (long x, long y) target)
        {
            return (Math.Abs(target.x - pos.x) + Math.Abs(target.y - pos.y));
        }
    }

    public class Map
    {
        private Dictionary<(long x, long y), long> _geologicIndex;
        private Dictionary<(long x, long y), long> _erosionLevel;

        public (long x, long y) Target { get; set; }
        public (long x, long y) MaxSearch { get; set; }

        private long _depth;

        public const char Rocky = '.';
        public const char Wet = '=';
        public const char Narrow = '|';

        public Map((long x, long y) target, (long x, long y) maxSearch, long depth)
        {
            _geologicIndex = new Dictionary<(long x, long y), long>();
            _erosionLevel = new Dictionary<(long x, long y), long>();

            Target = target;
            MaxSearch = maxSearch;

            _depth = depth;
        }

        public char GetType((long x, long y) pos)
        {
            var mod = GetErosionLevel(pos) % 3;
            return mod == 0 ? Rocky :
                   mod == 1 ? Wet :
                              Narrow;
        }

        public long GetGeologicIndex((long x, long y) pos)
        {
            if (_geologicIndex.TryGetValue(pos, out var idx))
            {
                return idx;
            }

            if (pos == (0, 0)) idx = 0;
            else if (pos == Target) idx = 0;
            else if (pos.y == 0) idx = pos.x * 16807;
            else if (pos.x == 0) idx = pos.y * 48271;
            else idx = GetErosionLevel((pos.x - 1, pos.y)) * GetErosionLevel((pos.x, pos.y - 1));

            _geologicIndex.Add(pos, idx);
            return idx;
        }

        public long GetErosionLevel((long x, long y) pos)
        {
            if (_erosionLevel.TryGetValue(pos, out var lvl))
            {
                return lvl;
            }

            lvl = (GetGeologicIndex(pos) + _depth) % 20183;

            _erosionLevel.Add(pos, lvl);
            return lvl;
        }
    }
}