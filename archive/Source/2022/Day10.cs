using static System.Console;

public class Day10
{
    public async Task Part1()
    {
        var program = await ReadInput("Day10.txt");
        var cmd = "";
        var x = 1L;
        var ptr = 0;

        var cmdCycle = 0;
        var cmdArgs = Array.Empty<int>();
        var currCmdSize = 0;

        var sum = 0L;
        for(var cycle = 0; ; cycle++, cmdCycle++)
        {
            if (new[]{ 20, 60, 100, 140, 180, 220 }.Contains(cycle))
            {
                sum += cycle*x;
                //WriteLine($"{cycle}: {cycle*x} [{x}]");
            }

            if (cmd == "addx" && cmdCycle == 2)
            {
                var oldX = x;
                x += cmdArgs[0];
                //WriteLine($"[{cycle}] x: {oldX} -> {x}");
            }

            if (cmdCycle == currCmdSize)
            {
                if (ptr >= program.Count) break;
                cmdCycle = 0;
                cmd = program[ptr].cmd;
                cmdArgs = program[ptr].args;
                currCmdSize = cmd switch
                {
                    "noop" => 1,
                    "addx" => 2,
                    _ => 0
                };
                //WriteLine($"{ptr} [{cycle}]: {cmd} ({string.Join(", ", cmdArgs)}) | x: {x}");
                ptr++;
                continue;
            }
        }

        WriteLine(sum);
    }

    public async Task Part2()
    {
        var program = await ReadInput("Day10.txt");
        var cmd = "";
        var x = 1L;
        var ptr = 0;

        var cmdCycle = 0;
        var cmdArgs = Array.Empty<int>();
        var currCmdSize = 0;

        for(var cycle = 0; cycle < 6*40; cycle++, cmdCycle++)
        {
            if (cmd == "addx" && cmdCycle == 2)
            {
                var oldX = x;
                x += cmdArgs[0];
            }

            if (cycle % 40 == 0) WriteLine();
            var currX = cycle % 40;
            Write(Math.Abs(x - currX) < 2 ? "#" : " ");

            if (cmdCycle == currCmdSize)
            {
                if (ptr >= program.Count) break;
                cmdCycle = 0;
                cmd = program[ptr].cmd;
                cmdArgs = program[ptr].args;
                currCmdSize = cmd switch
                {
                    "noop" => 1,
                    "addx" => 2,
                    _ => 0
                };
                ptr++;
                continue;
            }
        }

        WriteLine();
        WriteLine();
    }

    private async Task<List<(string cmd, int[] args)>> ReadInput(string filename)
    {
        var program = new List<(string cmd, int[] args)>();
        var cmd = "";

        await foreach(var line in System.IO.File.ReadLinesAsync(filename))
        {
            var parts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries
                                      | StringSplitOptions.TrimEntries);
            cmd = parts[0];
            var argList = new List<int>();
            if (parts.Length > 1)
            {
                argList.Add(int.Parse(parts[1]));
            }

            program.Add((cmd, argList.ToArray()));
        }

        return program;
    }
}
