using static System.Console;

public class Day18
{
    public async Task Part1()
    {
        var cubes = new HashSet<(int x, int y, int z)>();
        await foreach (var (x, y, z) in ReadInput("Day18.txt"))
        {
            cubes.Add((x, y, z));
        }

        var sum = 0;
        foreach (var (x, y, z) in cubes)
        {
            foreach (var (xx, yy, zz) in new[]
            {
                (x+1, y  , z  ), (x-1, y  , z  ),
                (x  , y+1, z  ), (x  , y-1, z  ),
                (x  , y  , z+1), (x  , y  , z-1)
            })
            {
                if (!cubes.Contains((xx, yy, zz))) sum++;
            }
        }

        WriteLine(sum);
    }

    public async Task Part2()
    {
        var cubes = new HashSet<(int x, int y, int z)>();
        var minX = int.MaxValue;
        var input = ReadInput("Day18.txt");
        await foreach (var (x, y, z) in input)
        {
            minX = Math.Min(minX, x);
            cubes.Add((x, y, z));
        }

        Func<char, char> get_opposite = input => input switch
        {
            'L' => 'R',
            'R' => 'L',
            'U' => 'D',
            'D' => 'U',
            'F' => 'B',
            'B' => 'F',
            _ => throw new InvalidOperationException($"'{input}' is invalid")
        };

        Func<char, (int dx, int dy, int dz, char s)[]> get_neighbors = input => input switch
        {
            'L' => new[] { (-1,  1,  0, 'D'), (-1, -1,  0, 'U'), (-1,  0,  1, 'B'), (-1,  0, -1, 'F') },
            'R' => new[] { ( 1,  1,  0, 'D'), ( 1, -1,  0, 'U'), ( 1,  0,  1, 'B'), ( 1,  0, -1, 'F') },
            'U' => new[] { ( 1,  1,  0, 'L'), (-1,  1,  0, 'R'), ( 0,  1,  1, 'B'), ( 0,  1, -1, 'F') },
            'D' => new[] { ( 1, -1,  0, 'L'), (-1, -1,  0, 'R'), ( 0, -1,  1, 'B'), ( 0, -1, -1, 'F') },
            'F' => new[] { ( 1,  0,  1, 'L'), (-1,  0,  1, 'R'), ( 0,  1,  1, 'D'), ( 0, -1,  1, 'U') },
            'B' => new[] { ( 1,  0, -1, 'L'), (-1,  0, -1, 'R'), ( 0,  1, -1, 'D'), ( 0, -1, -1, 'U') },
            _ => throw new InvalidOperationException($"{input} is invalid")
        };

        Func<char, (int dx, int dy, int dz, char s)[]> get_neighbors2 = input => input switch
        {
            'L' => new[] { ( 0,  1,  0, 'L'), ( 0, -1,  0, 'L'), ( 0,  0,  1, 'L'), ( 0,  0, -1, 'L') },
            'R' => new[] { ( 0,  1,  0, 'R'), ( 0, -1,  0, 'R'), ( 0,  0,  1, 'R'), ( 0,  0, -1, 'R') },
            'U' => new[] { ( 1,  0,  0, 'U'), (-1,  0,  0, 'U'), ( 0,  0,  1, 'U'), ( 0,  0, -1, 'U') },
            'D' => new[] { ( 1,  0,  0, 'D'), (-1,  0,  0, 'D'), ( 0,  0,  1, 'D'), ( 0,  0, -1, 'D') },
            'F' => new[] { ( 0,  1,  0, 'F'), ( 0, -1,  0, 'F'), ( 1,  0,  0, 'F'), (-1,  0,  0, 'F') },
            'B' => new[] { ( 0,  1,  0, 'B'), ( 0, -1,  0, 'B'), ( 1,  0,  0, 'B'), (-1,  0,  0, 'B') },
            _ => throw new InvalidOperationException($"{input} is invalid")
        };

        var start = cubes.First(_ => _.x == minX);

        var visited = new HashSet<(int x, int y, int z, char s)>
        {
            (start.x, start.y, start.z, 'L')
        };

        var q = new List<(int x, int y, int z, char s)>
        {
            (start.x, start.y, start.z, 'L')
        };

        var hits = new List<(int x, int y, int z, char s)>();
        while (q.Any())
        {
            var dq = q.First();
            q.RemoveAt(0);
            hits.Add((dq.x, dq.y, dq.z, dq.s));

            foreach (var (dx, dy, dz, ds) in get_neighbors2(dq.s))
            {
                var next = (x: dq.x+dx, y: dq.y+dy, z: dq.z+dz, s: ds);
                if (!cubes.Contains((next.x, next.y, next.z))) continue;
                if (next.s switch {
                    'L' => cubes.Contains((next.x-1, next.y,   next.z)),
                    'R' => cubes.Contains((next.x+1, next.y,   next.z)),
                    'U' => cubes.Contains((next.x,   next.y+1, next.z)),
                    'D' => cubes.Contains((next.x,   next.y-1, next.z)),
                    'F' => cubes.Contains((next.x,   next.y,   next.z+1)),
                    'B' => cubes.Contains((next.x,   next.y,   next.z-1)),
                    _ => throw new InvalidOperationException()
                }) continue;
                if (visited.Contains(next)) continue;
                visited.Add(next);
                q.Add((next.x, next.y, next.z, next.s));
            }

            var neighbor_deltas = get_neighbors(dq.s);

            foreach (var (dx, dy, dz, ds) in neighbor_deltas)
            {
                var next = (x: dq.x+dx, y: dq.y+dy, z: dq.z+dz, s: ds);
                if (!cubes.Contains((next.x, next.y, next.z))) continue;
                if (visited.Contains(next)) continue;
                visited.Add(next);
                q.Add((next.x, next.y, next.z, next.s));
            }

            var thisCubeNeighbors = dq.s switch {
                var s when s == 'F' || s == 'B' => neighbor_deltas.Select(_ => (_.dx, _.dy,    0, _.s)),
                var s when s == 'L' || s == 'R' => neighbor_deltas.Select(_ => (   0, _.dy, _.dz, _.s)),
                var s when s == 'U' || s == 'D' => neighbor_deltas.Select(_ => (_.dx,    0, _.dz, _.s)),
                _ => throw new InvalidOperationException($"{dq.s}")
            };

            foreach (var (dx, dy, dz, ds) in thisCubeNeighbors)
            {
                var next = (x: dq.x, y: dq.y, z: dq.z, s: get_opposite(ds));
                if (cubes.Contains((dq.x + dx, dq.y + dy, dq.z + dz)))
                {
                    continue;
                }

                var (dx2, dy2, dz2) = (dq.s, next.s) switch
                {
                    ('L', 'D') => (-1, -1, dz),
                    ('D', 'L') => (-1, -1, dz),
                    ('L', 'U') => (-1,  1, dz),
                    ('U', 'L') => (-1,  1, dz),
                    ('R', 'U') => ( 1,  1, dz),
                    ('U', 'R') => ( 1,  1, dz),
                    ('R', 'D') => ( 1, -1, dz),
                    ('D', 'R') => ( 1, -1, dz),

                    ('F', 'D') => (dx, -1,  1),
                    ('D', 'F') => (dx, -1,  1),
                    ('F', 'U') => (dx,  1,  1),
                    ('U', 'F') => (dx,  1,  1),
                    ('B', 'U') => (dx,  1, -1),
                    ('U', 'B') => (dx,  1, -1),
                    ('B', 'D') => (dx, -1, -1),
                    ('D', 'B') => (dx, -1, -1),

                    ('F', 'L') => (-1, dy,  1),
                    ('L', 'F') => (-1, dy,  1),
                    ('F', 'R') => ( 1, dy,  1),
                    ('R', 'F') => ( 1, dy,  1),
                    ('B', 'R') => ( 1, dy, -1),
                    ('R', 'B') => ( 1, dy, -1),
                    ('B', 'L') => (-1, dy, -1),
                    ('L', 'B') => (-1, dy, -1),

                    _ => throw new InvalidOperationException($"({dq.s}, {next.s})")
                };

                if (cubes.Contains((dq.x + dx2, dq.y + dy2, dq.z + dz2)))
                {
                    continue;
                }

                if (visited.Contains(next)) continue;
                visited.Add(next);
                q.Add((next.x, next.y, next.z, next.s));
            }
        }

        WriteLine(hits.Count);
    }

    public async IAsyncEnumerable<(int x, int y, int z)> ReadInput(string filename)
    {
        await foreach(var line in System.IO.File.ReadLinesAsync(filename))
        {
            if (string.IsNullOrEmpty(line) || line[0] == '#') continue;

            var parts = line.Split(",");
            yield return (int.Parse(parts[0]),int.Parse(parts[1]),int.Parse(parts[2]));
        }

        yield break;
    }
}