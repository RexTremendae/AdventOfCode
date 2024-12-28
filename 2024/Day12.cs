using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Console;

public class Day12
{
    public async Task Part1()
    {
        var map = await ReadDataAsync("Day12.txt");

        var sum = 0L;
        for(;;)
        {
            var (id, area, perimeter) = FindRegion(map);
            if (area <= 0)
            {
                break;
            }

            WriteLine($"[{id}] {area} x {perimeter}");
            sum += area*perimeter;
        }

        WriteLine(sum);
    }

    private (char id, int area, int perimeter) FindRegion(char[][] map)
    {
        var (initialX, initialY) = FindFirstRegionPosition(map);
        if (initialX < 0 || initialY < 0) return ('!', 0, 0);
        var perimeter = 0;

        var q = new Queue<(int x, int y)>();
        q.Enqueue((initialX, initialY));
        var id = map[initialY][initialX];

        var visitedArea = new HashSet<(int x, int y)>();

        while (q.Any())
        {
            var (x, y) = q.Dequeue();

            if (map[y][x] == '#') continue;

            if (map[y][x] == id)
            {
                map[y][x] = '#';
                visitedArea.Add((x, y));

                foreach (var (nx, ny) in ((int, int)[])[(x+1, y), (x-1, y), (x, y+1), (x, y-1)])
                {
                    if (nx < 0 || ny < 0 || nx >= map[0].Length || ny >= map.Length)
                    {
                        perimeter++;
                        continue;
                    }

                    q.Enqueue((nx, ny));
                }
            }
            else
            {
                perimeter++;
            }
        }

        foreach (var (vx, vy) in visitedArea)
        {
            map[vy][vx] = '.';
        }

        return (id, visitedArea.Count, perimeter);
    }

    private (int x, int y) FindFirstRegionPosition(char[][] map)
    {
        for (var y = 0; y < map.Length; y++)
        for (var x = 0; x < map[0].Length; x++)
        {
            if (map[y][x] != '.')
            {
                return (x, y);
            }
        }

        return (-1, -1);
    }

    private async Task<char[][]> ReadDataAsync(string filename)
    {
        var map = new List<char[]>();
        var lines = await File.ReadAllLinesAsync(filename);

        foreach (var lne in lines)
        {
            if (string.IsNullOrWhiteSpace(lne)) break;
            map.Add(lne.ToCharArray());
        }

        return map.ToArray();
    }
}