using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;
using static ColorWriter;

public class Day10
{
    public static readonly Dictionary<char, Direction[]> Pipes = new()
    {
        { '-', new[] { Direction.Left, Direction.Right } },
        { '|', new[] { Direction.Up, Direction.Down } },
        { 'L', new[] { Direction.Up, Direction.Right } },
        { 'J', new[] { Direction.Up, Direction.Left } },
        { '7', new[] { Direction.Left, Direction.Down } },
        { 'F', new[] { Direction.Right, Direction.Down } },
    };

    public async Task Part1()
    {
        var (map, start) = await ReadData(@"C:\dev\AdventOfCode\2023\Day10.txt");
        WriteLine(Solve(map, start));
    }

    private int Solve(string[] map, (int x, int y) start)
    {
        var longestPath = new (int, int)[] { };
        var visitedStart = new[] { start }.ToHashSet();
        var q = new List<((int x, int y) pos, HashSet<(int, int)> visited, (int, int)[] path, int len)>();

        foreach (var to in new (int x, int y)[] {
            (start.x+1, start.y), (start.x-1, start.y),
            (start.x, start.y+1), (start.x, start.y-1) })
        {
            if (CanMove(map, start, to, visitedStart))
            {
                q.Add((to, visitedStart.ToHashSet(), new[] { to }, 1));
            }
        }

        while (q.Any())
        {
            var (pos, visited, path, len) = q.First();
            q.RemoveAt(0);
            visited.Add(pos);
            if (len > longestPath.Length) longestPath = path;

            foreach (var to in new (int x, int y)[] {
                (pos.x+1, pos.y), (pos.x-1, pos.y),
                (pos.x, pos.y+1), (pos.x, pos.y-1) })
            {
                if (map[to.y][to.x] == 'S' && CanMove(map, pos, to))
                {
                    if (len > 1)
                    {
                        return len / 2 + 1;
                    }
                    else
                        continue;
                }

                if (CanMove(map, pos, to, visited))
                {
                    q.Add((to, visited.ToHashSet(), path.Concat(new[] {to}).ToArray(), len+1));
                }
            }
        }

        return -1;
    }

    private bool CanMove(string[] map, (int x, int y) from, (int x, int y) to)
    {
        return CanMove(map, from, to, new HashSet<(int x, int y)>());
    }

    private bool CanMove(string[] map, (int x, int y) from, (int x, int y) to, HashSet<(int x, int y)> visited)
    {
        var movement = $"[{from}] {map[from.y][from.x]} => [{to}] {map[to.y][to.x]}";

        if (visited.Contains(to) && map[to.y][to.x] != 'S') return false;

        var dir = (x: to.x - from.x, y: to.y - from.y);

        var toChar = map[to.y][to.x];
        var toPossible = ((dir switch
        {
            _ when dir == ( 1,  0) && "-J7S".Contains(toChar) => true,
            _ when dir == (-1,  0) && "-LFS".Contains(toChar) => true,
            _ when dir == ( 0, -1) && "|7FS".Contains(toChar) => true,
            _ when dir == ( 0,  1) && "|JLS".Contains(toChar) => true,
            _ => false
        }));

        var fromChar = map[from.y][from.x];
        var fromPossible = dir switch
        {
            _ when dir == ( 1,  0) && "-LFS".Contains(fromChar) => true,
            _ when dir == (-1,  0) && "-J7S".Contains(fromChar) => true,
            _ when dir == ( 0, -1) && "|JLS".Contains(fromChar) => true,
            _ when dir == ( 0,  1) && "|7FS".Contains(fromChar) => true,
            _ => false
        };

        var canMove = toPossible && fromPossible;

        return canMove;
    }

    private async Task<(string[] map, (int x, int y) start)> ReadData(string filepath)
    {
        var map = new List<string>();
        var y = 0;
        var start = (-1, -1);

        foreach (var line in await File.ReadAllLinesAsync(filepath))
        {
            if (string.IsNullOrWhiteSpace(line)) break;
            map.Add($".{line}.");
            if (start == (-1, -1))
            {
                var startX = line.IndexOf('S');
                if (startX >= 0)
                {
                    start = (startX + 1, y + 1);
                }
            }
            y++;
        }

        map.Insert(0, "".PadLeft(map.First().Length, '.'));
        map.Add("".PadLeft(map.First().Length, '.'));

        return (map.ToArray(), start);
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    };
}
