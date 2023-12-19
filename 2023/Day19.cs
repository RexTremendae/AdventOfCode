using static ColorWriter;
using Workflow = (string name, Day19.Rule[] rules);
using Part = System.Collections.Generic.Dictionary<char, int>;

public class Day19
{
    public async Task Part1()
    {
        var (workflows, parts) = await ReadData("Day19.txt");

        var sum = 0L;
        foreach (var p in parts)
        {
            var current = "in";
            var match = false;

            while (!(new[] { "A", "R" }.Contains(current)))
            {
                foreach (var step in workflows[current])
                {
                    if (step.Operator == '>' && p[step.Category] > step.CompareValue)
                    {
                        current = step.Next;
                        match = true;
                        break;
                    }

                    if (step.Operator == '<' && p[step.Category] < step.CompareValue)
                    {
                        current = step.Next;
                        match = true;
                        break;
                    }

                    if (step.Operator == ' ')
                    {
                        current = step.Next;
                        match = true;
                        break;
                    }
                }

                if (!match) throw new InvalidOperationException();
            }

            if (current == "A")
            {
                sum += p.Sum(_ => _.Value);
            }
        }

        WriteLine(sum);
    }

    private async Task<(Dictionary<string, Rule[]>, Part[])> ReadData(string filepath)
    {
        var readWorkflows = true;

        var workflowList = new List<Workflow>();
        var partList = new List<Part>();

        foreach (var line in await File.ReadAllLinesAsync(filepath))
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                if (!readWorkflows) break;

                readWorkflows = false;
                continue;
            }

            if (readWorkflows)
            {
                var ruleList = new List<Rule>();
                var bracketStartIdx = line.IndexOf('{');
                var name = line[..bracketStartIdx];
                var rules = line[(bracketStartIdx+1)..^1].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                foreach (var _ in rules)
                {
                    Rule rule;
                    var split = _.Split(':');
                    if (split.Length == 2)
                    {
                        var cat = split[0][0];
                        var op = split[0][1];
                        var val = int.Parse(split[0][2..]);
                        var next = split[1];

                        rule = new Rule(op, cat, val, next);
                    }
                    else
                    {
                        rule = new Rule(' ', ' ', 0, split[0]);
                    }
                    ruleList.Add(rule);
                }

                workflowList.Add((name, ruleList.ToArray()));
            }
            else
            {
                var part = new Part();
                foreach (var _ in line[1..^1].Split(','))
                {
                    var split = _.Split('=');
                    part.Add(split[0][0], int.Parse(split[1]));
                }
                partList.Add(part);
            }
        }

        return (workflowList.ToDictionary(key => key.name, value => value.rules), partList.ToArray());
    }

    public readonly record struct Rule
    (
        char Operator,
        char Category,
        int CompareValue,
        string Next
    );
}
