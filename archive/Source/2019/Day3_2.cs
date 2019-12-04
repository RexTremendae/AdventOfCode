using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day3
{
    public class Program
    {
        public static void Main()
        {
            new Program().Run();
        }

        public void Run()
        {
            var map = new Dictionary<Point, long>();
            int wire = 0;
            var pos = new Point(0, 0);
            var distance = 0L;
            long minDistance = long.MaxValue;

            foreach (var line in File.ReadAllLines("Day3.txt"))
            {
                if (wire > 1) break;
                pos = (0, 0);
                distance = 0L;

                foreach (var movement in line.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()))
                {
                    var dir = movement[0] switch {
                        'U' => new Point(0, 1),
                        'D' => new Point(0, -1),
                        'L' => new Point(-1, 0),
                        'R' => new Point(1, 0),
                        _ => throw new Exception()
                    };

                    var repeat = int.Parse(movement.Substring(1));
                    for (int r = 0; r < repeat; r++)
                    {
                        pos += dir;
                        distance ++;

                        if (wire == 0)
                        {
                            if (map.ContainsKey(pos)) continue;
                            map.Add(pos, distance);
                        }
                        else if (map.TryGetValue(pos, out var otherDistance))
                        {
                            Console.WriteLine($"{pos}: {distance} + {otherDistance}");
                            var sumDistance = distance + otherDistance;
                            if (sumDistance < minDistance)
                            {
                                minDistance = distance + otherDistance;
                            }
                        }
                    }
                }
 
                wire++;
           }

            Console.WriteLine(minDistance);
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
}