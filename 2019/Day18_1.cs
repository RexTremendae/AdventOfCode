using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Console;

namespace Day18
{
    public class Program
    {
        public static void Main()
        {
            new Program().Run();
        }

        public void Run()
        {
            System.Console.OutputEncoding = System.Text.Encoding.UTF8;

            var (map, start) = Map.ReadData("Day18.txt");
            map.Print(start);

            var (steps, path) = FindShortestPath(start, map, false, false);
            WriteLine(path);
            WriteLine(steps);
        }

        private void CheckMapTypeHashIntegrity(Map map, Point start)
        {
            var map2 = map.Clone();
            var map3 = map.Clone();
            map2.OpenDoor((3, 1));

            map2.Print(start);

            WriteLine("1 - 2: " + map .Equals(map2));
            WriteLine("1 - 3: " + map .Equals(map3));
            WriteLine("2 - 2: " + map2.Equals(map2));
            WriteLine("2 - 3: " + map2.Equals(map3));

            WriteLine("1: " + map .GetHashCode());
            WriteLine("2: " + map2.GetHashCode());
            WriteLine("3: " + map3.GetHashCode());
        }

        private (int, string) FindShortestPath(Point start, Map map, bool debug = false, bool mapDebug = false)
        {
            var collectedKeys = new HashSet<char>();
            var q = new List<(Map map, Point pos, int steps, HashSet<char> collectedKeys, string path)>();
            q.Add((map: map.Clone(), pos: start, steps: 0, collectedKeys, path: "@"));

            HashSet<(Map, Point)> visitedState = new();

            var startTime = DateTime.Now;
            var lastTimeCheck = startTime;
            var interval = TimeSpan.FromSeconds(1);

            while (q.Any())
            {
                if (DateTime.Now - lastTimeCheck > interval)
                {
                    lastTimeCheck = DateTime.Now;
                    var elapsed = DateTime.Now - startTime;
                    WriteLine($"Elapsed time: {elapsed.TotalSeconds:0} s");
                }
                var dq = q[0];
                q.RemoveAt(0);

                if (debug)
                {
                    WriteLine($"Dequeuing {dq.path} ({dq.steps})");
                }

                if (!dq.map.Keys.Any())
                {
                    return (dq.steps, $"{dq.path} - @");
                }

                visitedState.Add((dq.map.Clone(), dq.pos));
                var (keySteps, doorSteps) = FindShortestParts(dq.pos, dq.map, mapDebug, false);

                foreach (var key in keySteps)
                {
                    var mapClone = dq.map.Clone();

                    mapClone.PickupKey(key.coord);
                    collectedKeys = Clone(dq.collectedKeys);
                    collectedKeys.Add(key.label);
                    var currSteps = dq.steps + key.steps;

                    TryEnqueue(q, visitedState, mapClone, currSteps, dq.path, key, "K", collectedKeys, debug);
                }

                foreach (var door in doorSteps)
                {
                    if (!dq.collectedKeys.Contains(door.label))
                    {
                        continue;
                    }

                    var mapClone = dq.map.Clone();
                    mapClone.OpenDoor(door.coord);
                    var currSteps = dq.steps + door.steps;

                    TryEnqueue(q, visitedState, mapClone, currSteps, dq.path, door, "D", Clone(dq.collectedKeys), debug);
                }
            }

            return (-1, "-");
        }

        private void TryEnqueue(List<(Map map, Point pos, int steps, HashSet<char> collectedKeys, string path)> q, HashSet<(Map, Point)> visited, Map map, int currSteps, string path, (char label, Point coord, int steps) entity, string entityTypeLabel, HashSet<char> collectedKeys, bool debug = false)
        {
            if (visited.Contains((map, entity.coord)))
            {
                return;
            }

            var idx = 0;
            while (idx < q.Count && q[idx].steps < currSteps)
            {
                idx++;
            }
            path = $"{path} - [{entityTypeLabel}] {entity.label}";

            if (debug)
            {
                WriteLine($"Enqueuing path: {path} ({currSteps})");
            }

            q.Insert(idx, (map, pos: entity.coord, steps: currSteps, Clone(collectedKeys), path));
        }

