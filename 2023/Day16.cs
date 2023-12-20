using static ColorWriter;

public class Day16
{
    [Flags]
    public enum Direction
    {
        Undefined = 0,
        North = 1,
        South = 2,
        East = 4,
        West = 8
    }

    public async Task Part1()
    {
        var map = await ReadData("Day16.txt");

        WriteLine(Solve(map, (0, 1)));
    }

    public async Task Part2()
    {
        var map = await ReadData("Day16.txt");

        var mapWidth = map.First().Length;
        var mapHeight = map.Length;

        var max = 0L;

        foreach (var x in 1.To(mapWidth-1))
        {
            max = long.Max(max, Solve(map, (x, 0)));
            max = long.Max(max, Solve(map, (x, mapHeight-1)));
        }

        foreach (var y in 1.To(mapHeight-1))
        {
            max = long.Max(max, Solve(map, (0, y)));
            max = long.Max(max, Solve(map, (mapWidth-1, y)));
        }

        WriteLine(max);
    }

    private long Solve(string[] map, (int x, int y) start)
    {
        var mapWidth = map.First().Length;
        var mapHeight = map.Length;

        var visited = new Direction[mapHeight][];

        for (int y = 0; y < mapHeight; y++)
        {
            visited[y] = new Direction[mapWidth];
        }

        var q = new List<(int x, int y, Direction dir)>(
            new [] { (start.x, start.y, 1 switch
            {
                _ when start.x == 0 => Direction.East,
                _ when start.y == 0 => Direction.South,
                _ when start.x == mapWidth-1 => Direction.West,
                _ when start.y == mapHeight-1 => Direction.North,
                _ => throw new InvalidOperationException()
            }) });

        while (q.Any())
        {
            var (x, y, dir) = q.First();
            q.RemoveAt(0);

            if (visited[y][x].HasFlag(dir)) continue;

            visited[y][x] |= dir;

            (x, y) = dir switch
            {
                Direction.North => (x, y-1),
                Direction.South => (x, y+1),
                Direction.East => (x+1, y),
                Direction.West => (x-1, y),
                _ => throw new InvalidOperationException()
            };

            var newDir = Array.Empty<Direction>();

            switch (map[y][x])
            {
                case ' ':
                    break;

                case '.':
                    newDir = [ dir ];
                    break;

                case '/':
                    newDir = dir switch
                    {
                        Direction.North => [ Direction.East ],
                        Direction.South => [ Direction.West ],
                        Direction.East => [ Direction.North ],
                        Direction.West => [ Direction.South ],
                        _ => throw new InvalidOperationException()
                    };
                    break;

                case '\\':
                    newDir = dir switch
                    {
                        Direction.North => [ Direction.West ],
                        Direction.South => [ Direction.East ],
                        Direction.East => [ Direction.South ],
                        Direction.West => [ Direction.North ],
                        _ => throw new InvalidOperationException()
                    };
                    break;

                case '-':
                    newDir = dir switch
                    {
                        Direction.North => [ Direction.East, Direction.West ],
                        Direction.South => [ Direction.East, Direction.West ],
                        Direction.East => [ Direction.East ],
                        Direction.West => [ Direction.West ],
                        _ => throw new InvalidOperationException()
                    };
                    break;

                case '|':
                    newDir = dir switch
                    {
                        Direction.North => [ Direction.North ],
                        Direction.South => [ Direction.South ],
                        Direction.East => [ Direction.North, Direction.South ],
                        Direction.West => [ Direction.North, Direction.South ],
                        _ => throw new InvalidOperationException()
                    };
                    break;
            }

            foreach (var nd in newDir)
            {
                q.Add((x, y, nd));
            }
        }

        visited[start.y][start.x] = 0;

        return visited.SelectMany(_ => _).Sum(_ => _ != 0 ? 1 : 0);
    }

    private async Task<string[]> ReadData(string filepath)
    {
        var lines = await File.ReadAllLinesAsync(filepath);
        var map = new List<string>();
        map.Add("".PadLeft(lines.First().Length+2, ' '));
        map.AddRange(lines.Select(_ => $" {_} "));
        map.Add("".PadLeft(lines.First().Length+2, ' '));

        return map.ToArray();
    }
}
