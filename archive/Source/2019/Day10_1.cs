using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Console;

namespace Day10
{
    public class Program
    {
        public static void Main()
        {
            new Program().Run();
        }

        private long GCF(long a, long b)
        {
            var f = 1L;
            var g = 1L;
            while (f < a && f < b)
            {
                f++;
                if (a % f == 0 && b % f == 0) g = f;
            }
            return g;
        }

        private void Run()
        {
            var astroids = ReadData("Day10.txt");
            var insights = new Dictionary<(Point, Point), bool>();

            var maxCount = 0;
            foreach (var ai in astroids)
            {
                int count = 0;
                foreach (var aj in astroids)
                {
                    if (ai == aj) continue;

                    if (!insights.TryGetValue((ai, aj), out var inSight))
                    {
                        inSight = IsInsightOf(astroids, ai, aj);
                        insights[(ai, aj)] = inSight;
                        insights[(aj, ai)] = inSight;
                    }

                    if (inSight) count++;
                }

                maxCount = Math.Max(maxCount, count);
            }

            WriteLine(maxCount);
        }

        private bool IsInsightOf(HashSet<Point> astroids, Point a1, Point a2)
        {
            var t = new Point(a2.X - a1.X, a2.Y - a1.Y);

            var stepX = 0L;
            var stepY = 0L;

            var tx = 1;
            var ty = 1;

            if (t.X < 0)
            {
                t = (-t.X, t.Y);
                tx = -1;
            }

            if (t.Y < 0)
            {
                t = (t.X, -t.Y);
                ty = -1;
            }

            if (t.X == 0) stepY = 1;
            else if (t.Y == 0) stepX = 1;
            else
            {
                var gcf = GCF(t.X, t.Y);
                stepX = t.X / gcf;
                stepY = t.Y / gcf;
            }

            var x = stepX;
            var y = stepY;

            var inSight = true;
            while (x < t.X || y < t.Y)
            {
                var test = astroids.Contains((a1.X + tx*x, a1.Y + ty*y));
                if (test)
                {
                    inSight = false;
                    break;
                }
                x += stepX;
                y += stepY;
            }

            return inSight;
        }

        private HashSet<Point> ReadData(string filename)
        {
            var lines = File.ReadAllLines(filename);
            int y = 0;
            var astroids = new HashSet<Point>();

            foreach (var l in lines)
            {
                for (int x = 0; x < l.Length; x++)
                {
                    if (l[x] == '#')
                    {
                        astroids.Add((x, y));
                    }
                }

                y++;
            }

            return astroids;
        }
    }

    public struct Point
    {
        public Point(long x, long y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator Point((long x, long y) pos) => new Point(pos.x, pos.y);
        public static implicit operator (long x, long y) (Point pos) => (pos.X, pos.Y);

        public long X { get; }

        public long Y { get; }

        public long Manhattan()
        {
            return Math.Abs(X) + Math.Abs(Y);
        }

        public static Point Parse(string input)
        {
            var split = input.Split(',');
            int x = int.Parse(split[0]);
            int y = int.Parse(split[1]);

            return new Point(x, y);
        }

        public static Point operator +(Point p1, Point p2)
        {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Point operator -(Point p1, Point p2)
        {
            return new Point(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static bool operator == (Point p1, Point p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator != (Point p1, Point p2)
        {
            return !p1.Equals(p2);
        }

        public override int GetHashCode()
        {
            return (X, Y).GetHashCode();
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
