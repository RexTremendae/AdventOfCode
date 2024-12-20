using static System.Console;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;

public class Day8
{
    public async Task Part1()
    {
        var (antennas, width, height) = await LoadData("Day8.txt");

        var nodes = new HashSet<(int, int)>();

        foreach (var c in antennas.Keys)
        {
            for (int a = 0; a < antennas[c].Length-1; a++)
            {
                for (int b = a+1; b < antennas[c].Length; b++)
                {
                    var _1 = antennas[c][a];
                    var _2 = antennas[c][b];

                    var Δ = (x: _2.x - _1.x, y: _2.y - _1.y);
                    var _0 = (x: _1.x - Δ.x, y: _1.y - Δ.y);
                    var _3 = (x: _2.x + Δ.x, y: _2.y + Δ.y);

                    if (Validate(_0, width, height))
                        nodes.Add(_0);

                    if (Validate(_3, width, height))
                        nodes.Add(_3);
                }
            }
        }

        WriteLine(nodes.Count);
    }

    public async Task Part2()
    {
        var (antennas, width, height) = await LoadData("Day8.txt");

        var nodes = new HashSet<(int, int)>();

        foreach (var c in antennas.Keys)
        {
            for (int a = 0; a < antennas[c].Length-1; a++)
            {
                for (int b = a+1; b < antennas[c].Length; b++)
                {
                    var _1 = antennas[c][a];
                    var _2 = antennas[c][b];

                    var Δ = (x: _2.x - _1.x, y: _2.y - _1.y);
                    var _0 = _1;
                    var _3 = _2;
                    
                    while (Validate(_0, width, height))
                    {
                        nodes.Add(_0);
                        _0 = (x: _0.x - Δ.x, y: _0.y - Δ.y);
                    }

                    while (Validate(_3, width, height))
                    {
                        nodes.Add(_3);
                        _3 = (x: _3.x + Δ.x, y: _3.y + Δ.y);
                    }
                }
            }
        }

        WriteLine(nodes.Count);
    }

    private async Task<(Dictionary<char, (int x, int y)[]> antennas, int width, int height)> LoadData(string filename)
    {
        var width = 0;
        int x = 0, y = 0;

        var antennas = new Dictionary<char, List<(int x, int y)>>();

        foreach (var line in await File.ReadAllLinesAsync("Day8.txt"))
        {
            if (string.IsNullOrWhiteSpace(line)) break;
            if (width == 0) width = line.Length;

            for (x = 0; x < line.Length; x++)
            {
                var chr = line[x];

                if (chr == '.') continue;
 
                if (!antennas.TryGetValue(chr, out var positions))
                {
                    positions = [];
                    antennas.Add(chr, positions);
                }

                positions.Add((x, y));
            }

            y++;
        }

        var height = y;

        return (antennas.ToDictionary(_ => _.Key, _ => _.Value.ToArray()), width, height);
    }

    private bool Validate((int x, int y) point, int width, int height)
    {
        return point.x >= 0 && point.y >= 0 && point.x < width && point.y < height;
    }
}
