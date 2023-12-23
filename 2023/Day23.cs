using static ColorWriter;

public class Day23
{
    public async Task Part1()
    {
        var map = await ReadData("Day23.txt").ToArrayAsync();

        WriteLine(Solve(map));
    }

    private long Solve(string[] map)
    {
        var start = (x: 0, y: 0);
        var goal = (x: 0, y: 0);

        foreach (var x in 0.To(map[0].Length))
        {
            if (map[0][x] == '.') start = (x, 0);
            if (map[^1][x] == '.') goal = (x, map.Length-1);
        }

        map[0] = map[0][..start.x] + 'v' + map[0][(start.x+1)..];
        map[^1] = map[^1][..goal.x] + 'v' + map[^1][(goal.x+1)..];

        var q = new List<(int x, int y, HashSet<(int, int)> visited)> { (start.x, start.y, new()) };

        var maxLen = 0L;

        while (q.Any())
        {
            var dq = q.First();
            q.RemoveAt(0);

            foreach (var (nx, ny) in GetCandidates((dq.x, dq.y), map))
            {
                if (dq.visited.Contains((nx, ny))) continue;
                var nvisit = dq.visited.ToHashSet();

                if ((nx, ny) == goal)
                {
                    maxLen = long.Max(dq.visited.Count+1, maxLen);
                }
                else
                {
                    var isVisited = nvisit.Contains((nx, ny));
                    if (!isVisited || (nvisit.Count < nvisit.Count + 1))
                    {
                        nvisit.Add((nx, ny));
                        q.Add((nx, ny, nvisit));
                    }
                }
            }
        }

        return maxLen;
    }

    private IEnumerable<(int x, int y)> GetCandidates((int x, int y) pos, string[] map)
    {
        var current = map[pos.y][pos.x];

        if (!"<^>".Contains(current) && map[pos.y+1][pos.x] != '#') yield return (pos.x, pos.y+1);
        if (!"<v>".Contains(current) && map[pos.y-1][pos.x] != '#') yield return (pos.x, pos.y-1);
        if (!"^>v".Contains(current) && map[pos.y][pos.x-1] != '#') yield return (pos.x-1, pos.y);
        if (!"^<v".Contains(current) && map[pos.y][pos.x+1] != '#') yield return (pos.x+1, pos.y);
    }

    private async IAsyncEnumerable<string> ReadData(string filepath)
    {
        foreach (var line in await File.ReadAllLinesAsync(filepath))
        {
            if (string.IsNullOrWhiteSpace(line)) break;
            yield return line;
        }
    }
}
