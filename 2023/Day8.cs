using static ColorWriter;

public class Day8
{
    public async Task Part1()
    {
        var (instructions, map) = await ReadData("Day8.txt");
        var current = "AAA";
        var instrIdx = 0;
        var steps = 0;

        while (current != "ZZZ")
        {
            var next = instructions[instrIdx] switch
            {
                'L' => map[current].L,
                'R' => map[current].R,
                _ => throw new InvalidOperationException()
            };

            //WriteLine($"{current} => {next}");
            current = next;

            steps++;
            instrIdx++;
            if (instrIdx >= instructions.Length) instrIdx = 0;
        }

        WriteLine(steps);
    }

    public async Task<(string instructions, Dictionary<string, (string L, string R)> map)> ReadData(string filepath)
    {
        var isFirst = true;
        var instructions = string.Empty;
        var map = new Dictionary<string, (string, string)>();
        var splitOpts = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;

        foreach (var line in await File.ReadAllLinesAsync(filepath))
        {
            if (isFirst)
            {
                instructions = line.Trim();
                isFirst = false;
                continue;
            }

            if (string.IsNullOrWhiteSpace(line)) continue;

            var split = line.Split('=', splitOpts);
            var lr = split[1][1..^1].Split(',', splitOpts);
            map.Add(split[0], (lr[0], lr[1]));
        }

        return (instructions, map);
    }
}
