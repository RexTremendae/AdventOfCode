using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System;

using static System.Math;
using static System.Console;

public class Nanobot
{
    public Nanobot(int id, long x, long y, long z, long r)
    {
        Id = id;
        X = x;
        Y = y;
        Z = z;
        R = r;
    }

    public int Id { get; }
    public long X { get; }
    public long Y { get; }
    public long Z { get; }
    public long R { get; }

    public override bool Equals(object obj)
    {
        if (!(obj is Nanobot other))
        {
            return false;
        }

        return other.Id == Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}

public record Point(long X, long Y, long Z);

public class BoundingBox
{
    public Point Min { get; init; }
    public Point Max { get; init; }

    public Point Size => new Point(X: Max.X - Min.X, Y: Max.Y - Min.Y, Z: Max.Z - Min.Z);
}

public static class Extensions
{
    public static bool Intersects(this Nanobot _1, Nanobot _2)
    {
        var dist = new Point(_1.X, _1.Y, _1.Z).ManhattanDistanceTo(new Point(_2.X, _2.Y, _2.Z));
        return dist <= (_1.R + _2.R);
    }

    public static bool InRange(this Nanobot s, Point p)
    {
        var dist = p.ManhattanDistanceTo(new Point(s.X, s.Y, s.Z));
        return dist <= s.R;
    }

    public static long ManhattanDistanceTo(this Point _1, Point _2)
    {
        return (long)Math.Abs(_1.X - _2.X) + Math.Abs(_1.Y - _2.Y) + Math.Abs(_1.Z - _2.Z);
    }

    public static long SqrDistanceTo(this Point _1, Point _2)
    {
        var xdist = _1.X - _2.X;
        var ydist = _1.Y - _2.Y;
        var zdist = _1.Z - _2.Z;

        return xdist*xdist + ydist*ydist + zdist*zdist;
    }
}

public class MultiDictionary<TKey, TValue>
{
    private readonly Dictionary<TKey, HashSet<TValue>> _data;

    public MultiDictionary()
    {
        _data = new();
    }

    public void Add(TKey key, TValue value)
    {
        if (!_data.TryGetValue(key, out var list))
        {
            list = new HashSet<TValue>();
            _data.Add(key, list);
        }

        list.Add(value);
    }

    public IEnumerable<TValue> GetValues(TKey key)
    {
        if (!_data.TryGetValue(key, out var values))
        {
            return Enumerable.Empty<TValue>();
        }

        return values;
    }
}

public class Cluster
{
    private readonly HashSet<Nanobot> _members;
    private readonly HashSet<Nanobot> _unmembers;

    public Cluster(Nanobot s)
    {
        _members = new HashSet<Nanobot>();
        _unmembers = new HashSet<Nanobot>();
        _members.Add(s);
    }

    public bool Contains(Nanobot s)
    {
        return _members.Contains(s);
    }

    public bool TryAdd(Nanobot s)
    {
        if (_members.Contains(s)) return true;
        if (_unmembers.Contains(s)) return false;

        foreach (var m in _members)
        {
            if (!(m.Intersects(s)))
            {
                _unmembers.Add(s);
                return false;
            }
        }

        _members.Add(s);
        return true;
    }

    public IEnumerable<Nanobot> Members => _members;
}

public class Program
{
    public static void Main()
    {
        WriteLine();
        WriteLine();
        var start = DateTime.Now;
        new Program().Run();
        var duration = DateTime.Now - start;

        ForegroundColor = ConsoleColor.Magenta;
        WriteLine();
        Write($"{duration.Minutes:00}:{duration.Seconds:00}");
        System.Console.ResetColor();
        WriteLine($".{duration.Milliseconds:000}");
        WriteLine();
        WriteLine();
    }

