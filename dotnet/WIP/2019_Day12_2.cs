using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static System.Console;

namespace Day12
{
    public class Program
    {
/*
        // Puzzle input
        private (int id, Point3D position, Point3D velocity)[] _moons = new (int, Point3D, Point3D)[] {
            (0, (x:  6, y: 10, z: 10), (x: 0, y: 0, z: 0)),
            (1, (x: -9, y:  3, z: 17), (x: 0, y: 0, z: 0)),
            (2, (x:  9, y: -4, z: 14), (x: 0, y: 0, z: 0)),
            (3, (x:  4, y: 14, z:  4), (x: 0, y: 0, z: 0))
        };
*/

        // Easy example
        private (int id, Point3D position, Point3D velocity)[] _moons = new (int, Point3D, Point3D)[] {
            (0, (x: -1, y:   0, z:  2), (x: 0, y: 0, z: 0)),
            (1, (x:  2, y: -10, z: -7), (x: 0, y: 0, z: 0)),
            (2, (x:  4, y:  -8, z:  8), (x: 0, y: 0, z: 0)),
            (3, (x:  3, y:   5, z: -1), (x: 0, y: 0, z: 0))
        };

/*
        // Hard example
        private (int id, Point3D position, Point3D velocity)[] _moons = new (int, Point3D, Point3D)[] {
            (0, (x: -8, y: -10, z:  0), (x: 0, y: 0, z: 0)),
            (1, (x:  5, y:   5, z: 10), (x: 0, y: 0, z: 0)),
            (2, (x:  2, y:  -7, z:  3), (x: 0, y: 0, z: 0)),
            (3, (x:  9, y:  -8, z: -3), (x: 0, y: 0, z: 0))
        };
*/
        private HashSet<State> _history = new HashSet<State>();

        public static void Main()
        {
            var start = DateTime.Now;
            new Program().Run();
            var duration = DateTime.Now - start;
            WriteLine($"{duration.TotalMinutes:0.00} minutes passed");
        }

        public void Run()
        {
            char dimension = 'X';
            var cycle = new long[3];

            for (int i = 0; i < 3; i++, dimension++)
            {
                var count = 0L;
                var state = new State(_moons, dimension);
                while(!_history.Contains(state))
                {
                    _history.Add(state);

                    DoTimeStep(dimension);
                    count++;

                    var countFormat = count.ToString("# ### ### ###").PadLeft(13);
                    if (count % 1000 == 0) WriteLine($"{countFormat}...");

                    state = new State(_moons, dimension);
                }

                WriteLine($"{dimension}: {count}");
                cycle[dimension-'X'] = count;
            }

            var final = 0L;
            var xDiff = cycle[0];
            var yDiff = cycle[1];
            var zDiff = cycle[2];

            do
            {
                final += Math.Min(xDiff, Math.Min(yDiff, zDiff));

                xDiff = cycle[0] - final % cycle[0];
                yDiff = cycle[1] - final % cycle[1];
                zDiff = cycle[2] - final % cycle[2];
            }
            while (xDiff != cycle[0] || yDiff != cycle[1] || zDiff != cycle[2]);

            WriteLine();
            WriteLine(final);
        }

        private void PrintState(int step)
        {
            WriteLine($"{step} steps:");
            foreach (var moon in _moons)
            {
                WriteLine($"P: {moon.position.ToString()}     V: {moon.velocity.ToString()}");
            }
            WriteLine();
        }

        private void DoTimeStep(char dimension)
        {
            dimension = char.ToLower(dimension);

            foreach (var moon1 in _moons)
            foreach (var moon2 in _moons)
            {
                if (moon1.id >= moon2.id) continue;
                if (dimension == 'x')
                {
                if (moon1.position.X != moon2.position.X)
                {
                    var change = moon1.position.X > moon2.position.X ? -1 : 1;
                    moon1.velocity.X += change;
                    moon2.velocity.X -= change;
                }
                }
                if (dimension == 'y')
                {
                if (moon1.position.Y != moon2.position.Y)
                {
                    var change = moon1.position.Y > moon2.position.Y ? -1 : 1;
                    moon1.velocity.Y += change;
                    moon2.velocity.Y -= change;
                }
                }
                if (dimension == 'z')
                {
                if (moon1.position.Z != moon2.position.Z)
                {
                    var change = moon1.position.Z > moon2.position.Z ? -1 : 1;
                    moon1.velocity.Z += change;
                    moon2.velocity.Z -= change;
                }
                }
            }

            foreach (var moon in _moons)
            {
                moon.position.X += moon.velocity.X;
                moon.position.Y += moon.velocity.Y;
                moon.position.Z += moon.velocity.Z;
            }
        }
    }

    public class State
    {
        private long[] _data;

        public State((int id, Point3D position, Point3D velocity)[] moons, char dimension)
        {
            dimension = char.ToLower(dimension);
            _data = new long[8];

            int idx = 0;
            foreach (var m in moons)
            {
                if (dimension == 'x') _data[idx++] = m.velocity.X;
                if (dimension == 'y') _data[idx++] = m.velocity.Y;
                if (dimension == 'z') _data[idx++] = m.velocity.Z;

                if (dimension == 'x') _data[idx++] = m.position.X;
                if (dimension == 'y') _data[idx++] = m.position.Y;
                if (dimension == 'z') _data[idx++] = m.position.Z;
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as State;

            for (int i = 0; i < _data.Length; i++)
            {
                if (_data[i] != other._data[i]) return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            var hashBase = 0L;
            for (int i = 0; i < _data.Length; i++)
            {
                hashBase += _data[i];
            }

            return hashBase.GetHashCode();
        }
    }

    public class Point3D
    {
        public Point3D(long x, long y, long z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static implicit operator Point3D((long x, long y, long z) pos) => new Point3D(pos.x, pos.y, pos.z);
        public static implicit operator (long x, long y, long z) (Point3D pos) => (pos.X, pos.Y, pos.Z);

        public long X { get; set; }
        public long Y { get; set; }
        public long Z { get; set; }

        public long Manhattan() => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);

        public static Point3D operator +(Point3D p1, Point3D p2)
        {
            return new Point3D(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
        }

        public static Point3D operator -(Point3D p1, Point3D p2)
        {
            return new Point3D(p1.X - p2.X, p1.Y - p2.Y, p1.Z + p2.Z);
        }

        public override int GetHashCode()
        {
            return (X, Y, Z).GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (!(other is Point3D otherPoint)) return false;
            return X == otherPoint.X &&
                   Y == otherPoint.Y &&
                   Z == otherPoint.Z;
        }

        public override string ToString()
        {
            return $"({Format(X)}, {Format(Y)}, {Format(Z)})";
        }

        private string Format(long coord)
        {
            return coord.ToString().PadLeft(6);
        }
    }
}