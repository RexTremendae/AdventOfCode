using static System.Console;

public class Day14
{
    public async Task Part1()
    {
        var data = await ReadInput("Day14.txt");

        var maxY = 0;

        var pieces = new HashSet<(int x, int y)>();

        foreach (var (x, y) in data)
        {
            maxY = Math.Max(maxY, y);

            pieces.Add((x, y));
        }

        var start = (x: 500, y: 0);
        var current = start;
        var count = 0;

        while(current.y <= maxY)
        {
            var (x, y) = (current.x, current.y+1);
            if (!pieces.Contains((x, y)))
            {
                current = (x, y);
                continue;
            }

            x = x-1;
            if (!pieces.Contains((x, y)))
            {
                current = (x, y);
                continue;
            }

            x += 2;
            if (!pieces.Contains((x, y)))
            {
                current = (x, y);
                continue;
            }

            pieces.Add(current);
            current = start;
            count++;
        }

        WriteLine(count);
    }

    public async Task Part2()
    {
        var data = await ReadInput("Day14.txt");

        var maxY = 0;

        var pieces = new HashSet<(int x, int y)>();

        foreach (var (x, y) in data)
        {
            maxY = Math.Max(maxY, y);

            pieces.Add((x, y));
        }

        var floorY = maxY + 2;
        var start = (x: 500, y: 0);
        var current = start;
        var count = 0;

        for(;;)
        {
            if (current.y+1 == floorY)
            {
                pieces.Add(current);
                count++;
                current = start;
                continue;
            }

            var (x, y) = (current.x, current.y+1);
            if (!pieces.Contains((x, y)))
            {
                current = (x, y);
                continue;
            }

            x = x-1;
            if (!pieces.Contains((x, y)))
            {
                current = (x, y);
                continue;
            }

            x += 2;
            if (!pieces.Contains((x, y)))
            {
                current = (x, y);
                continue;
            }

            pieces.Add(current);
            count++;
            if (current == start) break;

            current = start;
        }

        WriteLine(count);
    }

    public async Task<HashSet<(int x, int y)>> ReadInput(string filename)
    {
        var data = new HashSet<(int, int)>();
        await foreach(var line in System.IO.File.ReadLinesAsync(filename))
        {
            var from = (x: 0, y: 0);
            var to = (x: 0, y: 0);
            foreach (var rawCoord in $"> {line}".Split("-", StringSplitOptions.TrimEntries).Select(_ => _[2..]))
            {
                from = to;

                var parts = rawCoord.Split(",");
                var coord = (int.Parse(parts[0]), int.Parse(parts[1]));
                to = coord;

                if (from == (0, 0))
                {
                    from = coord;
                    continue;
                }

                if (from.x != to.x)
                {
                    if (from.y != to.y) throw new InvalidOperationException($"({from}) -> ({to})");
                    for (var x = Math.Min(from.x, to.x); x <= Math.Max(from.x, to.x); x++)
                    {
                        data.Add((x, from.y));
                    }
                }
                else
                {
                    if (from.x != to.x) throw new InvalidOperationException($"({from}) -> ({to})");
                    for (var y = Math.Min(from.y, to.y); y <= Math.Max(from.y, to.y); y++)
                    {
                        data.Add((from.x, y));
                    }
                }
            }
        }

        return data;
    }
}