    public void Run()
    {
        var cluster = FindLargestCluster(ReadData());
        var boundingBox = GetBoundingBox(cluster);

        var start = boundingBox.Min;
        var stop = boundingBox.Max;
        var (pos, count) = Search(cluster, start, stop, 10_000, x: true, y: true, z: true);
        WriteLine("Max count: " + count);

        start = pos with { X = pos.X - 10_000_000, Y = pos.Y, Z = pos.Z };
        stop = pos with { X = pos.X + 10_000_000, Y = pos.Y, Z = pos.Z };
        (pos, count) = Search(cluster, start, stop, 20_000, x: true, y: false, z: false);
        WriteLine("Max count: " + count);

        start = pos with { X = pos.X, Y = pos.Y - 10_000_000, Z = pos.Z };
        stop = pos with { X = pos.X, Y = pos.Y + 10_000_000, Z = pos.Z };
        (pos, count) = Search(cluster, start, stop, 20_000, x: false, y: true, z: false);
        WriteLine("Max count: " + count);

        start = pos with { X = pos.X, Y = pos.Y, Z = pos.Z - 10_000_000 };
        stop = pos with { X = pos.X, Y = pos.Y, Z = pos.Z + 10_000_000 };
        (pos, count) = Search(cluster, start, stop, 20_000, x: false, y: false, z: true);
        WriteLine("Max count: " + count);
    }

    public (Point, int) Search(List<Nanobot> cluster, Point start, Point end, long steps, bool x, bool y, bool z, bool debug = false)
    {
        var lastCount = 0;
        var pos = start;

        var maxCount = 0;
        var maxPos = new Point(0, 0, 0);

        var step = new Point
        (
            X: x ? ((end.X - start.X) / steps) : 0,
            Y: y ? ((end.Y - start.Y) / steps) : 0,
            Z: z ? ((end.Z - start.Z) / steps) : 0
        );

        for (int s = 0; s < steps; s ++)
        {
            pos = pos with { X = pos.X + step.X, Y = pos.Y + step.Y, Z = pos.Z + step.Z };
            var count = CountInRange(cluster, pos);

            if (count > maxCount)
            {
                maxCount = count;
                maxPos = pos;
            }

            if (count != lastCount)
            {
                if (debug)
                {
                    Write($"s: {s}   {count}  ");
                    if (count > lastCount)
                    {
                        ForegroundColor = ConsoleColor.Green;
                        WriteLine('↑');
                    }
                    else
                    {
                        ForegroundColor = ConsoleColor.Red;
                        WriteLine('↓');
                    }
                    ResetColor();
                }
                lastCount = count;
            }
        }

        return (maxPos, maxCount);
    }

        //WriteLine($"Min Pos: ({minX:##0_000_000}, {minY:##0_000_000}, {minZ:##0_000_000})");
        //WriteLine($"BB Size: ({maxX-minX:##0_000_000}, {maxY-minY:##0_000_000}, {maxZ-minZ:##0_000_000})");
/*
        var pos = new Point((maxX + minX)/2, (maxY + minY)/2, (maxZ + minZ)/2);

        var inRangeCount = CountInRange(cluster, pos);
        if (inRangeCount < cluster.Count)
        {
            WriteLine("Bounding box center is not in range of all nanobots - need another starting position!");
            WriteLine($"({pos.X:##0_000_000}, {pos.Y:##0_000_000}, {pos.Z:##0_000_000}) - {inRangeCount}");
            return;
        }

        int stepX = 1_000_000;
        int stepY = 1_000_000;
        int stepZ = 1_000_000;
        int dimension = 0;

        Point last;

        long lastOutput = (pos.X + pos.Y + pos.Z) * 2 + 1;

        while (stepX > 0 && stepY > 0 && stepZ > 0)
        {
            if (pos.X + pos.Y + pos.Z < lastOutput / 2)
            {
                lastOutput = pos.X + pos.Y + pos.Z;
                WriteLine($"{lastOutput:##0_000_000}");
            }
            last = pos;
            switch (dimension)
            {
                case 0:
                    if (stepX == 0 || stepX > pos.X) break;
                    pos = pos with { X = pos.X - stepX };
                    break;

                case 1:
                    if (stepY == 0 || stepY > pos.Y) break;
                    pos = pos with { Y = pos.Y - stepY };
                    break;

                case 2:
                    if (stepZ == 0 || stepZ > pos.Z) break;
                    pos = pos with { Z = pos.Z - stepZ };
                    break;
            }

            if (CountInRange(cluster, pos) != inRangeCount)
            {
                pos = last;

                switch (dimension)
                {
                    case 0:
                        stepX /= 2;
                        break;

                    case 1:
                        stepY /= 2;
                        break;

                    case 2:
                        stepZ /= 2;
                        break;
                }
            }

            dimension++;
            if (dimension == 3) dimension = 0;
        }

        WriteLine();

        // 101_293_956 - too low
        WriteLine($"({pos.X:##0_000_000}, {pos.Y:##0_000_000}, {pos.Z:##0_000_000}) - {pos.X + pos.Y + pos.Z:##0_000_000}");
        WriteLine(CountInRange(cluster, pos));
*/

