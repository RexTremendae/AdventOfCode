using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace Day24
{
    public class Program
    {
        public static void Main()
        {
            var start = DateTime.Now;
            new Program().Run(Challenge);
            var duration = DateTime.Now - start;
            WriteLine(duration.Minutes.ToString().PadLeft(2, '0') + ":" + duration.Seconds.ToString().PadLeft(2, '0'));
        }

        public void Run(string map)
        {
            var (parsedMap, points) = Parse(map);
            points = points.ToDictionary(key => (char)(key.Key - '0' + 'A'), value => value.Value);
            var distances = FindShortestDistances(parsedMap, points);

            foreach (var kvp in distances.OrderBy(d => d.Key.Item1).ThenBy(d => d.Key.Item2))
            {
                WriteLine($"({kvp.Key.Item1}, {kvp.Key.Item2}): {kvp.Value}");
            }

            WriteLine();
            WriteLine("---------------");
            WriteLine();

            WriteLine(FindShortestPath(points.Keys.ToArray(), distances));
        }

        public int FindShortestPath(char[] keys, Dictionary<(char, char), int> distances)
        {
            keys = keys.OrderBy(x => x).ToArray();
            var q = new List<(char[], char[])>();
            Enqueue(q, new char[] {}, keys, 0);
            var shortest = int.MaxValue;

            while(q.Any())
            {
                var deQ = q.First();
                q.RemoveAt(0);

                if (deQ.Item2.Length == 0)
                {
                    var path = deQ.Item1;
                    var pathLength = 0;
                    for (int i = 1; i < path.Length; i++)
                        pathLength += distances[(path[i-1], path[i])];
                    pathLength += distances[(path.Last(), keys.First())];
                    
                    if (pathLength < shortest) shortest = pathLength;
                    /*
                    WriteLine(
                        string.Join(", ", deQ.Item1.Select(c => c.ToString()))
                        + ": " + pathLength);
                    */
                }

                for (int i = 0; i < deQ.Item2.Length; i++)
                    Enqueue(q, deQ.Item1, deQ.Item2, i);
            }

            return shortest;
        }

        public void Enqueue(List<(char[], char[])> q, char[] array1, char[] array2, int index)
        {
            if (index >= array2.Length) return;

            var theRest = array2[0..index].Concat(array2[(index+1)..]).ToArray();
            var theFirst = array1.Concat(new [] {array2[index]}).ToArray();
            q.Add((theFirst, theRest));
        }

        public Dictionary<(char, char), int> FindShortestDistances(List<string> map, Dictionary<char, Point> points)
        {
            var distances = new Dictionary<(char, char), int>();

            foreach (var (point, pos) in points)
            {
                foreach (var (toPoint, dist) in FindShortestDistanceFor(point, pos, map))
                    distances.Add((point, toPoint), dist);
            }

            return distances;
        }

        public IEnumerable<(char, int)> FindShortestDistanceFor(char point, Point position, List<string> map)
        {
            var q = new List<(Point, int)>();
            var visited = new HashSet<Point>();
            visited.Add(position);
            q.Add((position, 0));

            while (q.Any())
            {
                var deQ = q.First();
                q.RemoveAt(0);

                var deQ_c = map[deQ.Item1.Y][deQ.Item1.X];
                if (!deQ.Item1.Equals(position) && deQ_c != '.')
                {
                    yield return ((char)(deQ_c - '0' + 'A'), deQ.Item2);
                    //continue;
                }

                EnqueueCandidates(map, q, deQ, visited);
            }
        }

        public void EnqueueCandidates(List<string> map, List<(Point, int)> q, (Point, int) deQ, HashSet<Point> visited)
        {
            int x = deQ.Item1.X;
            int y = deQ.Item1.Y;
            int d = deQ.Item2;

            TryEnqueue(map, q, new Point(x, y+1), d, visited);
            TryEnqueue(map, q, new Point(x, y-1), d, visited);
            TryEnqueue(map, q, new Point(x+1, y), d, visited);
            TryEnqueue(map, q, new Point(x-1, y), d, visited);
        }

        public void TryEnqueue(List<string> map, List<(Point, int)> q, Point candidate, int distance, HashSet<Point> visited)
        {
            int x = candidate.X;
            int y = candidate.Y;

            if (x < 0 || y < 0) return;
            if (x >= map[0].Length || y >= map.Count) return;
            if (visited.Contains(candidate)) return;
            if (map[y][x] == '#') return;

            q.Add((candidate, distance+1));
            visited.Add(candidate);
        }

        public (List<string>, Dictionary<char, Point>) Parse(string map)
        {
            var data = new List<string>();
            var points = new Dictionary<char, Point>();

            int y = 0;
            foreach (var lineRaw in map.Split('\n'))
            {
                var line = lineRaw.LastOrDefault() == '\r' ? lineRaw[..^1] : lineRaw;
                if (string.IsNullOrEmpty(line)) continue;

                data.Add(line);

                for (int x = 0; x < line.Length; x++)
                {
                    if (line[x] != '#' && line[x] != '.')
                    {
                        points.Add(line[x], new Point(x, y));
                    }
                }

                y++;
            }

            return (data, points);
        }

        public class Point
        {
            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; set; }
            public int Y { get; set; }

            public override bool Equals(object? obj)
            {
                try
                {
                    var other = obj as Point;
                    return other!.X == X && other!.Y == Y;
                }
                catch
                {
                    WriteLine("Compare with " + obj?.GetType()?.Name ?? "<NULL>");
                    throw;
                }
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(X, Y);
            }
        }

        public const string Sample =
@"
###########
#0.1.....2#
#.#######.#
#4.......3#
###########
";

        public const string Challenge =
@"
###################################################################################################################################################################################
#.....#.#.....#...#....4#.....#.#...#.........#...#...............#...................#...#.#...........#.#...........#.#.#.#.........#.#.......#...#...........#.....#...#7..#.#.#
###.#.#.###.#.#.###.#.#.#.#.#.#.#.#.#.#.#.#.#.#.#.###.#.###.#.###.#.#.#.###.###.#.#####.###.#.#.###.#.#.#.#.#.#.#.#.#.#.#.#.###.#####.#.#.#.#.#####.#.#.#.###.#.#.#.#.#####.#.#.#.#
#.#.....#.#.#...#.........#.....#.....#.......#.#.#.............#.#.#.#.....#.#.......#.....#.........#...#.......#.....#.#.#.............#...........#.#.....#.#.....#.......#.#.#
#.#.#.#.#.#.#.#.#.#.#####.#####.###.###.#.###.#.###.###.#.#####.#.#.#.#.#.###.#.#.###.#.#.#.#.###.#########.###########.#.#.###.#.#.###.###.#.###.###.#.#.#####.#.###.#.#####.#.###
#...........#...#...#.....#.....#...#.#...#.#.....#.........#...#...#.....#.....#.#.#...#...#...#...#.....#.......#...#...#...............#...#...#.............#.....#.#.....#...#
###.#.#.###.#.#.#.#.###.#.###.#####.#.#.#.#.#.###.###.###.#.#.#.###.#.#.#.#.###.#.#.#.###.#####.#########.#.#.#.#.#.###.#.#.#.#.#####.#.#.#.#.###.#.#.#.#.#.#.#.#####.#.###.#.#.#.#
#3#...#.#.#.#.........#...............#...#.#.....#...#.....#...#.......#...#.....#.#.#...#.....#...#.....#.#.#.....#.....#...........#.#.#.#.....#.#.........#.#...#.#.#.#...#...#
#.###.###.#######.###.#.###.#.#.#.###.###.#######.###.#.#####.#####.#.#.#.#.#######.###.###.###.###.###.#.#########.#.#.#.#.#.#####.###.#.###.#.###.#.#####.###.###.###.#.#.#.###.#
#.#...#.....#.#.............#.....#.#.....#.#.....#.#.#.....#.....#.......#.....#.................#...........#...#.#.....#...#.....#...#.......#.#.....#...#...#.#.#...#...#...#.#
#.###.###.###.#.#.#.#####.#.###.#.#.###.#.#.#.#.#.#.#.#.#.#####.#####.#.#.#.#.#.#.###########.#.#.#.#.#.###.#.#.#.#.#.#.#.#.#.###.#.#.#####.#####.#.###.#.#.#.#.#.#.#####.#.###.#.#
#.....#.......#.#.#.#.#...............#...#.#.#.#...#...........#.....#.#...#.................#...#.#.#...#.............#...#.........#...............#...#.#.#.....#.....#.....#.#
#####.#.#######.#.###.#.#.#.#.###.#.#.#.###.###.###.#.#.#.#.#.#.###.#.#.#.#.#######.###.#.###.#.#.#.###.#.#.###.###.#.#.#.#.#####.#####.#.###.#####.###.#.#.#####.#.#.#####.#.#.#.#
#.#...#.........#...#.#...#.......#...#.#.......#...#.#.........#.#.#...#.#.#.#.........#.#.#.......#...#...#...#.#...#.......#...#.....#...#...#.#...#...#...#...........#...#.#.#
#.#####.#.###.#.#.#######.#.###.#.#.#.#########.#.#.#.#.#####.#.#.#######.#.#.###########.#.#########.###.#.#.#.#.###.#.#.###.#########.#.#.#.###.#.#.###.#.#.###.#####.#.###.#.#.#
#.......#.......#...#.#.#...#...#.....#.#...#...#.#.#.#.#...#.....#.#...#...#.............#.......#.......#...#.#.............#.......#.....#...#...#.#.....#.............#...#.#.#
#.#####.###.#####.#.#.#.#.#.#.#.#.#.#.#.#.###.###.#.###.###.#.#.###.#.#.#.#.###.#.###.#.#.#.#.#.#.#.#######.#.#.###.#.#.#.#.###.#.###.###.#####.#.#.#.#.#####.###.#.###.#####.###.#
#..6#...#...#...#...#.#.....#...#.#.#...#...........#.#.#...#.#.#.....#.....#.#.#.....#.......#.................#.#.....#.#.........#...#...#...........#.#2....#.#.......#.#.#.#.#
#.###.###.#.###.#####.#####.#.###.###.#.###.#.#####.#.#.#.#.#.#.###.#.#.#.#.#.#.#.#.#.#.###.#######.#.#.#.#.#####.#.#.#######.###.#####.###.#####.#####.#.#####.###.#######.###.###
#.#.....#...#...#...........#.#.......#.#...#.#.............#...#...#.....#...#.....#.......#.......#.......#...#...#.......#...#.......#.#...#...#.........#...#...#...#.......#.#
#.#.###.#.#.#.#.###.#######.#.#.###.###.#####.###.#.###.#######.#####.#####.#.#####.#.###.#.#.#.#.#####.###.#.#.#.#.#.#.#.#.#############.###.#.#.#.###.#.#.###.#.#.#.#.#####.#.#.#
#...#.........#.....#...#.#...#.....#...#...#.......#.....#...#...#...#...#.............#.#...#.............#.....#...#.#.#.......#.....#.....#.....#...........#...#...#.....#...#
#.#######.#.#.###.#.#.#.#.#.###.#.#.#.###.#.###.#.#.#.#####.#.#.#.#.#.#.#.#.#####.#####.#####.#.#######.###.#.#.###.#.###.#.#.#.#.#.###.#.#.###.#.#.#######.###.#.###.#.#.#.#.###.#
#.....#.......#...#.#...#.....#...#.#...........#.....#.....#.#.#...#.....#.................#.........#.#.......#...........#...#...#.......#0#...#.....#.......#.#...........#...#
#.#.#.#.#.###.#.#.#.###.###.#.#.###.#.#.#####.#######.#.#.#.#.#.###.###.###.#.#####.###.#####.#.#.###.###.###.###.#####.###.#.#.#.#.#.###.#.#.#.#.#.###.#.###.#.#.#.#.#.#.#####.###
#.#.#...#...#.#.......#.............#...........................#.......#...........#.#...#...#.#...#.....#...#.#.#.#.#.#.......#.#...#...#...#...............#.......#.....#.....#
#.#.###.#.#.#.#.#.#####.#.#####.#.#.###.#.#.#.#.#############.#.###.#.#.#.#.#####.#.#.###.#.###.#.#.#######.###.#.#.#.#.#.###.#.#####.#.###.###.#######.#.###.#####.#.#.#.#######.#
#...#.......#.....#...#...#...#.....#5....#...#.......#.#.#...#...........#.#.......#.#...#.#.......#.#.#...#...#.....#.............#...#...#.....#.................#.....#.#...#.#
#######.#.#.#######.#####.###.#.#.#######.#.#.#.#.#.#.#.#.#.###.#.###.#.#.#.###.###.#.#.#.###.#.###.#.#.###.#.###.#####.###.#######.#.#.#.#.#.#.#.#########.###.#.#.#.#.#.#.#.#.###
#.#.........#...........#.........#.........#.#.#...........#...#.....#...................#...........#...#...#...#.#.......#...#.....#.#.#.....#.#.............#.........#.#...#.#
#.#.#.###.#.###.#.###.#.###.#.#######.#.###.#.#.#.#########.#.###.#.#####.###.#.#.###.#.#.#.###.#.#####.###.#.###.#.#.###.#.#.#.#.#.#.#.#.###.#.#.###.#.#####.#.#.#######.#.#####.#
#.........#.#.....#.....#...#...#.......#.....#.................#...#...#.....#...#...#.#.#.#...#...........#.#.....#.#.....#...#.#...#.......#.........#.....#.....#.......#...#.#
#.#####.#.#.#.#.#.#.#####.###.###.#.#####.###.#####.###.#.#.#.#.#.###.#.#.#.#.#.#####.###.###.#.#.#.#.#.###.#.#.#.#.#.#.#.#####.#.#.#.#.#.#########.#.#.#.###.#.###.#.#.#.#.#.#.###
#.......#...#...#.....#.#...#...#...#.#.............#.....#.............#.#.......#.......#...#...#...#.....#.......#...#...........#.#...#.#.......#...........#.#.....#.....#...#
#.#.#.#.#.###.#.#.#.#.#.#.#.#.#.#.#.#.#.###.#.#.#####.#.###.#.#.#####.#.#.#.#####.#.#.###.###.#.#.#.#.#.#.#.#####.#.#.#####.###.###.###.###.#.#.#.#.#.#.#########.#####.#.#.#.#.#.#
#.#.#.#.............#...#...#.#.....#...........#.........#...#.#.#...#.#.........#.........#.........#.....#.........#...#...#...#..1#.....#.#.#...#.#.....#...#...........#.....#
###################################################################################################################################################################################
";
    }
}