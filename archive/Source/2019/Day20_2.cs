using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Console;

namespace Day20
{
    using PortalCoordinateLookup = Dictionary<(int x, int y), Portal>;
    using PortalLabelLookup = Dictionary<string, HashSet<((int x, int y) coord, PortalType type)>>;
    using ConnectionLookup = Dictionary<Portal, HashSet<(Portal, int)>>;
    using Map = List<List<char>>;

    public class PortalMovement
    {
        public int ToLevel { get; init; }
        public Portal From { get; init; }
        public Portal To { get; init; }
    }

    public class Portal
    {
        public string label { get; init; }
        public PortalType type { get; init; }

        public override string ToString()
        {
            var tt = type == PortalType.Inner ? "I" : "O";
            return $"{label} {Program.ToShortString(type)}";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(label, type);
        }

        public override bool Equals(object other)
        {
            if (!(other is Portal otherPortal))
            {
                return false;
            }

            return label == otherPortal.label && type == otherPortal.type;
        }
    }

    public enum PortalType
    {
        Inner, Outer
    }

    public class Program
    {
        private const char PortalChar    = '⌘';
        private const char WalkableChar  = '·';
        private const char WallChar      = '▢';

        public static void Main()
        {
            new Program().Run();
        }

        public void Run()
        {
            System.Console.OutputEncoding = System.Text.Encoding.UTF8;

            var map = ReadData("Day20.txt");
            var (portalByCoordinate, portalByLabel) = PrepareData(map);
            foreach (var (coord, portal) in portalByCoordinate)
            {
                WriteLine($"{portal.label}: ({coord.x.ToString().PadLeft(2)}, {coord.y.ToString().PadLeft(2)})  [{portal.type}]");
            }
            PrintMap(Clone(map));

            WriteLine();
            var connections = FindConnections(map, portalByCoordinate, portalByLabel);
            foreach (var (from, toList) in connections.OrderBy(cnn => cnn.Key.label))
            {
                foreach (var (to, steps) in toList)
                {
                    WriteLine($"{from} -> {to} : {steps}");
                }
            }
            WriteLine();

            var (path, ssteps) = FindShortest(connections);

            WriteLine($"Shortest path: {ssteps}");
            WriteLine();
            WriteLine($"{path}");
            WriteLine();
        }

        private (string, int) FindShortest(ConnectionLookup connections)
        {
            var debug = false;

            var visited = new HashSet<(Portal, Portal, int)>();
            var q = new List<(PortalMovement movement, HashSet<(Portal, Portal, int)> visited, int steps, string path)>();

            var portalMovement = new PortalMovement
            {
                ToLevel = 0,
                From = new Portal { label = "-", type = PortalType.Outer },
                To = new Portal { label = "AA", type = PortalType.Inner }
            };

            q.Add((portalMovement, visited, 0, "AA"));

            while (q.Any())
            {
                var dq = q[0];
                q.RemoveAt(0);
                portalMovement = dq.movement;

                if (debug)
                    WriteLine(Environment.NewLine + $"Dequeuing {dq.path}" + Environment.NewLine);

                var fromPortal = new Portal { label = portalMovement.To.label, type = portalMovement.To.type == PortalType.Inner ? PortalType.Outer : PortalType.Inner };
                foreach (var (toPortal, steps) in connections[fromPortal])
                {
                    var addSteps = dq.steps + steps;

                    var level = portalMovement.ToLevel;

                    if (toPortal.label == "ZZ")
                    {
                        if (level == 0)
                        {
                            return ($"{dq.path} -| {level} |-> {toPortal.label}", addSteps - 1);
                        }

                        continue;
                    }

                    if (toPortal.label == "AA" || visited.Contains((fromPortal, toPortal, level)))
                    {
                        continue;
                    }

                    var newLevel = level + (toPortal.type == PortalType.Inner ? +1 : -1);
                    if (newLevel < 0)
                    {
                        continue;
                    }

                    visited = Clone(dq.visited);
                    visited.Add((fromPortal, toPortal, level));

                    var idx = 0;
                    while (idx < q.Count && addSteps > q[idx].steps)
                    {
                        idx++;
                    }

                    var newMovement = new PortalMovement { From = fromPortal, To = toPortal, ToLevel = newLevel };
                    if (debug)
                      WriteLine($"Enqueueing (...) -| {level} |-> {toPortal.label}");
                    q.Insert(idx, (newMovement, visited, addSteps, $"{dq.path}{ToShortString(fromPortal.type)} -| {level}:{steps} |-> {ToShortString(toPortal.type)}{toPortal.label}"));
                }
            }

            return ("-", -1);
        }

        public static string ToShortString(PortalType type)
        {
            return $"[{(type == PortalType.Outer ? "O" : "I")}]";
        }

