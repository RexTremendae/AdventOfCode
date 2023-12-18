using static ColorWriter;

public class Day18
{
    public async Task Part1()
    {
        var (map, min, max) = await DrawOutline("Day18.txt");

        var fillStart = (x: int.MinValue, y: int.MinValue);

        //PrintMap(map, min, max, fillStart);
        //WriteLine();

        foreach (var x in min.x.To(max.x+1))
        {
            if (fillStart.x == int.MinValue)
            {
                if (map.Contains((x, 0)) && !map.Contains((x, 1)))
                {
                    fillStart = (x, 1);
                    break;
                }
            }
        }

        Fill(map, min, max);

        // PrintMap(map, min, max, fillStart);

        var sum = 0L;
        foreach (var y in min.y.To(max.y+1))
        {
            foreach (var x in min.x.To(max.x+1))
            {
                if (map.Contains((x, y))) sum++;
            }
        }

        WriteLine(sum);
    }

    private void PrintMap(HashSet<(int x, int y)> map, (int x, int y) min, (int x, int y) max, (int x, int y) fillStart)
    {
        foreach (var y in min.y.To(max.y+1))
        {
            Write($"{y:000} ");
            foreach (var x in min.x.To(max.x+1))
            {
                if (fillStart.x == int.MinValue)
                {
                    if (map.Contains((x, y)) && !map.Contains((x, y+1)))
                    {
                        fillStart = (x, y+1);
                    }
                }
                Write((x, y) ==
                    fillStart ? 'X'
                    : (x, y) == (0, 0) ? 'O'
                    : map.Contains((x, y)) ? '#'
                    : '.');
            }
            WriteLine();
        }
    }

    private void Fill(HashSet<(int x, int y)> map, (int x, int y) min, (int x, int y) max)
    {
        foreach (var y in min.y.To(max.y+1))
        {
            var inside = false;
            var startUp = false;
            var x = min.x;

            while (x <= max.x)
            {
                if (map.Contains((x, y)))
                {
                    inside = true;
                    startUp = map.Contains((x, y-1));

                    while (map.Contains((x, y))) x++;
                    if ((startUp && map.Contains((x-1, y+1))) ||
                        (!startUp && map.Contains((x-1, y-1))))
                    {
                        while (!map.Contains((x, y)) && x <= max.x)
                        {
                            map.Add((x, y));
                            x++;
                        }
                    }
                }

                x++;
            }
        }
    }

    private void SlowFill(HashSet<(int x, int y)> map, (int x, int y) fillStart)
    {
        var q = new List<(int x, int y)>([fillStart]);

        while (q.Any())
        {
            var (x, y) = q.First();
            q.RemoveAt(0);
            if (map.Contains((x, y))) continue;
            map.Add((x, y));

            foreach (var (nx, ny) in new (int, int)[] { (x+1, y), (x-1, y), (x, y+1), (x, y-1) })
            {
                if (map.Contains((nx, ny))) continue;
                q.Add((nx, ny));
            }
        }
    }

    private async Task<(HashSet<(int x, int y)> map, (int x, int y) min, (int x, int y) max)> DrawOutline(string filepath)
    {
        var pos = (x:0, y:0);
        var map = new HashSet<(int x, int y)>();
        map.Add(pos);
        var min = (x: 0, y: 0);
        var max = (x: 0, y: 0);

        await foreach (var (dir, steps, _) in ReadData(filepath))
        {
            for (int i = 0; i < steps; i++)
            {
                pos = dir switch
                {
                    'U' => (pos.x, pos.y-1),
                    'D' => (pos.x, pos.y+1),
                    'L' => (pos.x-1, pos.y),
                    'R' => (pos.x+1, pos.y),
                    _ => throw new InvalidOperationException()
                };
                map.Add(pos);
            }

            min = (int.Min(min.x, pos.x), int.Min(min.y, pos.y));
            max = (int.Max(max.x, pos.x), int.Max(max.y, pos.y));
        }

        return (map, min, max);
    }

    public async IAsyncEnumerable<(char dir, int steps, string metadata)> ReadData(string filepath)
    {
        foreach (var line in await File.ReadAllLinesAsync(filepath))
        {
            if (string.IsNullOrEmpty(line)) continue;
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            yield return (parts[0][0], int.Parse(parts[1]), parts[2][2..^1]);
        }
    }
}
