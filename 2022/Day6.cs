using static System.Console;

public class Day6
{
    public async Task Part1()
    {
        var input = await File.ReadAllTextAsync("Day6.txt");
        var x = 3;
        var str = " " + input[..3];

        while (x < input.Length)
        {
            str = str[1..] + input[x];
            x++;
            if (str.ToHashSet().Count() == 4) break;
        }
        WriteLine(x);
    }

    public async Task Part2()
    {
        await Task.CompletedTask;
        var input = await File.ReadAllTextAsync("Day6.txt");
        var len = 14;
        var x = len-1;
        var str = (" " + input[..(len-1)]).ToCharArray().ToList();

        while (x < input.Length)
        {
            str.RemoveAt(0);
            str.Add(input[x]);
            x++;
            if (str.ToHashSet().Count() == len) break;
        }
        WriteLine(x);
    }
}
