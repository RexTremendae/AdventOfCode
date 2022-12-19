using static System.Console;

public class Day12
{
    public async Task Part1()
    {
        var (map, start, goal) = await ReadInput("Day12.txt");
        Clock.Tick();
        var solution = Solve(map, start, goal);
        WriteLine($"{Clock.Tock().TotalSeconds:0.00} s");
        //WriteLine(solution.Length-1);
        WriteLine(solution);
    }

    private List<(int heuristic, int[][] visited, int distance, (int x, int y) position, (int x, int y)[] path)> Setup(int[][] map, (int x, int y) start, (int x, int y) goal)
    {
        var height = map.Length;
        var width = map[0].Length;

        var visited = new int[height][];
        foreach (var y in 0.To(height))
        {
            visited[y] = new int[width];
            foreach (var x in 0.To(width))
            {
                visited[y][x] = int.MaxValue;
            }
        }

        return new List<(int heuristic, int[][] visited, int distance, (int x, int y) position, (int x, int y)[] path)>
        {
            (heuristic: width+height,
             visited: visited,
             distance: 0,
             position: start,
             path: new (int, int)[] { start })
        };
    }

    private int Solve(int[][] map, (int x, int y) start, (int x, int y) goal)
    {
        var height = map.Length;
        var width = map[0].Length;

        var q = Setup(map, start, goal);

        var current = 0;

        while (q.Any())
        {
            var dq = q.First();
            q.RemoveAt(0);
            //WriteLine(dq.position);
            //WriteLine(string.Join(" ", dq.path));

            if (dq.heuristic > current)
            {
                WriteLine(dq.heuristic);
                current = dq.heuristic;
            }

            var px = dq.position.x;
            var py = dq.position.y;

            var candidates = new List<(int x, int y, int height)>();

            var nDist = dq.distance+1;

            foreach (var (x, y) in new[] { (px+1, py), (px-1, py), (px, py+1), (px, py-1) })
            {
                if (x < 0 || y < 0 || x >= width || y >= height) continue;
                if (map[y][x] > map[py][px]+1) continue;
                if (dq.visited[y][x] <= nDist) continue;

                candidates.Add((x, y, map[y][x]));
            }

            foreach (var (x, y, h) in candidates.OrderBy(_ => _.height))
            {
                if ((x, y) == goal)
                {
                    return nDist;
                    //return dq.path.Concat(new[] {(x, y)}).ToArray();
                }

                var nVisited = dq.visited.Select(_ => _.ToArray()).ToArray();
                nVisited[y][x] = nDist;
                //var nPath = dq.path.Concat(new[] {(x, y)}).ToArray();

                var heuristic = (Math.Abs(x - goal.x) + Math.Abs(y - goal.y)) + (25 - h);
                heuristic += nDist + (25 - map[y][x]);

                var idx = 0;
                while (idx < q.Count && heuristic > q[idx].heuristic) idx++;
                //q.Insert(idx, (heuristic + nDist, nVisited, nDist, (x,y), nPath));
                q.Insert(idx, (heuristic, nVisited, nDist, (x,y), Array.Empty<(int, int)>()));
            }
        }

        return 0;
        //return Array.Empty<(int, int)>();
    }

    private async Task<(int[][] map, (int x, int y) start, (int x, int y) goal)> ReadInput(string filename)
    {
        var start = (-1, -1);
        var goal = (-1, -1);
        var map = new List<int[]>();

        int y = 0;
        await foreach(var line in System.IO.File.ReadLinesAsync(filename))
        {
            if (string.IsNullOrEmpty(line)) break;
            int x = 0;
            var heightList = new List<int>();
            foreach (var ch in line)
            {
                var height = 0;

                switch (ch)
                {
                    case 'S':
                        start = (x, y);
                        break;

                    case 'E':
                        height = ('z' - 'a');
                        goal = (x, y);
                        break;

                    default:
                        height = ch - 'a';
                        break;
                }

                heightList.Add(height);
                x++;
            }
            y++;
            map.Add(heightList.ToArray());
        }

        if (start == (-1, -1) || goal == (-1, -1)) throw new InvalidOperationException();

        return (map.ToArray(), start, goal);
    }
}
