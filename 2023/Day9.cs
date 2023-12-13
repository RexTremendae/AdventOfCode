using static ColorWriter;

public class Day9
{
    public async Task Part1()
    {
        var sum = 0L;
        await foreach (var sq in ReadDataFromFile("Day9.txt"))
        {
            sum += Extrapolate(sq);
        }

        WriteLine(sum);
    }

    public async Task Part2()
    {
        var sum = 0L;
        await foreach (var sq in ReadDataFromFile("Day9.txt"))
        {
            sum += Extrapolate(sq, false);
        }

        WriteLine(sum);
    }

    private int Extrapolate(int[] sq, bool forward = true)
    {
        var stack = new List<int[]>(new[] { sq });
        while (sq.Any(_ => _ != 0))
        {
            var nextSq = new int[sq.Length-1];
            for (int i = 1; i < sq.Length; i++)
            {
                nextSq[i-1] = sq[i] - sq[i-1];
            }
            stack.Add(nextSq);
            sq = nextSq;
        }

        var next = 0;
        for (int i = stack.Count-1; i >= 0; i--)
        {
            next = forward
                ? stack[i].Last() + next
                : stack[i].First() - next;
        }

        return next;
    }

    private async IAsyncEnumerable<int[]> ReadDataFromFile(string filepath)
    {
        var splitOpts = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
        foreach (var line in await File.ReadAllLinesAsync(filepath))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            yield return line.Trim().Split(' ', splitOpts).Select(int.Parse).ToArray();
        }
    }

    private IEnumerable<int[]> ReadDataFromContent(string content)
    {
        var splitOpts = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
        var split = content.Replace("\r", "").Split('\n', splitOpts).Select(_ => _.Trim());

        foreach (var line in split)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            yield return line.Split(' ', splitOpts).Select(int.Parse).ToArray();
        }
    }
}