    public BoundingBox GetBoundingBox(List<Nanobot> cluster)
    {
        long minX = long.MaxValue;
        long minY = long.MaxValue;
        long minZ = long.MaxValue;

        long maxX = 0;
        long maxY = 0;
        long maxZ = 0;

        foreach (var m in cluster)
        {
            if (minX > m.X) minX = m.X;
            if (minY > m.Y) minY = m.Y;
            if (minZ > m.Z) minZ = m.Z;

            if (maxX < m.X) maxX = m.X;
            if (maxY < m.Y) maxY = m.Y;
            if (maxZ < m.Z) maxZ = m.Z;
        }

        return new BoundingBox { Min = new Point(minX, minY, minZ), Max = new Point(maxX, maxY, maxZ) };
    }

    public int CountInRange(List<Nanobot> cluster, Point pos)
    {
        int inRangeCount = 0;
        foreach (var m in cluster)
        {
            if (m.InRange(pos)) inRangeCount ++;
        }
        return inRangeCount;
    }

    public List<Nanobot> FindLargestCluster(List<Nanobot> nanobots)
    {
        var clusters = new List<Cluster>();
        var clusterIndex = new MultiDictionary<int, Cluster>();

        for (int i = 0; i < nanobots.Count; i++)
        {
            bool memberOfAnyCluster = false;
            var s = nanobots[i];
            foreach (var c in clusters)
            {
                if (c.TryAdd(s))
                {
                    clusterIndex.Add(s.Id, c);
                    memberOfAnyCluster = true;
                }
            }

            if (!memberOfAnyCluster)
            {
                var c = new Cluster(s);
                clusters.Add(c);
                clusterIndex.Add(s.Id, c);

                for (int j = 0; j < i; j++)
                {
                    var di = nanobots[j];
                    if (c.TryAdd(di))
                    {
                        clusterIndex.Add(di.Id, c);
                    }
                }
            }
        }

        var orderedClusters = clusters.OrderByDescending(c => c.Members.Count());
/*
        foreach (var c in orderedClusters)
        {
            WriteLine(c.Members.Count());
        }
*/
        return orderedClusters.First().Members.ToList();
    }

    public static List<Nanobot> ReadData()
    {
        var regex = new Regex(@"pos=<(?<x>-?\d+),(?<y>-?\d+),(?<z>-?\d+)>, r=(?<r>\d+)");
        var data = new List<Nanobot>();

        int id = 0;
        foreach (var line in File.ReadAllLines("Day23.txt"))
        {
            if (string.IsNullOrWhiteSpace(line)) break;

            var match = regex.Match(line);
            if (!match.Success) continue;

            var x = long.Parse(match.Groups["x"].Value);
            var y = long.Parse(match.Groups["y"].Value);
            var z = long.Parse(match.Groups["z"].Value);
            var r = long.Parse(match.Groups["r"].Value);

            data.Add(new Nanobot(id: id, x: x, y: y, z: z, r: r));
            id++;
        }

        return data;
    }
}