        private ConnectionLookup FindConnections(Map map, PortalCoordinateLookup portalByCoordinate, PortalLabelLookup portalByLabel)
        {
            var connections = new ConnectionLookup();

            foreach (var (label, portalList) in portalByLabel)
            {
                foreach (var (coord, type) in portalList)
                {
                    var (lx, ly) = coord;

                    var q = new List<((int x, int y) pos, int steps, Map map)>();
                    q.Add(((lx, ly), 0, Clone(map)));

                    while (q.Any())
                    {
                        var dq = q[0];
                        q.RemoveAt(0);
                        var x = dq.pos.x;
                        var y = dq.pos.y;
                        foreach (var pos in new (int x, int y)[]
                        {
                            (x + 1, y), (x - 1, y),
                            (x, y + 1), (x, y - 1)
                        })
                        {
                            if (portalByCoordinate.TryGetValue(pos, out var portal) && portal.label != label)
                            {
                                Add(connections, new Portal {label = label, type = type}, portal, dq.steps);
                                Add(connections, portal, new Portal {label = label, type = type}, dq.steps);
                            }
                            else if (dq.map[pos.y][pos.x] == WalkableChar)
                            {
                                var visited = Clone(dq.map);
                                visited[pos.y][pos.x] = '*';
                                q.Add((pos, dq.steps + 1, visited));
                            }
                        }
                    }
                }
            }

            return connections;
        }

        private void Add(ConnectionLookup connections, Portal from, Portal to, int steps)
        {
            if (!connections.TryGetValue(from, out var toList))
            {
                toList = new();
                connections.Add(from, toList);
            }
            toList.Add((to, steps));
        }

        private void PrintMap(Map map)
        {
            WriteLine();
            for (int y = 0; y < map.Count; y ++)
            {
                for (int x = 0; x < map[0].Count; x++)
                {
                    Write(map[y][x]);
                }
                WriteLine();
            }
            WriteLine();
        }

        private Map ReadData(string filePath)
        {
            var map = new Map();
            foreach (var line in File.ReadAllLines(filePath))
            {
                if (string.IsNullOrEmpty(line))
                {
                    break;
                }

                map.Add(line
                    .Select(c => c == '.'
                        ? WalkableChar
                        : c == '#'
                        ? WallChar
                        : c)
                    .ToList());
            }
            return map;
        }

        private HashSet<(Portal, Portal, int)> Clone(HashSet<(Portal, Portal, int)> hashSet)
        {
            var clone = new HashSet<(Portal, Portal, int)>();
            foreach (var val in hashSet)
            {
                clone.Add(val);
            }

            return clone;
        }

        private Map Clone(Map map)
        {
            var clone = new Map();
            for (int y = 0; y < map.Count; y++)
            {
                var mapRow = map[y];
                var cloneRow = new List<char>();
                clone.Add(cloneRow);
                foreach (var c in mapRow)
                {
                    cloneRow.Add(c);
                }
            }
            return clone;
        }

        private (PortalCoordinateLookup portalByCoordinate, PortalLabelLookup portalByLabel)
        PrepareData(List<List<char>> map)
        {
            var portalByCoordinate = new PortalCoordinateLookup();
            var portalByLabel = new PortalLabelLookup();

            int ysize = map.Count;
            int xsize = map[0].Count;

            for (int y = 0; y < map.Count; y ++)
            for (int x = 0; x < map[0].Count; x++)
            {
                PortalType portalType;
                var current = map[y][x];

                if (!char.IsLetter(current))
                {
                    continue;
                }

                var second = ' ';
                int px, py;

                if (y == 0)
                {
                    second = map[y+1][x];
                    px = x;
                    py = y+1;
                    map[py][px] = PortalChar;
                    map[y][x] = ' ';
                    portalType = PortalType.Outer;
                }
                else if (y == ysize-2)
                {
                    second = map[y+1][x];
                    px = x;
                    py = y;
                    map[py][px] = PortalChar;
                    map[y+1][x] = ' ';
                    portalType = PortalType.Outer;
                }
                else if (x == 0)
                {
                    second = map[y][x+1];
                    px = x+1;
                    py = y;
                    map[py][px] = PortalChar;
                    map[y][x] = ' ';
                    portalType = PortalType.Outer;
                }
                else if (x == xsize-2)
                {
                    second = map[y][x+1];
                    px = x;
                    py = y;
                    map[py][px] = PortalChar;
                    map[y][x+1] = ' ';
                    portalType = PortalType.Outer;
                }
                else if (map[y-1][x] == WalkableChar)
                {
                    second = map[y+1][x];
                    px = x;
                    py = y;
                    map[py][px] = PortalChar;
                    map[y+1][x] = ' ';
                    portalType = PortalType.Inner;
                }
                else if (map[y+2][x] == WalkableChar)
                {
                    second = map[y+1][x];
                    px = x;
                    py = y+1;
                    map[py][px] = PortalChar;
                    map[y][x] = ' ';
                    portalType = PortalType.Inner;
                }
                else if (map[y][x-1] == WalkableChar)
                {
                    second = map[y][x+1];
                    px = x;
                    py = y;
                    map[py][px] = PortalChar;
                    map[y][x+1] = ' ';
                    portalType = PortalType.Inner;
                }
                else if (map[y][x+2] == WalkableChar)
                {
                    second = map[y][x+1];
                    px = x+1;
                    py = y;
                    map[py][px] = PortalChar;
                    map[y][x] = ' ';
                    portalType = PortalType.Inner;
                }
                else
                {
                    throw new InvalidOperationException($"Unexpected portal in data at ({x}, {y})");
                }

                var label = $"{current}{second}";

                portalByCoordinate.Add((px, py), new Portal { label = label, type = portalType });
                if (!portalByLabel.TryGetValue(label, out var portalList))
                {
                    portalList = new();
                    portalByLabel.Add(label, portalList);
                }
                portalList.Add(((px, py), portalType));
            }

            return (portalByCoordinate, portalByLabel);
        }
    }
}
