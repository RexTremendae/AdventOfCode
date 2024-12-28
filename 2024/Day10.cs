using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Console;

public class Day10
{
    public async Task Part1()
    {
        var map = await ReadDataAsync("Day10.txt");
        var sum = 0L;

        for (var y = 0; y < map.Length; y++)
        for (var x = 0; x < map[y].Length; x++)
        {
            var current = FindTrailScore(map, x, y);
            if (current.Count > 0)
            {
                sum += current.Count;
            }
        }

        WriteLine(sum);
    }

    private Dictionary<(int, int), int> FindTrailScore(int[][] map, int x, int y)
    {
        var result = new Dictionary<(int, int), int>();
        FindTrailScore(map, x, y, 0, result);
        return result;
    }

    private void FindTrailScore(int[][] map, int x, int y, int current, Dictionary<(int, int), int> result)
    {
        if (map[y][x] != current) return;
        if (map[y][x] == 9)
        {
            if (!result.TryGetValue((x, y), out var sum))
            {
                result.Add((x, y), 1);
            }

            result[(x, y)]++;
            return;
        }

        foreach (var (nx, ny) in ((int, int)[])[(x+1, y), (x-1, y), (x, y+1), (x, y-1)])
        {
            if (nx < 0 || ny < 0 || nx >= map[0].Length || ny >= map.Length) continue;
            FindTrailScore(map, nx, ny, current+1, result);
        }
    }

    public async Task<int[][]> ReadDataAsync(string filename)
    {
        return (await File.ReadAllLinesAsync(filename))
            .Select(_ => _.Trim())
            .Where(_ => !string.IsNullOrWhiteSpace(_))
            .Select(_ => _.ToCharArray().Select(_ => _ - '0').ToArray())
            .ToArray();
    }
}
