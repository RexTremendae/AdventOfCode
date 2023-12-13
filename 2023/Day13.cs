using static ColorWriter;

public class Day13
{
    public async Task Part1()
    {
        var sum = 0L;

        await foreach (var pattern in ReadData("Day13.txt"))
        {
            var mirror = FindHorizontalMirror(pattern);
            if (mirror >= 0)
            {
                sum += 100*mirror;
            }
            else
            {
                mirror = FindVerticalMirror(pattern);
                if (mirror < 0) throw new InvalidOperationException();
                sum += mirror;
            }
        }

        WriteLine(sum);
    }

    public async Task Part2()
    {
        var sum = 0L;

        await foreach (var pattern in ReadData("Day13.txt"))
        {
            var mirror = FindHorizontalSmudgeMirror(pattern);
            if (mirror >= 0)
            {
                sum += 100*mirror;
            }
            else
            {
                mirror = FindVerticalSmudgeMirror(pattern);
                if (mirror < 0)
                {
                    throw new InvalidOperationException();
                }
                sum += mirror;
            }
        }

        WriteLine(sum);
    }

    private int FindVerticalSmudgeMirror(string[] pattern)
    {
        var inverted = Invert(pattern);
        return FindHorizontalSmudgeMirror(inverted);
    }

    private int FindHorizontalSmudgeMirror(string[] pattern)
    {
        var line = pattern.First();
        for (var y = 1; y < pattern.Length; y++)
        {
            var smudgeFixed = false;
            var isEqual = line == pattern[y];
            var isSmudgeEqual = SmudgeEquals(line, pattern[y]);
            if (isSmudgeEqual) smudgeFixed = true;

            if (isEqual || isSmudgeEqual)
            {
                var y1 = y-2;
                var y2 = y+1;

                var isMirror = true;
                for(;;)
                {
                    if (y1 < 0 || y2 >= pattern.Length) break;

                    isEqual = pattern[y1] == pattern[y2];
                    isSmudgeEqual = smudgeFixed ? false : SmudgeEquals(pattern[y1], pattern[y2]);
                    if (isSmudgeEqual) smudgeFixed = true;

                    if (!isEqual && !isSmudgeEqual)
                    {
                        isMirror = false;
                        break;
                    }

                    y1--;
                    y2++;
                }

                if (isMirror && smudgeFixed)
                {
                    return y;
                }
            }

            line = pattern[y];
        }

        return -1;
    }

    private bool SmudgeEquals(string line1, string line2)
    {
        var hasDiff = false;
        for (int i = 0; i < line1.Length; i++)
        {
            if (line1[i] == line2[i]) continue;

            if (hasDiff) return false;
            hasDiff = true;
        }

        return hasDiff;
    }

    private string[] Invert(string[] lines)
    {
        var inverted = new List<char[]>();
        for (var y = 0; y < lines.Length; y++)
        {
            var line = lines[y];
            for (int x = 0; x < line.Length; x++)
            {
                if (inverted.Count <= x) inverted.Add(new char[lines.Length]);
                inverted[x][y] = line[x];
            }
        }

        return inverted.Select(_ => string.Join("", _)).ToArray();
    }

    private int FindVerticalMirror(string[] pattern)
    {
        var inverted = Invert(pattern);
        return FindHorizontalMirror(inverted);
    }

    private int FindHorizontalMirror(string[] pattern)
    {
        var line = pattern.First();
        for (int y = 1; y < pattern.Length; y++)
        {
            if (line == pattern[y])
            {
                var y1 = y-2;
                var y2 = y+1;

                var isMirror = true;
                for(;;)
                {
                    if (y1 < 0 || y2 >= pattern.Length) break;
                    if (pattern[y1] != pattern[y2])
                    {
                        isMirror = false;
                        break;
                    }

                    y1--;
                    y2++;
                }

                if (isMirror)
                {
                    return y;
                }
            }

            line = pattern[y];
        }

        return -1;
    }

    private async IAsyncEnumerable<string[]> ReadData(string filepath)
    {
        var pattern = new List<string>();
        foreach (var line in await File.ReadAllLinesAsync(filepath))
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                yield return pattern.ToArray();
                pattern.Clear();
            }
            else
            {
                pattern.Add(line);
            }
        }

        if (pattern.Any())
        {
            yield return pattern.ToArray();
        }
    }
}