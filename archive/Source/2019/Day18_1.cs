using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Console;

namespace Day18
{
    using Connections = Dictionary<char, Dictionary<char, (int steps, char[] doors)>>;

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
            var totalKeyCount = map.Keys.Count;

            var connections = FindConnections(start, map, false);
            var initial = connections['@'];
            connections.Remove('@');

            var q = new List<(int prio, int steps, char pos, Connections connections, char[] collectedKeys)>();
            var allKeys = map.Keys.Values.OrderBy(x => x).ToArray();

            foreach (var (key, init) in initial)
            {
                var connectionClone = connections.ToDictionary(key => key.Key, value => value.Value);
                var prio = init.steps + CalculateHeuristic(key, connections, map, new [] { key });

                int idx = 0;
                while (idx < q.Count && q[idx].prio < prio)
                {
                    idx++;
                }

                q.Insert(idx, (prio, init.steps, key, connectionClone, new[] { key }));
            }

            var totalSeconds = 0;
            var lastTotalSeconds = 0;
            var maxCollected = 0;
            var startTime = DateTime.Now;

            var visited = new Dictionary<string, int>();

            for (;;)
            {
                var dq = q[0];
                q.RemoveAt(0);

                if (dq.collectedKeys.Length == allKeys.Length)
                {
                    WriteLine(dq.steps);
                    break;
                }

                totalSeconds = (int)(DateTime.Now - startTime).TotalSeconds;
                var printStatus = false;
                if (dq.collectedKeys.Length > maxCollected)
                {
                    maxCollected = dq.collectedKeys.Length;
                    printStatus = true;
                }
                if (totalSeconds != lastTotalSeconds && totalSeconds % 10 == 0)
                {
                    lastTotalSeconds = totalSeconds;
                    printStatus = true;
                }

                if (printStatus)
                {
                    Write(maxCollected.ToString().PadLeft(2, ' '));
                    Write(" / ");
                    Write(totalKeyCount.ToString().PadLeft(2, ' '));
                    WriteLine($"   ({totalSeconds.ToString().PadLeft(3, ' ')} s)");
                }

                foreach (var (key, connection) in dq.connections[dq.pos].OrderBy(c => c.Value.steps))
                {
                    var collectedKeys = dq.collectedKeys.Union(new[] {key}).ToArray();
                    var hasLockedDoors = false;
                    foreach (var door in connection.doors)
                    {
                        if (!collectedKeys.Contains(door))
                        {
                            hasLockedDoors = true;
                            break;
                        }
                    }

                    if (hasLockedDoors) continue;

                    var visitedState = $"{key} " + string.Join("", collectedKeys.OrderBy(k => k));
                    var steps = dq.steps + connection.steps;
                    if (visited.TryGetValue(visitedState, out var visitedSteps) &&
                        visitedSteps < steps)
                    {
                        continue;
                    }
                    visited[visitedState] = steps;

                    var clone = RemoveKey(dq.connections, dq.pos);

                    var idx = 0;

                    var prio = steps + CalculateHeuristic(key, dq.connections, map, collectedKeys);

                    while (idx < q.Count && q[idx].prio < prio)
                    {
                        idx++;
                    }
                    q.Insert(idx, (prio, steps, key, clone, collectedKeys));
                }
            }
        }

        private int CalculateHeuristic(char label, Connections connections, Map map, char[] collectedKeys)
        {
            var minX = int.MaxValue;
            var minY = int.MaxValue;
            var maxX = int.MinValue;
            var maxY = int.MinValue;

            foreach (var (keyPos, _) in map.Keys.Where(k => !collectedKeys.Contains(k.Value)))
            {
                minX = Math.Min(minX, keyPos.X);
                minY = Math.Min(minY, keyPos.Y);
                maxX = Math.Max(maxX, keyPos.X);
                maxY = Math.Max(maxY, keyPos.Y);
            }

            return (maxX - minX) + (maxY - minY);
        }

        private Connections RemoveKey(Connections connections, char key)
        {
            var result = new Connections();
            var removed = new Dictionary<char, (int steps, char[] doors)>();
            foreach (var (ckey, value) in connections)
            {
                if (ckey == key)
                {
                    removed = value;
                }
                else
                {
                    result.Add(ckey, value.ToDictionary(key => key.Key, value => value.Value));
                }
            }

            foreach (var (from, fmeta) in removed)
            {
                result[from].Remove(key);
                foreach (var (to, tmeta) in removed.Where(k => k.Key != from))
                {
                    var steps = fmeta.steps + tmeta.steps;
                    var doors = fmeta.doors.Concat(tmeta.doors).ToArray();
                    if (!result.TryGetValue(from, out var fromDict) ||
                        !fromDict.TryGetValue(to, out var ftmeta) ||
                        ftmeta.steps > steps)
                    {
                        result[from][to] = (steps, doors);
                        result[to][from] = (steps, doors);
                    }
                }
            }

            return result;
        }

        private Connections FindConnections(Point start, Map map, bool debug = false)
        {
            var all =
                map.Keys.Select(kvp => (coord: kvp.Key, label: kvp.Value))
                .Concat(new[] { (coord: start, label: '@') });

            var connections = new Connections();

            foreach (var (startCoord, startLabel) in all)
            {
                if (debug)
                {
                    WriteLine($"From {startLabel} {startCoord}");
                }
                var keySteps = FindShortestParts(startCoord, map, false);
                foreach (var (label, coord, steps, doors) in keySteps)
                {
                    if (debug)
                    {
                        WriteLine($"-> {label}: {coord} {steps} [{string.Join(", ", doors.Select(d => char.ToUpper(d)))}]");
                    }
                    AddConnection(connections, startLabel, label, steps, doors);
                }
            }

            return connections;
        }

        private void AddConnection(Connections connections,
                                   char from,
                                   char to,
                                   int steps,
                                   char[] doors)
        {
            if (!connections.TryGetValue(from, out var toDict))
            {
                toDict = new();
                connections.Add(from, toDict);
            }

            if (!toDict.TryGetValue(to, out var toSteps))
            {
                toDict.Add(to, (steps, doors.ToArray()));
            }
            else if (toSteps.steps != steps)
            {
                throw new InvalidOperationException("Steps mismatch!");
            }

            if (!connections.TryGetValue(to, out var fromDict))
            {
                fromDict = new();
                connections.Add(to, fromDict);
            }

            if (!fromDict.TryGetValue(from, out var fromSteps))
            {
                if (from != '@')
                {
                    fromDict.Add(from, (steps, doors.ToArray()));
                }
            }
            else if (toSteps.steps != steps)
            {
                throw new InvalidOperationException("Steps mismatch!");
            }
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

        private (char label, Point coord, int steps, char[] doors)[]
        FindShortestParts(Point start, Map map, bool mapDebug = false)
        {
            map = map.CloneMap();
            if (mapDebug)
            {
                WriteLine($"Start at ({start})");
            }

            var points = new[] { (point: start, doors: Array.Empty<char>()) };
            map.Visit(start);
            var steps = 1;

            var keySteps = new List<(char, Point, int, char[])>();

            while (points.Any())
            {
                var candidates = new HashSet<(Point point, char[] doors)>();
                foreach (var p in points)
                {
                    map.Visit(p.point);

                    foreach (var n in GetNeighbors(p.point))
                    {
                        candidates.Add((n, p.doors.ToArray()));
                    }
                }

                var nextPoints = new HashSet<(Point point, char[] doors)>();
                foreach (var n in candidates)
                {
                    var np = n.point;
                    switch (map[np])
                    {
                        case TileType.Walkable:
                            nextPoints.Add((new Point(np.X, np.Y), n.doors.ToArray()));
                            break;

                        case TileType.Key:
                            if (!map.IsVisited(np))
                            {
                                keySteps.Add((map.Keys[np], new Point(np.X, np.Y), steps, n.doors.ToArray()));
                            }

                            if (mapDebug)
                            {
                                WriteLine($"Key [{map.Keys[np]}] {steps} steps from starting point.");
                            }
                            break;

                        case TileType.Door:
                            nextPoints.Add((new Point(np.X, np.Y), n.doors.Concat(new[] {map.Doors[np]}).ToArray()));

                            if (mapDebug)
                            {
                                WriteLine($"Door [{map.Doors[np]}] {steps} steps from starting point.");
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

            return keySteps.ToArray();
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

        public bool IsVisited(Point point)
        {
            return _visitableMap?[point.Y][point.X] == TileType.Visited;
        }

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
            if (_visitableMap == null)
            {
                throw new NullReferenceException("_visitableMap is null!");
            }
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
