using static System.Console;

public partial class Day11
{
    public async Task Part1()
    {
        var filename = "Day11.txt";

        var input = await ReadInput(filename);

        var inspections = new Dictionary<int, long>();
        foreach (var key in input.Keys) inspections[key] = 0;

        foreach (var x in 0.To(20))
        {
            foreach (var (index, monkey) in input.OrderBy(_ => _.Key))
            {
                inspections[index] += monkey.Items.Count;
                foreach(var item in monkey.Items)
                {
                    var operand = monkey.Operand == 0 ? item : monkey.Operand;
                    var newItem = monkey.Operation switch
                    {
                        "*" => item*operand,
                        "+" => item+operand,
                        _ => throw new InvalidOperationException()
                    } / 3;

                    if (newItem % monkey.TestDivisibleBy == 0)
                    {
                        input[monkey.TestTrueThrowTo].Items.Add(newItem);
                    }
                    else
                    {
                        input[monkey.TestFalseThrowTo].Items.Add(newItem);
                    }
                }
                monkey.Items.Clear();
            }
        }
/*
        foreach (var key in input.Keys)
        {
            WriteLine($"Monkey {key}: {string.Join(", ", input[key].Items)}");
        }
*/
        var twoMost = inspections.Values.OrderByDescending(_ => _).Take(2).ToArray();
        WriteLine(twoMost[0] * twoMost[1]);

/*
        foreach (var key in input.Keys)
        {
            WriteLine($"Monkey {key}");
            var monkey = input[key];
            WriteLine($"  Starting items: {string.Join(", ", monkey.Items)}");
            WriteLine($"  Operation: {monkey.Operation} {monkey.Operand}");
            WriteLine($"  Test: / by {monkey.TestDivisibleBy}");
            WriteLine($"    True:  throw to {monkey.TestTrueThrowTo}");
            WriteLine($"    False: throw to {monkey.TestFalseThrowTo}");
        }
*/
    }

    public class Monkey
    {
        public List<long> Items { get; }
        public int TestDivisibleBy { get; }
        public int TestTrueThrowTo { get; }
        public int TestFalseThrowTo { get; }
        public string Operation { get; } = "";
        public int Operand { get; }

        public Monkey(IEnumerable<(string, string)> data)
        {
            Items = new();
            foreach (var (label, args) in data)
            {
                switch (label)
                {
                    case "Starting items":
                        Items.AddRange(args.Split(",").Select(_ => long.Parse(_)));
                        break;

                    case "Test":
                        TestDivisibleBy = int.Parse(args.Split(" ").Last());
                        break;

                    case "If true":
                        TestTrueThrowTo = int.Parse(args.Split(" ").Last());
                        break;

                    case "If false":
                        TestFalseThrowTo = int.Parse(args.Split(" ").Last());
                        break;

                    case "Operation":
                        var operation = args.Split("=")[1];
                        var parts = operation.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                        Operation = parts[1];
                        Operand = parts[2] == "old" ? 0 : int.Parse(parts[2]);
                        break;
                }
            }
        }
    }

    public async Task<Dictionary<int, Monkey>> ReadInput(string filename)
    {
        var currentMonkey = 0;
        var monkeyRows = new Dictionary<int, List<(string, string)>>();

        await foreach(var line in System.IO.File.ReadLinesAsync(filename))
        {
            if (string.IsNullOrEmpty(line)) continue;
            var parts = line.Split(":", StringSplitOptions.TrimEntries);

            var label = parts[0];
            var args = parts[1];

            if (label.StartsWith("Monkey"))
            {
                currentMonkey = label.Last() - '0';
                monkeyRows.Add(currentMonkey, new());
            }
            else
            {
                monkeyRows[currentMonkey].Add((label, args));
            }
        }

        return monkeyRows.ToDictionary(_ => _.Key, _ => new Monkey(_.Value));
    }
}
