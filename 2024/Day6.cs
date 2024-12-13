using static System.Console;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;

public class Day6
{
    public async Task Part1()
    {
        var lines = (await File.ReadAllLinesAsync("Day6.txt"))
            .Where(_ => !string.IsNullOrWhiteSpace(_))
            .ToArray();

        var xStart = -1;
        var yStart = -1;

        for (int rowY = 0; rowY < lines.Length; rowY++)
        {
            for (int rowX = 0; rowX < lines[rowY].Length; rowX++)
            {
                if (lines[rowY][rowX] == '^')
                {
                    xStart = rowX;
                    yStart = rowY;
                    break;
                }
            }

            if ((xStart, yStart) != (-1, -1)) break;
        }

        var x = xStart;
        var y = yStart;
        var dirIdx = 0;

        var directions = ((int x, int y)[])[(0, -1), (1, 0), (0, 1), (-1, 0)];
        var positions = new HashSet<(int, int)>();

        while (true)
        {
            positions.Add((x, y));

            var movement = directions[dirIdx];
            var next = (x: x+movement.x, y: y+movement.y);

            if (next.x < 0 || next.y < 0 || next.x >= lines[0].Length || next.y >= lines.Length)
                break;

            if (lines[next.y][next.x] == '^' && movement == (0, -1))
                break;

            if ("^.".Contains(lines[next.y][next.x]))
            {
                (x, y) = next;
                continue;
            }

            if (lines[next.y][next.x] == '#')
            {
                dirIdx++;
                if (dirIdx >= 4) dirIdx = 0;

                continue;
            }

            throw new InvalidDataException();
        }

        WriteLine(positions.Count);
    }

    public async Task Part2()
    {
        var lines = (await File.ReadAllLinesAsync("Day6.txt"))
            .Where(_ => !string.IsNullOrWhiteSpace(_))
            .ToArray();

        var xStart = -1;
        var yStart = -1;

        var data = new char[lines.Length][];

        for (int rowY = 0; rowY < lines.Length; rowY++)
        {
            data[rowY] = new char[lines[0].Length];
            for (int rowX = 0; rowX < lines[rowY].Length; rowX++)
            {
                var chr = lines[rowY][rowX];
                if (chr == '^')
                {
                    xStart = rowX;
                    yStart = rowY;
                    chr = '.';
                }

                data[rowY][rowX] = chr;
            }
        }

        var x = xStart;
        var y = yStart;
        var positions = new HashSet<(int, int)>();

        Solve(data, positions, x, y);

        var count = 0;
        
        foreach (var (xx, yy) in positions)
        {
            if ((xx, yy) == (xStart, yStart))
            {
                continue;
            }

            var ppos = new HashSet<(int, int)>();
            var ddata = data.Select(_ => _.ToArray()).ToArray();

            ddata[yy][xx] = '#';

            var isLoop = Solve(ddata, ppos, xStart, yStart);
            if (isLoop) count++;
        }

        WriteLine(count);
    }

    private bool Solve(char[][] data, HashSet<(int, int)> positions, int x, int y)
    {
        var dirIdx = 0;

        var directions = ((int x, int y)[])[(0, -1), (1, 0), (0, 1), (-1, 0)];

        var visited = new HashSet<((int, int) pos, (int, int) dir)>();

        while (true)
        {
            positions.Add((x, y));

            var movement = directions[dirIdx];
            var next = (x: x+movement.x, y: y+movement.y);

            if (visited.Contains(((x, y), movement))) return true;

            visited.Add(((x, y), movement));

            if (next.x < 0 || next.y < 0 || next.x >= data[0].Length || next.y >= data.Length)
                return false;

            if (data[next.y][next.x] == '.')
            {
                (x, y) = next;
                continue;
            }

            if (data[next.y][next.x] == '#')
            {
                dirIdx++;
                if (dirIdx >= 4) dirIdx = 0;

                continue;
            }

            throw new InvalidDataException($"(x, y): ({x}, {y}) [{data[y][x]}], next: ({next.x}, {next.y}) [{data[next.y][next.x]}]");
        }
    }
}
