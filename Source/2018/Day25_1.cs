using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static System.Console;

namespace Day25
{
    public class Program
    {
        public static void Main()
        {
            new Program().Run();
        }

        public void Run()
        {
            var clusters = new List<Cluster>();
            bool first = true;

            var data = ParseInput("Day25.txt");
            WriteLine("Calculating...");

            foreach (var p4 in data)
            {
                if (first)
                {
                    clusters.Add(new Cluster());
                    clusters.First().Add(p4);
                }
                else
                {
                    var connected = new List<Cluster>();
                    foreach (var c in clusters)
                    {
                        if (c.ShouldConnect(p4))
                        {
                            connected.Add(c);
                            c.Add(p4);
                        }
                    }

                    if (connected.Count() > 1)
                    {
                        var merged = new Cluster();
                        clusters.Add(merged);

                        foreach (var c in connected)
                        {
                            merged.Merge(c);
                            clusters.Remove(c);
                        }
                    }

                    if (connected.None())
                    {
                        var newCluster = new Cluster();
                        newCluster.Add(p4);
                        clusters.Add(newCluster);
                    }
                }
                first = false;
            }

            WriteLine(clusters.Count);
        }

        public IEnumerable<Position4D> ParseInput(string filename)
        {
            foreach (var line in File.ReadAllLines(filename))
            {
                if (string.IsNullOrEmpty(line)) continue;
                var p4 = new Position4D();

                var coords = line.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
                for (int i = 0; i < 4; i ++)
                {
                    p4.Coordinates[i] = int.Parse(coords[i]);
                }

                yield return p4;
            }
        }
    }

    public class Cluster
    {
        private List<Position4D> _positions;
        private int[] _min;
        private int[] _max;

        public IEnumerable<Position4D> Positions => _positions;
        public int[] Min => _min;
        public int[] Max => _max;

        public Cluster()
        {
            _positions = new List<Position4D>();
            _min = new int[4];
            _max = new int[4];
        }

        public void Merge(Cluster other)
        {
            foreach (var p4 in other.Positions)
            {
                Add(p4);
            }
        }

        public bool ShouldConnect(Position4D p4)
        {
            int offset = 3;

            bool allMinInRange = true;
            bool allMaxInRange = true;

            for (int i = 0; i < 4; i++)
            {
                if (_min[i] - offset > p4.Coordinates[i]) allMinInRange = false;
                if (_max[i] + offset < p4.Coordinates[i]) allMinInRange = false;
            }

            // Outside bounding box
            if (!allMaxInRange || !allMinInRange) return false;

            foreach (var p in _positions)
            {
                // Found one position close enough, connect
                if (p4.Distance(p) <= offset) return true;
            }

            // No existing position was close enough
            return false;
        }

        public void Add(Position4D p4)
        {
            if (_positions.None())
            {
                for (int i = 0; i < 4; i ++)
                {
                    _min[i] = p4.Coordinates[i];
                    _max[i] = p4.Coordinates[i];
                }
            }
            else
            {
                for (int i = 0; i < 4; i ++)
                {
                    if (_min[i] > p4.Coordinates[i]) _min[i] = p4.Coordinates[i];
                    if (_max[i] < p4.Coordinates[i]) _max[i] = p4.Coordinates[i];
                }
            }

            _positions.Add(p4);
        }
    }

    public class Position4D
    {
        public Position4D()
        {
            Coordinates = new int[4];
        }

        public int[] Coordinates { get; set; }

        public int Distance(Position4D other)
        {
            int distance = 0;
            for (int i = 0; i < 4; i++)
            {
                distance += Math.Abs(Coordinates[i] - other.Coordinates[i]);
            }

            return distance;
        }
    }

    public static class Extensions
    {
        public static bool None<T>(this IEnumerable<T> source)
        {
            return !source.Any();
        }
    }
}