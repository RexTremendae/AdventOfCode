using static System.Console;

public class Day12
{
    public void Part1(string data)
    {
        var (map, start, goal) = ParseData(data);
        WriteLine(Solve1(map, start, goal));
    }

    public void Part2(string data)
    {
        var (map, start, goal) = ParseData(data);
        WriteLine(Solve2(map, start, goal));
    }

    private int Solve1(int[][] map, (int x, int y) start, (int x, int y) goal)
    {
        var width = map[0].Length;
        var height = map.Length;

        var visited = new int[height][];
        for(int y = 0; y < height; y++)
        {
            visited[y] = new int[width];
            for (int x = 0; x < width; x++) visited[y][x] = int.MaxValue;
        }

        var q = new List<(int x, int y, int len, int elv)>()
        {
            (start.x, start.y, 0, map[start.y][start.x])
        };

        while (q.Any())
        {
            var dq = q.First();
            q.RemoveAt(0);

            foreach (var (x, y) in new[] {(dq.x+1, dq.y), (dq.x-1, dq.y), (dq.x, dq.y+1), (dq.x, dq.y-1)})
            {
                if (x < 0 || x >= width || y < 0 || y >= height) continue;
                if (map[y][x] > dq.elv+1) continue;
                if ((x, y) == goal)
                {
                    return dq.len + 1;
                }
                var nLen = dq.len + 1;
                if (nLen >= visited[y][x]) continue;
                visited[y][x] = nLen;
                q.Add((x, y, nLen, map[y][x]));
            }
        }

        return -1;
    }

    private int Solve2(int[][] map, (int x, int y) start, (int x, int y) goal)
    {
        var width = map[0].Length;
        var height = map.Length;

        var visited = new int[height][];
        for(int y = 0; y < height; y++)
        {
            visited[y] = new int[width];
            for (int x = 0; x < width; x++) visited[y][x] = int.MaxValue;
        }

        var q = new List<(int x, int y, int len, int elv)>()
        {
            (goal.x, goal.y, 0, map[goal.y][goal.x])
        };

        while (q.Any())
        {
            var dq = q.First();
            q.RemoveAt(0);

            foreach (var (x, y) in new[] {(dq.x+1, dq.y), (dq.x-1, dq.y), (dq.x, dq.y+1), (dq.x, dq.y-1)})
            {
                if (x < 0 || x >= width || y < 0 || y >= height) continue;
                if (map[y][x] < dq.elv-1) continue;
                var nLen = dq.len + 1;
                if (nLen >= visited[y][x]) continue;
                visited[y][x] = nLen;
                q.Add((x, y, nLen, map[y][x]));
            }
        }

        var minLen = int.MaxValue;
        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            if (map[y][x] != 1) continue;
            if (visited[y][x] < minLen) minLen = visited[y][x];
        }

