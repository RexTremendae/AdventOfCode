using static System.Console;
using ReactionLookup = System.Collections.Generic.Dictionary<string,
    (long amount, System.Collections.Generic.IEnumerable<(string chemical, long amount)> parts)>;

await new Day14().Part2();

public class Day14
{
    Dictionary<string, long> _resources = new();
    ReactionLookup _reactions = new();

    bool _traceOutput = false;

    public async Task Part2()
    {
        _reactions = await ParseInput("Day14.txt");

        long min = 1;
        long max = 1_000_000_000;
        var goal = 1_000_000_000_000;

        long n = min + (max - min) / 2;

        var last_below_goal = 1L;

        while (Math.Abs(min - max) > 1)
        {
            var sln = Solve(n);
            if (sln < goal) last_below_goal = n;
            WriteLine($"{n}: {sln}");
            if (sln > goal)
            {
                max = n;
                n = min + (n - min) / 2;
            }
            else
            {
                min = n;
                n = n + (max - n) / 2;
            }
        }

        var _out = $"  {last_below_goal.ToString()}  ";
        WriteLine();
        WriteLine("".PadLeft(_out.Length, '='));
        WriteLine($"  {last_below_goal}");
        WriteLine("".PadLeft(_out.Length, '='));
    }

    private long Solve(long n)
    {
        _resources.Clear();

        if (_traceOutput)
        {
            PrintReactions(_reactions);
        }

        var requiredResources = new List<(string chemical, long amount)>();
        requiredResources.Add(("FUEL", n));

        foreach (var chm in _reactions.Keys.ToHashSet())
        {
            _resources.Add(chm, 0);
        }

        var ore = 0L;
        while (requiredResources.Any())
        {
            var (from_ch, from_amnt) = requiredResources.First();
            TraceWrite($"{from_amnt} {from_ch}");
            requiredResources.RemoveAt(0);

            var react = _reactions[from_ch];

            var from_resrc = _resources[from_ch];
            var multiplier = 0L;
            if (from_resrc < from_amnt)
            {
                multiplier = (from_amnt - from_resrc) / react.amount;
                if ((from_amnt - from_resrc) % react.amount != 0) multiplier++;
            }

            foreach (var (p_ch, p_amnt) in react.parts)
            {
                TraceWrite($" => {p_ch}", false);

                var to_amnt = p_amnt*multiplier;
                TraceWrite($"  x{multiplier} ({to_amnt})");

                if (p_ch == "ORE")
                {
                    TraceWrite($"  ORE: {ore} + {to_amnt}", false);
                    ore += to_amnt;
                    TraceWrite($" -> {ore}");
                }
                else if (to_amnt > 0)
                {
                    TraceWrite($"  REQ: {to_amnt} {p_ch}");
                    requiredResources.Add((p_ch, to_amnt));
                }

                TraceWrite($"  {from_ch}: {_resources[from_ch]}", false);
                _resources[from_ch] = react.amount*multiplier + from_resrc - from_amnt;
                TraceWrite($" -> {_resources[from_ch]}");
            }
        }

        return ore;
    }

    private void PrintReactions(Dictionary<string, (long amount, IEnumerable<(string chemical, long amount)> parts)> reactions)
    {
        var display =
        (Action<KeyValuePair<string,
        (long amount, IEnumerable<(string chemical, long amount)> parts)>>)
        (_ =>
        {
            var parts = string.Join(", ", _.Value.parts.Select(_2 => $"{_2.amount} {_2.chemical}"));
            WriteLine($"{_.Value.amount} {_.Key} requires {parts}");
        });

        display(reactions.Single(_ => _.Key == "FUEL"));
        WriteLine();

        foreach (var _ in reactions
            .Where(_ => _.Key != "FUEL")
            .Where(_ => _.Value.parts.Count() != 1 || _.Value.parts.First().chemical != "ORE")
            .OrderBy(_ => _.Key))
        {
            display(_);
        }

        WriteLine();

        foreach (var _ in reactions
            .Where(_ => _.Value.parts.Count() == 1 && _.Value.parts.First().chemical == "ORE")
            .OrderBy(_ => _.Key))
        {
            display(_);
        }

        WriteLine();
        WriteLine("------------------------------");
        WriteLine();
    }

    private async Task<ReactionLookup> ParseInput(string filename)
    {
        var reactions = new ReactionLookup();
        await foreach(var line in File.ReadLinesAsync(filename))
        {
            if (string.IsNullOrEmpty(line)) continue;
            var splitIdx = line.IndexOf("=>");
            if (splitIdx < 0) continue;

            var leftSide = line[..splitIdx].Trim();
            var rightSide = line[(splitIdx+2)..].Trim();

            var (chemical, amount) = ParseComponent(rightSide);
            reactions.Add(chemical, (amount, leftSide.Split(",").Select(_ => ParseComponent(_))));
        }

        return reactions;
    }

    private (string chemical, long amount) ParseComponent(string rightSide)
    {
        var parts = rightSide.Trim().Split(" ");
        return (parts[1], long.Parse(parts[0]));
    }

    private void TraceWrite(object data, bool writeLine = true)
    {
        if (!_traceOutput) return;

        if (writeLine)
        {
            WriteLine(data);
        }
        else
        {
            Write(data);
        }
    }
}