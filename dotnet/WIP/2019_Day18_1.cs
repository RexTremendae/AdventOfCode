using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Console;

namespace Day20
{
    public class Program
    {
        public static void Main()
        {
            new Program().Run();
        }

        public void Run()
        {
            var maze = Load("Day18_small.txt");

            for (int y = maze.Min.Y; y <= maze.Max.Y; y++)
            {
                for (int x = maze.Min.X; x <= maze.Max.X; x++)
                {
                    Write(maze[(x, y)]);
                }

                WriteLine();
            }

            FindShortestPath(maze);
            WriteLine();
        }

        private long FindShortestPath(Maze maze)
        {
            var goal = maze.Keys.Count;

            var queue = new List<(Point start, string collectedKeys, long distance)>();
            queue.Add((maze.Start, "", 0));
            var progress = 0;

            while(true)
            {
                var q = queue.First();
                queue.RemoveAt(0);
                if (q.collectedKeys.Length == goal) break;
                if (q.collectedKeys.Length > progress)
                {
                    progress = q.collectedKeys.Length;
                    WriteLine(goal - progress);
                }
                foreach (var kvp in maze.Keys)
                {
                    if (q.collectedKeys.Contains(kvp.Key)) continue;
                    var distance = q.distance + FindShortestPath(maze, q.start, q.collectedKeys, kvp.Key);
                    var collectedKeys = q.collectedKeys + kvp.Key;
                    //WriteLine($"[{collectedKeys}] {distance}");
                    var idx = 0;
                    while (idx < queue.Count)
                    {
                        if (queue[idx].distance > distance) break;
                        idx++;
                    }
                    queue.Insert(idx, (kvp.Value, collectedKeys, distance));
                }
            }

            return 0;
        }

        private long FindShortestPath(Maze maze, Point pos, string collectedKeys, char goal)
        {
            var visited = new HashSet<(Point pos, string keys)>();
            var queue = new List<(long prio, Point pos, string keys, long dist)>();
            queue.Add((0, maze.Start, "", 0));

            while (true)
            {
                var q = queue.First();
                queue.RemoveAt(0);

                var posData = maze[(q.pos)];
                if (posData == goal) return q.dist;

                var keys = q.keys;
                if (posData.IsKey())
                {
                    var keyList = q.keys.ToCharArray().ToList();
                    if (!keyList.Contains(posData))
                    {
                        keyList.Add(posData);
                        keys = string.Join("", keyList.OrderBy(x => x));
                        if (keyList.Count == goal) return q.dist;
                    }
                }

                visited.Add((q.pos, keys));

                foreach (var candidate in GetCandidates(maze, q.pos, q.keys))
                {
                    if (visited.Contains((candidate, keys))) continue;
                    int idx = 0;
                    var prio = q.dist + Heuristic(maze, q.pos, q.keys);

                    while (idx < queue.Count)
                    {
                        if (prio < queue[idx].prio) break;
                        idx++;
                    }

                    queue.Add((prio, candidate, keys, q.dist+1));
                }
            }
        }

        private long Heuristic(Maze maze, Point pos, string collectedKeys)
        {
            var h = 0L;
            foreach (var kvp in maze.Keys)
            {
                if (collectedKeys.Contains(kvp.Key)) continue;
                h += pos.Manhattan(kvp.Value);
                if (maze.Doors.TryGetValue(char.ToUpper(kvp.Key), out var door))
                    h += kvp.Value.Manhattan(door);
            }

            return h;
        }

        private IEnumerable<Point> GetCandidates(Maze maze, Point pos, string keys)
        {
            foreach (var candidate in new[] { new Point(pos.X + 1, pos.Y), new Point(pos.X - 1, pos.Y),
                                              new Point(pos.X, pos.Y + 1), new Point(pos.X, pos.Y - 1)})
            {
                var data = maze[candidate];
                if (data == '.' || data == '@') yield return candidate;
                if (data.IsKey()) yield return candidate;
                if (data.IsDoor() && keys.Contains(char.ToLower(data))) yield return candidate;
            }
        }

        public Maze Load(string filename)
        {
            var maze = new Maze();
            int y = 0;

            foreach (var line in File.ReadAllLines(filename))
            {
                int x = 0;
                foreach (var c in line)
                {
                    if (c == ' ' || c == '\n' || c == '\r') { x++; continue; }
                    if (c == '@')
                    {
                        maze.Start = (x, y);
                    }
                    else if (c.IsDoor())
                    {
                        maze.AddDoor(c, (x, y));
                    }
                    else if (c.IsKey())
                    {
                        maze.AddKey(c, (x, y));
                    }

                    maze.AddPiece((x, y), c);
                    x++;
                }
                y++;
            }

            return maze;
        }
    }

    public class Maze
    {
        public Point Min { get; private set; }
        public Point Max { get; private set; }
        public Point Start { get; set; }

        private Dictionary<Point, char> _data;
        public Dictionary<char, Point> Keys;
        public Dictionary<char, Point> Doors;

        public Maze()
        {
            _data = new Dictionary<Point, char>();
            Keys = new Dictionary<char, Point>();
            Doors = new Dictionary<char, Point>();
        }

        public char this[Point pos]
        {
            get
            {
                if (_data.TryGetValue(pos, out var data)) return data;
                return ' ';
            }
        }

        public void AddKey(char key, Point pos)
        {
            Keys.Add(key, pos);
        }

        public void AddDoor(char door, Point pos)
        {
            Doors.Add(door, pos);
        }

        public void AddPiece(Point pos, char chr)
        {
            MinMax(pos);
            _data.Add(pos, chr);
        }

        private void MinMax(Point pos)
        {
            Min = (Math.Min(Min.X, pos.X), Math.Min(Min.Y, pos.Y));
            Max = (Math.Max(Max.X, pos.X), Math.Max(Max.Y, pos.Y));
        }
    }

    public static class Extensions
    {
        public static bool IsKey(this char data)
        {
            return (data >= 'a' && data <= 'z');
        }

        public static bool IsDoor(this char data)
        {
            return (data >= 'A' && data <= 'Z');
        }
    }

    public struct Point
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator Point((int x, int y) pos) => new Point(pos.x, pos.y);
        public static implicit operator (int x, int y) (Point pos) => (pos.X, pos.Y);

        public int X { get; }

        public int Y { get; }

        public static Point Parse(string input)
        {
            var split = input.Split(',');
            int x = int.Parse(split[0]);
            int y = int.Parse(split[1]);

            return new Point(x, y);
        }

        public int Manhattan(Point other)
        {
            return Math.Abs(other.X - X) + Math.Abs(other.Y + Y);
        }

        public static Point operator +(Point p1, Point p2)
        {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Point operator -(Point p1, Point p2)
        {
            return new Point(p1.X - p2.X, p1.Y - p2.Y);
        }

        public override int GetHashCode()
        {
            return (X, Y).GetHashCode();
        }

        public static bool operator == (Point p1, Point p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator != (Point p1, Point p2)
        {
            return !p1.Equals(p2);
        }

        public override bool Equals(object other)
        {
            if (!(other is Point otherPoint)) return false;
            return X == otherPoint.X && Y == otherPoint.Y;
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}