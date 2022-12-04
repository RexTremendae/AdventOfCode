using static System.Console;

public class Day4
{
    public async Task Part1()
    {
        int sum = 0;
        int cnt = 0;

        await foreach(var line in File.ReadLinesAsync("Day4.txt"))
        {
            if (string.IsNullOrEmpty(line)) continue;

            var pairs = line.Split(",").Select(_ => ParseInterval(_)).ToArray();
            if (pairs[0].first > pairs[1].first
                || (pairs[0].first == pairs[1].first && pairs[0].last < pairs[1].last))
            {
                var tmp = pairs[1];
                pairs[1] = pairs[0];
                pairs[0] = tmp;
            }

            if (pairs[0].last >= pairs[1].last)
            {
                //WriteLine($"[{cnt}] {pairs[0]} {pairs[1]}");
                sum++;
            }
            cnt++;
        }

        WriteLine(sum);
    }

    public async Task Part2()
    {
        int sum = 0;
        int cnt = 0;

        await foreach(var line in File.ReadLinesAsync("Day4.txt"))
        {
            if (string.IsNullOrEmpty(line)) continue;

            var pairs = line.Split(",").Select(_ => ParseInterval(_)).ToArray();
            if (pairs[0].first > pairs[1].first)
            {
                var tmp = pairs[1];
                pairs[1] = pairs[0];
                pairs[0] = tmp;
            }

            if (pairs[0].last >= pairs[1].first)
            {
                //WriteLine($"[{cnt}] {pairs[0]} {pairs[1]}");
                sum++;
            }
            cnt++;
        }

        WriteLine(sum);
    }

    private (int first, int last) ParseInterval(string input)
    {
        var parts = input.Split("-");
        return (int.Parse(parts[0]), int.Parse(parts[1]));
    }
}
