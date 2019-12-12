using System;

using static System.Console;

namespace Day12
{
    public class Program
    {
        private (int id, Point3D position, Point3D velocity)[] _moons = new (int, Point3D, Point3D)[] {
            (0, (x:  6, y: 10, z: 10), (x: 0, y: 0, z: 0)),
            (1, (x: -9, y:  3, z: 17), (x: 0, y: 0, z: 0)),
            (2, (x:  9, y: -4, z: 14), (x: 0, y: 0, z: 0)),
            (3, (x:  4, y: 14, z:  4), (x: 0, y: 0, z: 0))
        };

        public static void Main()
        {
            new Program().Run();
        }

        public void Run()
        {
            //PrintState(0);
            for (int i = 0; i < 1000; i++)
            {
                DoTimeStep();
                //PrintState(i+1);
            }

            var total = 0L;
            foreach (var moon in _moons)
            {
                total += moon.position.Manhattan() * moon.velocity.Manhattan();
            }
            WriteLine(total);
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

        private void DoTimeStep()
        {
            foreach (var moon1 in _moons)
            foreach (var moon2 in _moons)
            {
                if (moon1.id >= moon2.id) continue;
                if (moon1.position.X != moon2.position.X)
                {
                    var change = moon1.position.X > moon2.position.X ? -1 : 1;
                    moon1.velocity.X += change;
                    moon2.velocity.X -= change;
                }
                if (moon1.position.Y != moon2.position.Y)
                {
                    var change = moon1.position.Y > moon2.position.Y ? -1 : 1;
                    moon1.velocity.Y += change;
                    moon2.velocity.Y -= change;
                }
                if (moon1.position.Z != moon2.position.Z)
                {
                    var change = moon1.position.Z > moon2.position.Z ? -1 : 1;
                    moon1.velocity.Z += change;
                    moon2.velocity.Z -= change;
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