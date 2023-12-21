using static ColorWriter;

public class Day21
{
    public async Task Part1()
    {
        var (map, start) = await LoadData("Day21.txt");

        var height = map.Length;
        var width = map.First().Length;

        var candidates = new[] { start }.ToHashSet();
        foreach (var _ in 0.To(64))
        {
            var nextCandidates = new HashSet<(int x, int y)>();
            foreach (var cnd in candidates)
            {
                foreach (var next in new (int x, int y)[] {
                    (cnd.x, cnd.y+1), (cnd.x, cnd.y-1),
                    (cnd.x+1, cnd.y), (cnd.x-1, cnd.y) })
                {
                    if (next.x < 0 || next.y < 0 || next.x >= width || next.y >= height)
                        continue;

                    if (map[next.y][next.x] == '.')
                        nextCandidates.Add(next);
                }
            }
            candidates = nextCandidates;
        }

        WriteLine(candidates.Count);

        //PrintMap(map, candidates);
    }

    private void PrintMap(string[] map, HashSet<(int x, int y)> candidates)
    {
        for (int y = 0; y < map.Length; y++)
        {
            var line = map[y];
            for (int x = 0; x < line.Length; x++)
            {
                if (candidates.Contains((x, y))) Write ('O');
                else Write(line[x]);
            }
            WriteLine();
        }
    }

    private async Task<(string[] map, (int x, int y) start)> LoadData(string filepath)
    {
        var map = new List<string>();
        var start = (0, 0);

        foreach (var line in await File.ReadAllLinesAsync(filepath))
        {
            if (string.IsNullOrWhiteSpace(line)) break;
            var x = line.IndexOf('S');
            var lineToAdd = line;
            if (x >= 0)
            {
                start = (x, map.Count);
                lineToAdd = lineToAdd.Replace("S", ".");
            }
            map.Add(lineToAdd);
        }

        return (map.ToArray(), start);
    }
}
