using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace Day12
{
    public class Program
    {
/*
        // First example - answer: 2772
        private (int id, Point3D position, Point3D velocity)[] _moons = new (int, Point3D, Point3D)[] {
            (0, (x: -1, y:  0, z:  2), (x: 0, y: 0, z: 0)),
            (1, (x:  2, y:-10, z: -7), (x: 0, y: 0, z: 0)),
            (2, (x:  4, y: -8, z:  8), (x: 0, y: 0, z: 0)),
            (3, (x:  3, y:  5, z: -1), (x: 0, y: 0, z: 0))
        };

        // Second example
        private (int id, Point3D position, Point3D velocity)[] _moons = new (int, Point3D, Point3D)[] {
            (0, (x: -8, y:-10, z:  0), (x: 0, y: 0, z: 0)),
            (1, (x:  5, y:  5, z: 10), (x: 0, y: 0, z: 0)),
            (2, (x:  2, y: -7, z:  3), (x: 0, y: 0, z: 0)),
            (3, (x:  9, y: -8, z: -3), (x: 0, y: 0, z: 0))
        };
*/
        // Puzzle input
        private (int id, Point3D position, Point3D velocity)[] _moons = new (int, Point3D, Point3D)[] {
            (0, (x:  6, y: 10, z: 10), (x: 0, y: 0, z: 0)),
            (1, (x: -9, y:  3, z: 17), (x: 0, y: 0, z: 0)),
            (2, (x:  9, y: -4, z: 14), (x: 0, y: 0, z: 0)),
            (3, (x:  4, y: 14, z:  4), (x: 0, y: 0, z: 0))
        };

        private HashSet<(long, long, long, long, long, long, long, long)> _visitedX = new();
        private HashSet<(long, long, long, long, long, long, long, long)> _visitedY = new();
        private HashSet<(long, long, long, long, long, long, long, long)> _visitedZ = new();

        public static void Main()
        {
            new Program().Run();
        }

        public void Run()
        {
            var steps = 0L;
            bool? _x = null, _y = null, _z = null;
            bool? _xp, _yp, _zp;

            for(;;)
            {
                _xp = _x;
                _yp = _y;
                _zp = _z;

                var (xUpdated, yUpdated, zUpdated) = AddVisited();
                (_x, _y, _z) = AddVisited();
                if ((xUpdated, yUpdated, zUpdated) == (false, false, false))
                {
                    break;
                }

                if (_xp == false && _x == true) WriteLine("X");
                if (_yp == false && _y == true) WriteLine("Y");
                if (_zp == false && _z == true) WriteLine("Z");

                DoTimeStep();
                steps++;
            }

            long xCycle = _visitedX.Count;
            long yCycle = _visitedY.Count;
            long zCycle = _visitedZ.Count;

            var commonFactors = new Dictionary<long, long>();

            var xFactors = GetPrimeFactors(xCycle).ToArray();
            WriteLine($"X: {xCycle} factors: {string.Join(" ", xFactors)}");
            var yFactors = GetPrimeFactors(yCycle).ToArray();
            WriteLine($"Y: {yCycle} factors: {string.Join(" ", yFactors)}");
            var zFactors = GetPrimeFactors(zCycle).ToArray();
            WriteLine($"Z: {zCycle} factors: {string.Join(" ", zFactors)}");

            AddCommonFactors(commonFactors, xFactors);
            AddCommonFactors(commonFactors, yFactors);
            AddCommonFactors(commonFactors, zFactors);

            var commonFactorsString = "";
            var commonFactorProduct = 1L;
            foreach (var key in commonFactors.Keys.OrderBy(x => x))
            {
                for (var i = 0; i < commonFactors[key]; i++)
                {
                    commonFactorsString += key.ToString() + " ";
                    commonFactorProduct *= key;
                }
            }
            WriteLine($"Common factors: {commonFactorsString}");

            var xSum = xCycle;
            var ySum = yCycle;
            var zSum = zCycle;

            WriteLine($"xCycle*yCycle*zCycle = {xCycle*yCycle*zCycle:N0}");
            WriteLine($"{commonFactorProduct:N0} ({commonFactorProduct})");
        }

        private void AddCommonFactors(Dictionary<long, long> commonFactors, long[] factors)
        {
            var last = 0L;
            var factorCount = 0;

            foreach (var factor in factors.OrderBy(x => x))
            {
                if (last != factor)
                {
                    if (last != 0)
                    {
                        if (commonFactors.TryGetValue(last, out var count))
                        {
                            if (factorCount > count)
                            {
                                commonFactors[last] = factorCount;
                            }
                        }
                        else
                        {
                            commonFactors.Add(last, factorCount);
                        }
                    }
                    last = factor;
                    factorCount = 1;
                }
                else
                {
                    factorCount++;
                }
            }

            if (last != 0)
            {
                if (commonFactors.TryGetValue(last, out var count))
                {
                    if (factorCount > count)
                    {
                        commonFactors[last] = factorCount;
                    }
                }
                else
                {
                    commonFactors.Add(last, factorCount);
                }
            }
        }

        private IEnumerable<long> GetPrimeFactors(long value)
        {
            var factor = 2;
            var primeFactors = new List<long>();

            while (value > 1)
            {
                if (value % factor == 0)
                {
                    primeFactors.Add(factor);
                    value /= factor;
                }
                else
                {
                    if (factor == 2) factor++;
                    else factor += 2;
                }
            }
            return primeFactors;
        }

        private (bool, bool, bool) AddVisited()
        {
            var xUpdated = _visitedX.Add((_moons[0].position.X, _moons[1].position.X, _moons[2].position.X, _moons[3].position.X,
                                          _moons[0].velocity.X, _moons[1].velocity.X, _moons[2].velocity.X, _moons[3].velocity.X));

            var yUpdated = _visitedY.Add((_moons[0].position.Y, _moons[1].position.Y, _moons[2].position.Y, _moons[3].position.Y,
                                          _moons[0].velocity.Y, _moons[1].velocity.Y, _moons[2].velocity.Y, _moons[3].velocity.Y));

            var zUpdated = _visitedZ.Add((_moons[0].position.Z, _moons[1].position.Z, _moons[2].position.Z, _moons[3].position.Z,
                                          _moons[0].velocity.Z, _moons[1].velocity.Z, _moons[2].velocity.Z, _moons[3].velocity.Z));

            return (xUpdated, yUpdated, zUpdated);
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

        public override bool Equals(object? other)
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