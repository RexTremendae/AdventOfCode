using System.Numerics;
using static ColorWriter;

public class Day11
{
    public async Task Part1()
    {
        var lines = await ReadData("Day11.txt").ToListAsync();

        var inverted = Invert(lines.Select(_ => _.ToCharArray()).ToList());
        Expand(inverted);
        var reverted = Invert(inverted);
        Expand(reverted);

        var galaxies = new HashSet<(int x, int y)>();

        int x = 0, y = 0;
        foreach (var line in reverted)
        {
            x = 0;
            foreach (var chr in line)
            {
                if (chr == '#')
                {
                    galaxies.Add((x, y));
                }
                x++;
            }
            y++;
        }

        var visited = new HashSet<(int x, int y)>();

        var sum = 0L;

        foreach (var (x1, y1) in galaxies)
        {
            visited.Add((x1, y1));
            foreach (var (x2, y2) in galaxies)
            {
                if (visited.Contains((x2, y2))) continue;
                sum += int.Abs(x1 - x2) + int.Abs(y1 - y2);
            }
        }

        WriteLine(sum);
    }

    public async Task Part2()
    {
        var expansionRate = 1_000_000;
        //var expansionRate = 100;
        var lines = await ReadData("Day11.txt").ToListAsync();

        var inverted = Invert(lines.Select(_ => _.ToCharArray()).ToList());
        var expandedXSet = GetExpansions(inverted).ToHashSet();
        var reverted = Invert(inverted);
        var expandedYSet = GetExpansions(reverted).ToHashSet();

        var galaxies = new HashSet<(BigInteger x, BigInteger y)>();

        int x = 0, y = 0;
        BigInteger expandedX = 0, expandedY = 0;
        foreach (var line in reverted)
        {
            x = 0;
            expandedX = 0;
            foreach (var chr in line)
            {
                if (chr == '#')
                {
                    galaxies.Add((expandedX, expandedY));
                }

                expandedX += expandedXSet.Contains(x) ? expansionRate : 1;
                x++;
            }
            expandedY += expandedYSet.Contains(y) ? expansionRate : 1;
            y++;
        }

        var visited = new HashSet<(BigInteger x, BigInteger y)>();

        BigInteger sum = 0;

        foreach (var (x1, y1) in galaxies)
        {
            visited.Add((x1, y1));
            foreach (var (x2, y2) in galaxies)
            {
                if (visited.Contains((x2, y2))) continue;
                sum += BigInteger.Abs(x1 - x2) + BigInteger.Abs(y1 - y2);
            }
        }

        WriteLine(sum);
    }

    private List<char[]> Invert(List<char[]> lines)
    {
        var inverted = new List<char[]>();
        for (var y = 0; y < lines.Count; y++)
        {
            var line = lines[y];
            for (int x = 0; x < line.Length; x++)
            {
                if (inverted.Count <= x) inverted.Add(new char[lines.Count]);
                inverted[x][y] = line[x];
            }
        }

        return inverted;
    }

    private void Expand(List<char[]> lines)
    {
        // Expand
        for (var y = 0; y < lines.Count; y++)
        {
            if (lines[y].All(_ => _ == '.'))
            {
                lines.Insert(y, "".PadLeft(lines[y].Length-1, '.').ToCharArray());
                y++;
            }
        }
    }

    private IEnumerable<int> GetExpansions(List<char[]> lines)
    {
        // Expand
        for (var y = 0; y < lines.Count; y++)
        {
            if (lines[y].All(_ => _ == '.'))
            {
                yield return y;
            }
        }
    }

    private async IAsyncEnumerable<string> ReadData(string filepath)
    {
        var y = 0;

        foreach (var line in await File.ReadAllLinesAsync(filepath))
        {
            if (string.IsNullOrWhiteSpace(line)) break;
            yield return line.Trim();

            y++;
        }
    }
}
