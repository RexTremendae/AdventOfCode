using static System.Console;

public class Day1
{
    public async Task Part1()
    {
        var max = 0;
        var currentElf = 0;

        await foreach(var line in File.ReadLinesAsync("Day1.txt"))
        {
            if (string.IsNullOrEmpty(line))
            {
                max = Math.Max(currentElf, max);
                currentElf = 0;
                continue;
            }

            currentElf += int.Parse(line);
        }

        WriteLine(max);
    }

    public async Task Part2()
    {
        List<int> elves = new();
        var currentElf = 0;

        await foreach(var line in File.ReadLinesAsync("Day1.txt"))
        {
            if (string.IsNullOrEmpty(line))
            {
                elves.Add(currentElf);
                currentElf = 0;
                continue;
            }

            currentElf += int.Parse(line);
        }

        elves.Sort();

        WriteLine (elves.TakeLast(3).Sum());
    }
}

