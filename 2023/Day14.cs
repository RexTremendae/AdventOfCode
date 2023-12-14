using static ColorWriter;

public class Day14
{
    public async Task Part1()
    {
        var data = await ReadData("Day14.txt")
            .Select(_ => _.ToCharArray())
            .ToArrayAsync();
        Tilt(data);

        WriteLine(Sum(data));
    }

    public async Task Part2()
    {
        var cycleCount = 1_000_000_000;
        var data = await ReadData("Day14.txt")
            .Select(_ => _.ToCharArray())
            .ToArrayAsync();

        var history = new List<char[][]>();
        history.Add(Copy(data));
        var cycleStart = -1;
        var cycleLength = -1;

        var i = 0;
        while (cycleStart < 0)
        {
            Cycle(data);
            if (cycleStart < 0)
            {
                for (int j = 0; j < history.Count; j++)
                {
                    if (Equals(history[j], data))
                    {
                        cycleStart = j;
                        cycleLength = i-j+1;
                        WriteLine($"Cycle start at {cycleStart}, length {cycleLength}");
                        break;
                    }
                }
            }
            history.Add(Copy(data));

            i++;
        }

        data = history[cycleStart + ((cycleCount - cycleStart) % cycleLength)];

        var value = data.Length;
        var sum = 0L;
        for (int y = 0; y < data.Length; y++)
        {
            for (int x = 0; x < data[y].Length; x++)
            {
                var current = data[y][x];
                if (current == 'O') sum += value;
            }
            value--;
        }

        WriteLine(sum);
    }

    long Sum(char[][] data)
    {
        var value = data.Length;
        var sum = 0L;
        for (int y = 0; y < data.Length; y++)
        {
            for (int x = 0; x < data[y].Length; x++)
            {
                var current = data[y][x];
                if (current == 'O') sum += value;
            }
            value--;
        }

        return sum;
    }

    private bool Equals(char[][] data1, char[][] data2)
    {
        for (int y = 0; y < data1.Length; y++)
        for (int x = 0; x < data1[y].Length; x++)
            if (data1[y][x] != data2[y][x]) return false;

        return true;
    }

    private char[][] Copy(char[][] data)
    {
        return data.Select(_ => _.ToArray()).ToArray();
    }

    private void Cycle(char[][] data)
    {
        for (int i = 0; i < 4; i ++)
        {
            Tilt(data);
            Rotate(data);
        }
    }

    private void Rotate(char[][] data)
    {
        var copy = Copy(data);

        for (int y = 0; y < copy.Length; y++)
        for (int x = 0; x < copy[y].Length; x++)
        {
            data[y][x] = copy[copy.Length-x-1][y];
        }
    }

    private void Tilt(char[][] data)
    {
        for (int y = 0; y < data.Length; y++)
        for (int x = 0; x < data[y].Length; x++)
        {
            if (data[y][x] == 'O')
            {
                var yy = y-1;
                for(;;)
                {
                    if (yy < 0 || data[yy][x] != '.') break;
                    data[yy][x] = data[yy+1][x];
                    data[yy+1][x] = '.';
                    yy--;
                }
            }
        }
    }

    private async IAsyncEnumerable<string> ReadData(string filepath)
    {
        foreach (var line in await File.ReadAllLinesAsync(filepath))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            yield return line.Trim();
        }
    }
}