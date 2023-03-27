using static System.Console;

public class Day8
{
    public async Task Part1()
    {
        WriteLine("Reading indata...");
        WriteLine();

        var data = new List<string>();
        await foreach(var line in System.IO.File.ReadLinesAsync("Day8.txt"))
        {
            if (string.IsNullOrEmpty(line)) continue;
            data.Add(line);
        }

        var width = data[0].Length;
        var height = data.Count;

        var visibilityMap = new bool[height][];
        for (int y = 0; y < height; y++)
        {
            visibilityMap[y] = new bool[width];
            visibilityMap[y][0] = true;
            visibilityMap[y][width-1] = true;
        }

        for (int x = 0; x < width; x++)
        {
            visibilityMap[0][x] = true;
            visibilityMap[height-1][x] = true;
        }

        char max;

        for (int y = 1; y < height-1; y++)
        {
            var row = data[y];

            max = row.First();
            for (int x = 1; x < width-1; x++)
            {
                if (row[x] > max)
                {
                    max = row[x];
                    visibilityMap[y][x] = true;
                }
                if (max == '9') break;
            }

            max = row.Last();
            for (int x = width-2; x > 0; x--)
            {
                if (row[x] > max)
                {
                    max = row[x];
                    visibilityMap[y][x] = true;
                }
                if (max == '9') break;
            }
        }

        for (int x = 1; x < width-1; x++)
        {
            max = data[0][x];
            for (int y = 1; y < height-1; y++)
            {
                if (data[y][x] > max)
                {
                    max = data[y][x];
                    visibilityMap[y][x] = true;
                }
                if (max == '9') break;
            }

            max = data[height-1][x];
            for (int y = height-2; y > 0; y--)
            {
                if (data[y][x] > max)
                {
                    max = data[y][x];
                    visibilityMap[y][x] = true;
                }
                if (max == '9') break;
            }
        }

        PrintTrees(data, visibilityMap);

        var sum = visibilityMap.Sum(_ => _.Sum(_ => _ ? 1: 0));

        WriteLine();
        WriteLine(sum);
        WriteLine();
    }

    public async Task Part2()
    {
        WriteLine("Reading indata...");
        WriteLine();

        var data = new List<string>();
        await foreach(var line in System.IO.File.ReadLinesAsync("Day8.txt"))
        {
            if (string.IsNullOrEmpty(line)) continue;
            data.Add(line);
        }

        var width = data[0].Length;
        var height = data.Count;

        var scenicScores = new long[height][];
        for (int y = 0; y < height; y++)
        {
            scenicScores[y] = new long[width];
        }

        for (int x = 1; x < width-1; x++)
        for (int y = 1; y < height-1; y++)
        {
            var total = 1;
            var current = data[y][x];
            foreach (var (dx, dy) in new[] { (1, 0), (-1, 0), (0, 1), (0, -1) })
            {
                var xx = x;
                var yy = y;
                var score = 0;
                for(;;)
                {
                    xx += dx;
                    yy += dy;
                    if (xx < 0 || yy < 0 || xx >= width || yy >= height)
                        break;

                    score++;

                    if (data[yy][xx] >= current) break;
                }
                total *= score;
            }

            scenicScores[y][x] = total;
        }

        var sum = scenicScores.Max(_ => _.Max());

        WriteLine();
        WriteLine(sum);
        WriteLine();
    }

    private void PrintTrees(List<string> data, bool[][] visibilityMap)
    {
        for (int y = 0; y < data.Count; y++)
        {
            for (int x = 0; x < data[y].Length; x++)
            {
                ForegroundColor = visibilityMap[y][x]
                    ? ConsoleColor.White
                    : ConsoleColor.DarkGray;
                Write(data[y][x]);
            }
            WriteLine();
        }
        ResetColor();
    }
}