        private ((char label, Point coord, int steps)[] keys, (char label, Point coord, int steps)[] doors) FindShortestParts(Point start, Map map, bool alwaysContinue, bool mapDebug = false)
        {
            map = map.CloneMap();
            if (mapDebug)
            {
                WriteLine($"Start at ({start})");
            }

            var points = new[] { start };
            var steps = 1;

            var keySteps = new List<(char, Point, int)>();
            var doorSteps = new List<(char, Point, int)>();

            while (points.Any())
            {
                var candidates = new HashSet<Point>();
                foreach (var p in points)
                {
                    if (p != start)
                    {
                        map.Visit(p);
                    }

                    foreach (var n in GetNeighbors(p))
                    {
                        candidates.Add(n);
                    }
                }

                var nextPoints = new HashSet<Point>();
                foreach (var n in candidates)
                {
                    switch (map[n])
                    {
                        case TileType.Walkable:
                            nextPoints.Add(new Point(n.X, n.Y));
                            break;

                        case TileType.Key:
                            if (alwaysContinue)
                            {
                                nextPoints.Add(new Point(n.X, n.Y));
                            }

                            keySteps.Add((map.Keys[n], new Point(n.X, n.Y), steps));
                            if (mapDebug)
                            {
                                WriteLine($"Key [{map.Keys[n]}] {steps} steps from starting point.");
                            }
                            break;

                        case TileType.Door:
                            if (alwaysContinue)
                            {
                                nextPoints.Add(new Point(n.X, n.Y));
                            }

                            doorSteps.Add((map.Doors[n], new Point(n.X, n.Y), steps));
                            if (mapDebug)
                            {
                                WriteLine($"Door [{map.Doors[n]}] {steps} steps from starting point.");
                            }
                            break;
                    }
                }

                points = nextPoints.ToArray();
                if (mapDebug)
                {
                    map.Print(start);
                }
                steps++;
            }

            return (keySteps.ToArray(), doorSteps.ToArray());
        }

        private Point[] GetNeighbors(Point point)
        {
            return new[]
            {
                new Point(point.X + 1, point.Y),
                new Point(point.X - 1, point.Y),
                new Point(point.X, point.Y + 1),
                new Point(point.X, point.Y - 1)
            };
        }

        private IEnumerable<Point> GetNeighbors(Point[] points)
        {
            foreach (var p in points)
            {
                yield return new Point(p.X + 1, p.Y);
                yield return new Point(p.X - 1, p.Y);
                yield return new Point(p.X, p.Y + 1);
                yield return new Point(p.X, p.Y - 1);
            };
        }

        private HashSet<char> Clone(HashSet<char> set)
        {
            return set.ToHashSet();
        }
    }

    public class Map
    {
        private static readonly List<List<TileType>> _map = new();
        private List<List<TileType>>? _visitableMap;

        private Dictionary<Point, char> _keys = new();
        private Dictionary<Point, char> _doors = new();

        public IReadOnlyDictionary<Point, char> Keys => _keys;
        public IReadOnlyDictionary<Point, char> Doors => _doors;

        public static (Map map, Point start) ReadData(string filePath)
        {
            _map.Clear();

            var map = new Map();
            var start = new Point(-1, -1);

            int y = 0;
            foreach (var line in File.ReadAllLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    break;
                }

                var mapRow = new List<TileType>();
                for (int x = 0; x < line.Length; x++)
                {
                    TileType tile;

                    switch (line[x])
                    {
                        case '@':
                            start = new Point(x, y);
                            tile = TileType.Walkable;
                            break;

                        case '.':
                            tile = TileType.Walkable;
                            break;

                        case '#':
                            tile = TileType.Wall;
                            break;

                        case char key when key >= 'a' && key <= 'z':
                            map._keys.Add(new Point(x, y), key);
                            tile = TileType.Walkable;
                            break;

                        case char door when door >= 'A' && door <= 'Z':
                            map._doors.Add(new Point(x, y), char.ToLower(door));
                            tile = TileType.Walkable;
                            break;

                        default:
                            throw new InvalidOperationException($"Invalid character: {line[x]}");
                    }

                    mapRow.Add(tile);
                }

                _map.Add(mapRow);
                y++;
            }

