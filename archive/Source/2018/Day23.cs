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
    public static DateTime _start;

    public static void Main()
    {
        WriteLine();
        WriteLine();
        _start = DateTime.Now;
        new Program().Run();
        WriteLine();
        PrintDuration();
        WriteLine();
    }

    public static void PrintDuration()
    {
        var duration = DateTime.Now - _start;

        ForegroundColor = ConsoleColor.Magenta;
        Write($"{duration.Minutes:00}:{duration.Seconds:00}");
        System.Console.ResetColor();
        Write($".{duration.Milliseconds:000}");
    }

    public void Run()
    {
        var cluster = FindLargestCluster(ReadData());

        var maxDist = 0L;
        foreach (var bot in cluster)
        {
            var dist = bot.X + bot.Y + bot.Z - bot.R;
            maxDist = (long)Math.Max(dist, maxDist);
        }
        WriteLine(maxDist.ToString("N0"));
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

        for (int i = 0; i < nanobots.Count; i++)
        {
            bool memberOfAnyCluster = false;
            var s = nanobots[i];
            foreach (var c in clusters)
            {
                if (c.TryAdd(s))
                {
                    memberOfAnyCluster = true;
                }
            }

            if (!memberOfAnyCluster)
            {
                var c = new Cluster(s);
                clusters.Add(c);
            }
        }

        var orderedClusters = clusters.OrderByDescending(c => c.Members.Count());
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
