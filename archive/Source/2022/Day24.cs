
public class Day24
{
    public async Task Part1()
    {
        var filename = "Day24.txt";

        var ((width, height), blizzards) = await ReadData(filename);

        //PrintState(0, blizzards);

        var positions = new HashSet<(int x, int y)>();
        positions.Add((0, -1));

        var nextPositions = new HashSet<(int x, int y)>();

        int state = 1;

        for (;; state++)
        {
            blizzards = NextState(blizzards, width, height);

            //PrintState(state, blizzards);
            nextPositions = new();
            nextPositions.Add((0, -1));

            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                if (blizzards.ContainsKey((x, y)))
                {
                    continue;
                }

                var add = false;

                if (positions.Contains((x, y)))
                {
                    nextPositions.Add((x, y));
                    add = true;
                }

                if (x-1 >= 0 && positions.Contains((x-1, y)))
                {
                    nextPositions.Add((x, y));
                    add = true;
                }

                if (x+1 <= width-1 && positions.Contains((x+1, y)))
                {
                    nextPositions.Add((x, y));
                    add = true;
                }

                if (y+1 <= height-1 && positions.Contains((x, y+1)))
                {
                    nextPositions.Add((x, y));
                    add = true;
                }

                if (y-1 >= 0 && positions.Contains((x, y-1)))
                {
                    nextPositions.Add((x, y));
                    add = true;
                }

                if ((x, y) == (0, 0) && positions.Contains((0, -1)))
                {
                    nextPositions.Add((x, y));
                    add = true;
                }

                if (add && (x, y) == (width-1, height-1))
                {
                    Console.WriteLine($"Solution at {state+1}");
                    return;
                }
            }

            positions = nextPositions;
        }
    }

    public async Task Part2()
    {
        var filename = "Day24.txt";
        var ((width, height), blizzards) = await ReadData(filename);

        var state = blizzards;
        int steps;

        var _1 = (0, -1);
        var _2 = (width-1, height);

        int sum = 0;

        (steps, state) = Solve(_1, _2, state, width, height);
        steps ++;
        sum += steps;
        Console.WriteLine(steps);

        (steps, state) = Solve(_2, _1, state, width, height);
        sum += steps;
        Console.WriteLine(steps);

        (steps, state) = Solve(_1, _2, state, width, height);
        sum += steps;
        Console.WriteLine(steps);

        Console.WriteLine("------");
        Console.WriteLine(sum);
        Console.WriteLine("------");
    }

    private (int steps, Dictionary<(int x, int y), List<char>> state) Solve((int x, int y) start, (int x, int y) goal, Dictionary<(int x, int y), List<char>> state, int width, int height)
    {
        var positions = new HashSet<(int x, int y)>(new[] { start });
        HashSet<(int x, int y)> nextPositions;

        int stateIdx = 0;

        for (;;)
        {
            state = NextState(state, width, height);
            stateIdx++;

            nextPositions = new(new[] { start });

            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                if (state.ContainsKey((x, y)))
                {
                    continue;
                }

                var add = false;

                if (positions.Contains((x, y)))
                {
                    nextPositions.Add((x, y));
                    add = true;
                }

                if (x-1 >= 0 && positions.Contains((x-1, y)))
                {
                    nextPositions.Add((x, y));
                    add = true;
                }

                if (x+1 <= width-1 && positions.Contains((x+1, y)))
                {
                    nextPositions.Add((x, y));
                    add = true;
                }

                if (y+1 <= height-1 && positions.Contains((x, y+1)))
                {
                    nextPositions.Add((x, y));
                    add = true;
                }

                if (y-1 >= 0 && positions.Contains((x, y-1)))
                {
                    nextPositions.Add((x, y));
                    add = true;
                }

                if ((x, y) == (0, 0) && positions.Contains((0, -1)))
                {
                    nextPositions.Add((x, y));
                    add = true;
                }

                if ((x, y) == (width-1, height-1) && positions.Contains((width-1, height)))
                {
                    nextPositions.Add((x, y));
                    add = true;
                }

                if (add && int.Abs(goal.x - x) + int.Abs(goal.y - y) == 1)
                {
                    return (stateIdx, state);
                }
            }

            positions = nextPositions;
        }
    }

    private void PrintState(int state, Dictionary<(int x, int y), List<char>> blizzards)
    {
        Console.WriteLine();
        Console.WriteLine($"State {state}");
        foreach (var (key, value) in blizzards)
        {
            Console.WriteLine($"({key.x}, {key.y}) {string.Join(", ", value)}");
        }
    }

    private Dictionary<(int x, int y), List<char>> NextState(Dictionary<(int x, int y), List<char>> blizzards, int width, int height)
    {
        // Console.WriteLine($"Width: {width}  Height: {height}");
        var next = new Dictionary<(int x, int y), List<char>>();

        foreach (var (key, value) in blizzards)
        {
            foreach (var dir in value)
            {
                // Console.Write($"Next ({key.x}, {key.y}): {dir}  -->  ");

                var (nextX, nextY) = dir switch
                {
                    '>' => (key.x >= width-1 ? 0 : key.x+1, key.y),
                    '<' => (key.x <= 0 ? width-1 : key.x-1, key.y),
                    '^' => (key.x, key.y <= 0 ? height-1 : key.y-1),
                    'v' => (key.x, key.y >= height-1 ? 0 : key.y+1),
                    _ => throw new InvalidOperationException()
                };

                if (!next.TryGetValue((nextX, nextY), out var directions))
                {
                    directions = new();
                    next.Add((nextX, nextY), directions);
                }

                directions.Add(dir);
                // Console.WriteLine($"({nextX}, {nextY}): {dir}");
            }
        }

        return next;
    }

    private async Task<((int width, int height), Dictionary<(int x, int y), List<char>>)> ReadData(string filename)
    {
        int width = 0;
        int y = 0;

        var blizzards = new Dictionary<(int, int), List<char>>();

        foreach (var line in await File.ReadAllLinesAsync(filename))
        {
            if (string.IsNullOrEmpty(line))
            {
                break;
            }

            if (line[1..^1].Contains('#'))
            {
                continue;
            }

            width = line.Length - 2;
            var current = line[1..^1];
            for (int x = 0; x < current.Length; x++)
            {
                if (current[x] != '.')
                {
                    blizzards.Add((x, y), new[] { current[x] }.ToList());
                }
            }

            y++;
        }

        return ((width, y), blizzards);
    }
}