            return (map: map, start: start);
        }

        public void PickupKey(Point coord)
        {
            if (!_keys.Remove(coord))
            {
                throw new InvalidOperationException($"There is no uncollected key at {coord}!");
            }
        }

        public void OpenDoor(Point coord)
        {
            if (!_doors.Remove(coord))
            {
                throw new InvalidOperationException($"There is no closed door at {coord}!");
            }
        }

        private static readonly IReadOnlyDictionary<TileType, char> MapSymbols = new Dictionary<TileType, char>
        {
            [TileType.Start]    = '●',
            [TileType.Wall]     = '▢',
            [TileType.Visited]  = '·',
            [TileType.Key]      = '⚷',
            [TileType.Door]     = 'Ω',
            [TileType.Walkable] = '·'
        };

        private static readonly IReadOnlyDictionary<TileType, ConsoleColor> MapColors = new Dictionary<TileType, ConsoleColor>
        {
            [TileType.Start]    = ConsoleColor.DarkCyan,
            [TileType.Wall]     = ConsoleColor.White,
            [TileType.Visited]  = ConsoleColor.Cyan,
            [TileType.Key]      = ConsoleColor.Cyan,
            [TileType.Door]     = ConsoleColor.Red,
            [TileType.Walkable] = ConsoleColor.DarkGray
        };

        public TileType this[Point point] =>
            point switch
            {
                _ when _keys.ContainsKey(point) => TileType.Key,
                _ when _doors.ContainsKey(point) => TileType.Door,
                _ when _visitableMap != null => _visitableMap[point.Y][point.X],
                _ => _map[point.Y][point.X]
            };

        public Map Clone()
        {
            var clone = new Map();
            foreach (var (key, value) in _keys)
            {
                clone._keys.Add(key, value);
            }
            foreach (var (key, value) in _doors)
            {
                clone._doors.Add(key, value);
            }
            return clone;
        }

        public Map CloneMap()
        {
            var clone = Clone();
            clone._visitableMap = new();

            foreach (var mapRow in _map)
            {
                var cloneRow = new List<TileType>();
                foreach (var tile in mapRow)
                {
                    cloneRow.Add(tile);
                }
                clone._visitableMap.Add(cloneRow);
            }

            return clone;
        }

        public void Visit(Point p)
        {
            _visitableMap[p.Y][p.X] = TileType.Visited;
        }

        public override bool Equals(object? other)
        {
            if (!(other is Map otherMap))
                return false;

            if (_keys.Count != otherMap._keys.Count)
                return false;

            if (_doors.Count != otherMap._doors.Count)
                return false;

            foreach (var (key, value) in _keys)
            {
                if (!otherMap._keys.TryGetValue(key, out var otherValue) || value != otherValue)
                    return false;
            }

            foreach (var (key, value) in _doors)
            {
                if (!otherMap._doors.TryGetValue(key, out var otherValue) || value != otherValue)
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();

            foreach (var (key, value) in _keys)
            {
                hash.Add((key, value));
            }

            foreach (var (key, value) in _doors)
            {
                hash.Add((key, value));
            }

            return hash.ToHashCode();
        }

        public void Print(Point start)
        {
            WriteLine();
            for (int y = 0; y < _map.Count; y ++)
            {
                for (int x = 0; x < _map[0].Count; x++)
                {
                    var tile = (x, y) switch
                    {
                        var p when p == start => TileType.Start,
                        _ => this[(x,y)]
                    };

                    ForegroundColor = MapColors[tile];
                    Write(MapSymbols[tile]);
                    ResetColor();
                }
                WriteLine();
            }
            WriteLine();
        }
    }

    public enum TileType
    {
        Wall,
        Start,
        Walkable,
        Key,
        Door,
        Visited
    }

    public record Point
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; init; }
        public int Y { get; init; }

        public static implicit operator Point((int x, int y) point)
        {
            return new (point.x, point.y);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}
