using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Console;

namespace Day20
{
    using PortalCoordinateLookup = Dictionary<(int x, int y), (string label, PortalType type)>;
    using PortalLabelLookup = Dictionary<string, List<((int x, int y) coord, PortalType type)>>;
    using ConnectionLookup = Dictionary<string, List<(string, int)>>;
    using Map = List<List<char>>;

    public enum PortalType
    {
        Inner, Outer
    }

    public class Program
    {
        private const char PortalChar   = '⌘';
        private const char WalkableChar  = '·';
        private const char WallChar     = '▢';

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
            foreach (var (from, toList) in connections)
            {
                foreach (var (to, steps) in toList)
                {
                    WriteLine($"{from} -> {to} : {steps}");
                }
            }
            WriteLine();

            WriteLine($"Shortest path: {FindShortest(connections)}");
            WriteLine();
        }

        private (string, int) FindShortest(ConnectionLookup connections)
        {
            var label = "AA";
            var visited = new HashSet<(string, string)>();
            var q = new List<(string label, HashSet<(string, string)> visited, int steps, string path)>();
            q.Add((label, visited, 0, "AA"));

            while (q.Any())
            {
                var dq = q[0];
                q.RemoveAt(0);
                label = dq.label;
                if (dq.label == "ZZ")
                {
                    return (dq.path, dq.steps - 1);
                }

                foreach (var (toLabel, steps) in connections[dq.label])
                {
                    if (visited.Contains((label, toLabel)))
                    {
                        continue;
                    }

                    visited = Clone(dq.visited);
                    visited.Add((label, toLabel));
                    visited.Add((toLabel, label));

                    var addSteps = dq.steps + steps;
                    var idx = 0;
                    while (idx < q.Count && addSteps > q[idx].steps)
                    {
                        idx++;
                    }
                    q.Insert(idx, (toLabel, visited, dq.steps + steps, dq.path + $" -> {toLabel}"));
                }
            }

            return (string.Empty, -1);
        }

        private ConnectionLookup FindConnections(Map map, PortalCoordinateLookup portalByCoordinate, PortalLabelLookup portalByLabel)
        {
            var connections = new ConnectionLookup();

            foreach (var (label, portalList) in portalByLabel)
            {
                var (lx, ly) = portalList
                    .Where(p => p.type == PortalType.Outer)
                    .Select(p => p.coord)
                    .Single();

                var q = new List<((int x, int y) pos, int steps, Map map)>();
                q.Add(((lx, ly), 0, Clone(map)));

                while (q.Any())
                {
                    var dq = q[0];
                    q.RemoveAt(0);
                    var x = dq.pos.x;
                    var y = dq.pos.y;
                    foreach (var pos in new (int x, int y)[] {
                        (x+1, y),(x-1, y),
                        (x, y+1),(x, y-1)
                    })
                    {
                        if (portalByCoordinate.TryGetValue(pos, out var portal) && portal.label != label)
                        //if (portalByCoordinate.TryGetValue(pos, out var portal) && type == PortalType.Inner)
                        {
                            Add(connections, label, portal.label, dq.steps);
                            Add(connections, portal.label, label, dq.steps);
                        }
                        else if (dq.map[pos.y][pos.x] == WalkableChar)
                        {
                            var visited = Clone(dq.map);
                            visited[pos.y][pos.x] = '*';
                            q.Add((pos, dq.steps+1, visited));
                        }
                    }
                }
            }

            return connections;
        }

        private void Add(ConnectionLookup connections, string from, string to, int steps)
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

        private HashSet<(string, string)> Clone(HashSet<(string, string)> hashSet)
        {
            var clone = new HashSet<(string, string)>();
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

                portalByCoordinate.Add((px, py), (label, portalType));
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
