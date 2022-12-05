using static System.Console;

public class Day5
{
    public async Task Part1()
    {
        var (stacks, moves) = await ParseInput("Day5.txt");
        foreach (var (from, to, count) in moves)
        {
            for (int _ = 0; _ < count; _++)
            {
                stacks[to].Push(stacks[from].Pop());
            }
        }

        foreach (var (_, stack) in stacks.OrderBy(_ => _.Key))
        {
            Write(stack.Peek());
        }
        WriteLine();
    }

    public async Task Part2()
    {
        var (stacks, moves) = await ParseInput("Day5.txt");
        foreach (var (from, to, count) in moves)
        {
            var crates = new List<char>();
            for (int _ = 0; _ < count; _++)
            {
                crates.Add(stacks[from].Pop());
            }
            crates.Reverse();
            foreach (var cr in crates)
            {
                stacks[to].Push(cr);
            }
        }

        foreach (var (_, stack) in stacks.OrderBy(_ => _.Key))
        {
            Write(stack.Peek());
        }
        WriteLine();
    }

    private async Task<(Dictionary<char, Stack<char>> stacks, List<(char from, char to, int count)> moves)> ParseInput(string filename)
    {
        var initalStateRows = new List<string>();
        var moveListRows = new List<string>();
        var isInitialState = true;

        await foreach(var line in File.ReadLinesAsync(filename))
        {
            if (string.IsNullOrEmpty(line))
            {
                isInitialState = false;
                continue;
            }

            if (isInitialState)
            {
                initalStateRows.Add(line);
            }
            else
            {
                moveListRows.Add(line);
            }
        }

        var labels = new List<(int, char)>();
        var stacks = new Dictionary<char, Stack<char>>();
        var moves = new List<(char from, char to, int count)>();

        var idx = -1;
        foreach (var ch in initalStateRows.Last())
        {
            idx++;
            if (ch == ' ') continue;
            labels.Add((idx, ch));
            stacks.Add(ch, new());
        }

        foreach (var line in initalStateRows.SkipLast(1).Reverse())
        {
            foreach (var (lbl_idx, lbl) in labels)
            {
                if (line[lbl_idx] == ' ') continue;
                stacks[lbl].Push(line[lbl_idx]);
            }
        }

        foreach (var line in moveListRows)
        {
            var parts = line.Split(" ");
            moves.Add((parts[3][0], parts[5][0], int.Parse(parts[1])));
        }

        return (stacks, moves);
    }
}