        return minLen;
    }

    private (int[][] map, (int x, int y) start, (int x, int y) goal) ParseData(string data)
    {
        var map = new List<int[]>();

        int y = 0;
        var start = (x:0, y:0);
        var goal  = (x:0, y:0);

        foreach (var line in data.Split("\n").Select(_ => _.Replace("\r", "")))
        {
            if (string.IsNullOrEmpty(line)) continue;
            var row = new int[line.Length];
            map.Add(row);

            for (var x = 0; x < line.Length; x++)
            {
                var ch = ' ';
                if (line[x] == 'S')
                {
                    ch = 'a';
                    start = (x, y);
                }
                else if (line[x] == 'E')
                {
                    ch = 'z';
                    goal = (x, y);
                }
                else
                {
                    ch = line[x];
                }
                row[x] = (ch - 'a' + 1);
            }
            y++;
        }

        return (map.ToArray(), start, goal);
    }

    public const string SmallMap = """
    Sabqponm
    abcryxxl
    accszExk
    acctuvwj
    abdefghi
    """;

    public const string PuzzleMap = """
    abacccaaaacccccccccccaaaaaacccccaaaaaaccccaaacccccccccccccccccccccccccccccccccccccccccccaaaaa
    abaaccaaaacccccccccccaaaaaaccccccaaaaaaaaaaaaaccccccccccccccccccccccccccccccccccccccccccaaaaa
    abaaccaaaacccccccccccaaaaacccccaaaaaaaaaaaaaaaccccccccccccccccccccccccccccccccccccccccccaaaaa
    abccccccccccccccccccccaaaaacccaaaaaaaaaaaaaaaacccccccccccccccccccccccccccaaaccccccccccccaaaaa
    abccccccccccccccccccccaacaacccaaaaaaaaccaaaaaccccccccccccccccccccccccccccaaaccccccccccccaccaa
    abcccccccccccccaacccaaaccccccaaaaaaaaaccaaaaaccccccccccccccccccccccccccccccacccccccccccccccca
    abcccccccccccaaaaaaccaaaccacccccaaaaaaacccccccccccccccccccccccccciiiicccccccddddddccccccccccc
    abcccccccccccaaaaaaccaaaaaaaccccaaaaaacccccaacccccccaaaccccccccciiiiiiiicccdddddddddacaaccccc
    abccccccccccccaaaaaaaaaaaaacccccaaaaaaacaaaacccccccaaaacccccccchhiiiiiiiiicddddddddddaaaccccc
    abcccccccccccaaaaaaaaaaaaaacccccccaaacccaaaaaacccccaaaaccccccchhhipppppiiiijjjjjjjddddaaccccc
    abcccccccccccaaaaaaaaaaaaaaccccccccccccccaaaaaccccccaaaccccccchhhpppppppiijjjjjjjjjddeeaccccc
    abcccccccccccccccccaaaaaaaacccccccccccccaaaaaccccccccccccccccchhppppppppppjjqqqjjjjjeeeaacccc
    abccccccccccccccccccaaaaaaaacccccccccccccccaacccccccccccccccchhhpppuuuupppqqqqqqqjjjeeeaacccc
    abcccccccccccccccccccaacccacccccccccccccccccccccccccccccccccchhhopuuuuuuppqqqqqqqjjjeeecccccc
    abacccccccccccccaaacaaaccccccccccccccccccccccccccccaaccccccchhhhoouuuuuuuqvvvvvqqqjkeeecccccc
    abaccccccccccccaaaaaacccccaaccccccccccccccccccccccaaaccccccchhhooouuuxxxuvvvvvvqqqkkeeecccccc
    abaccccccccccccaaaaaacccaaaaaaccccccccccccccccccaaaaaaaaccchhhhooouuxxxxuvyyyvvqqqkkeeecccccc
    abcccccccccccccaaaaacccaaaaaaaccccccccccccccccccaaaaaaaaccjjhooooouuxxxxyyyyyvvqqqkkeeecccccc
    abccccccccccccccaaaaaacaaaaaaaccccccccaaaccccccccaaaaaaccjjjooootuuuxxxxyyyyyvvqqkkkeeecccccc
    abccccccccccccccaaaaaaaaaaaaacccccccccaaaacccccccaaaaaacjjjooootttuxxxxxyyyyvvrrrkkkeeecccccc
    SbccccccccccccccccccaaaaaaaaacccccccccaaaacccccccaaaaaacjjjoootttxxxEzzzzyyvvvrrrkkkfffcccccc
    abcccccccccccaaacccccaaaaaaacaaaccccccaaaccccccccaaccaacjjjoootttxxxxxyyyyyyvvvrrkkkfffcccccc
    abcccccccccaaaaaacccaaaaaacccaaacacccaacccccccccccccccccjjjoootttxxxxyxyyyyyywvvrrkkkfffccccc
    abcccccccccaaaaaacccaaaaaaaaaaaaaaaccaaacaaacccccaacccccjjjnnnttttxxxxyyyyyyywwwrrkkkfffccccc
    abcaacacccccaaaaacccaaacaaaaaaaaaaaccaaaaaaacccccaacaaacjjjnnnntttttxxyywwwwwwwwrrrlkfffccccc
    abcaaaaccccaaaaacccccccccaacaaaaaaccccaaaaaacccccaaaaacccjjjnnnnnttttwwywwwwwwwrrrrllfffccccc
    abaaaaaccccaaaaaccccccaaaaaccaaaaacaaaaaaaaccccaaaaaaccccjjjjinnnntttwwwwwsssrrrrrllllffccccc
    abaaaaaaccccccccccccccaaaaacaaaaaacaaaaaaaaacccaaaaaaacccciiiiinnnntswwwwssssrrrrrlllfffccccc
    abacaaaaccccccccccccccaaaaaacaaccccaaaaaaaaaaccccaaaaaaccccciiiinnnssswwsssssllllllllfffccccc
    abccaaccccccccccccccccaaaaaaccccccccccaaacaaaccccaaccaacccccciiiinnsssssssmmllllllllfffaacccc
    abccccccccccccccccccccaaaaaaccccccccccaaaccccccccaaccccccccccciiinnmsssssmmmmlllllgggffaacccc
    abcccccccccccccccaccccccaaacccccccccccaaccccccccccccccccccccccciiimmmsssmmmmmgggggggggaaacccc
    abcccccccccaaaaaaaaccccccccccccccccccccccccccccaaaaaccccccccccciiimmmmmmmmmgggggggggaaacccccc
    abccccccccccaaaaaaccccccccccccccccccaacccccccccaaaaacccccccccccciiimmmmmmmhhggggcaaaaaaaccccc
    abccccccccccaaaaaacccccccccccccccccaacccccccccaaaaaacccccccccccciihhmmmmhhhhgccccccccaacccccc
    abccccaacaaaaaaaaaaccccccccccccccccaaaccccccccaaaaaaccccccccccccchhhhhhhhhhhaaccccccccccccccc
    abccccaaaaaaaaaaaaaaccccccccccaaccaaaaccccccccaaaaaacccaaacccccccchhhhhhhhaaaaccccccccccccccc
    abcccaaaaaaaaaaaaaaaccccccccaaaaaacaaaacacaccccaaaccccaaaacccccccccchhhhccccaaccccccccccaaaca
    abcccaaaaaacacaaacccccccccccaaaaaaaaaaaaaaacccccccccccaaaacccccccccccaaaccccccccccccccccaaaaa
    abcccccaaaacccaaaccccccccccaaaaaaaaaaaaaaaaccccccccccccaaacccccccccccaaacccccccccccccccccaaaa
    abcccccaacccccaacccccccccccaaaaaaaaaaaaaccccccccccccccccccccccccccccccccccccccccccccccccaaaaa
    """;
}
