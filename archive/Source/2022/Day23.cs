using static System.Console;

public class Day23
{
    public async Task Part1()
    {
        var data = await ReadInput("Day23.txt");

        //PrintData(data);
        int dirIdx = 0;
        foreach (var round in 0.To(10))
        {
            data = GetNextState(data, dirIdx);
            dirIdx++;
            if (dirIdx >= Directions.Length) dirIdx = 0;
            //PrintData(data);
        }

        int minX = 0;
        int minY = 0;
        int maxX = 0;
        int maxY = 0;

        foreach (var (x, y) in data)
        {
            minX = Math.Min(minX, x);
            maxX = Math.Max(maxX, x);
            minY = Math.Min(minY, y);
            maxY = Math.Max(maxY, y);
        }

        var sum = 0L;
        foreach (var y in minY.To(maxY+1))
        {
            foreach (var x in minX.To(maxX+1))
            {
                if (!data.Contains((x,y)))
                sum ++;
            }
        }

        WriteLine(sum);
    }

    public async Task Part2()
    {
        var state = await ReadInput("Day23.txt");

        int dirIdx = 0;
        var round = 0;

        Clock.Tick();
        for(;;)
        {
            var nextState = GetNextState(state, dirIdx);
            dirIdx++;
            if (dirIdx >= Directions.Length) dirIdx = 0;
            round++;

            if (nextState.Count != state.Count)
            {
                state = nextState;
                continue;
            }

            var isSame = true;
            foreach (var (x, y) in state)
            {
                if (!nextState.Contains((x, y)))
                {
                    state = nextState;
                    isSame = false;
                    break;
                }
            }

            if (isSame) break;
        }

        WriteLine($"{Clock.Tock().TotalSeconds:0.00} s");
        WriteLine(round);
    }

    private const string Directions = "NSWE";

    private HashSet<(int x, int y)> GetNextState(HashSet<(int x, int y)> state, int dirIdx)
    {
        var propositions = new Dictionary<(int, int), List<(int, int)>>();
        var nextState = state.ToHashSet();

        foreach (var (x, y) in state)
        {
            var deltas = new[] { 
                (x:-1, y:-1), (x:0, y:-1), (x:+1, y:-1),
                (x:-1, y:0),               (x:+1, y:0),
                (x:-1, y:+1), (x:0, y:+1), (x:+1, y:+1)
            };

            var doNothing = true;
            foreach (var (Δx, Δy) in deltas)
            {
                if (state.Contains((x+Δx, y+Δy)))
                {
                    doNothing = false;
                    break;
                }
            }
            if (doNothing) continue;

            (int x, int y)? prp = null;
            var pDirIdx = dirIdx;
            foreach (var d in 0.To(4))
            {
                switch (Directions[pDirIdx])
                {
                    case 'N':
                        if (!deltas.Where(_ => _.y == -1)
                            .Any(_ => state.Contains((x+_.x, y+_.y))))
                        {
                            prp = (x, y-1);
                        }
                        break;

                    case 'S':
                        if (!deltas.Where(_ => _.y == +1)
                            .Any(_ => state.Contains((x+_.x, y+_.y))))
                        {
                            prp = (x, y+1);
                        }
                        break;

                    case 'E':
                        if (!deltas.Where(_ => _.x == +1)
                            .Any(_ => state.Contains((x+_.x, y+_.y))))
                        {
                            prp = (x+1, y);
                        }
                        break;

                    case 'W':
                        if (!deltas.Where(_ => _.x == -1)
                            .Any(_ => state.Contains((x+_.x, y+_.y))))
                        {
                            prp = (x-1, y);
                        }
                        break;
                }
                if (prp != null) break;

                pDirIdx ++;
                if (pDirIdx >= Directions.Length) pDirIdx = 0;
            }

            if (prp == null) continue;

            if (!propositions.TryGetValue(prp.Value, out var fromList))
            {
                fromList = new();
                propositions.Add(prp.Value, fromList);
            }
            fromList.Add((x, y));
        }

        foreach (var (to, fromList) in propositions)
        {
            if (fromList.Count == 1)
            {
                nextState.Add(to);
                nextState.Remove(fromList.First());
            }
        }

        return nextState;
    }

    private void PrintData(HashSet<(int x, int y)> data)
    {
        int minX = 0;
        int minY = 0;
        int maxX = 0;
        int maxY = 0;

        foreach (var (x, y) in data)
        {
            minX = Math.Min(minX, x);
            maxX = Math.Max(maxX, x);
            minY = Math.Min(minY, y);
            maxY = Math.Max(maxY, y);
        }

        WriteLine($"({minX}, {minY}) - ({maxX}, {maxY})");
        foreach (var y in minY.To(maxY+1))
        {
            foreach (var x in minX.To(maxX+1))
            {
                Write(data.Contains((x, y)) ? '#' : '·');
            }
            WriteLine();
        }
        WriteLine();
    }

    public async Task<HashSet<(int x, int y)>> ReadInput(string filename)
    {
        var data = new HashSet<(int, int)>();
        var y = 0;
        await foreach(var line in System.IO.File.ReadLinesAsync(filename))
        {
            foreach (var x in 0.To(line.Length))
            {
                if (line[x] == '#') data.Add((x, y));
            }
            y++;
        }

        return data;
    }
}
