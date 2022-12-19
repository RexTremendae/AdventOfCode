using static System.Console;

public partial class Day11
{
    public async Task Part2()
    {
        checked {
        var filename = "Day11_small.txt";

        var input = await ReadInput2(filename);

        var inspections = new Dictionary<int, long>();
        foreach (var (key, value) in input)
        {
            inspections[key] = 0;
        }

        foreach (var x in 0.To(20))
        {
            foreach (var (mkyIdx, monkey) in input.OrderBy(_ => _.Key))
            {
                inspections[mkyIdx] += monkey.Items.Count;
                foreach(var itmIdx in 0.To(monkey.Items.Count))
                {
                    var item = monkey.Items[itmIdx];
                    var operand = monkey.Operand == 0 ? item : new BigNumber(monkey.Operand);

                    if (monkey.Operation == "+")
                    {
                        item.Add(operand);
                    }
                    else if (monkey.Operation == "*")
                    {
                        item.Multiply(operand);
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }

                    var isDivisible = item.IsDivisibleBy(monkey.TestDivisibleBy);

                    WriteLine($"Monkey {mkyIdx}");
                    var itemParts = item.ToString().Split("+", StringSplitOptions.TrimEntries);
                    foreach (var idx in 0.To(itemParts.Length))
                    {
                        WriteLine($"  {itemParts[idx]}{(idx != itemParts.Length-1 ? " +" : "")}");
                    }
                    WriteLine($"  is / by {monkey.TestDivisibleBy}: {isDivisible}");
                    WriteLine();

                    if (isDivisible)
                    {
                        input[monkey.TestTrueThrowTo].Items.Add(item);
                    }
                    else
                    {
                        input[monkey.TestFalseThrowTo].Items.Add(item);
                    }
                }
                monkey.Items.Clear();
            }

            foreach (var key in input.Keys)
            {
                WriteLine($"Monkey {key}: {inspections[key]} inspections");
            }
            WriteLine();
        }

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
        } // checked
    }

    public class BigNumber
    {
        private List<List<int>> _factors;

        public BigNumber(int value)
        {
            _factors = new();
            _factors.Add(Factorize(value).ToList());
        }

        public void Add(BigNumber item)
        {
            _factors.AddRange(item._factors.Select(_ => _.ToList()));
        }

        public void Multiply(BigNumber item)
        {
            var _1f = _factors.ToList();
            var _2f = item._factors.ToList();

            _factors.Clear();

            foreach (var _1 in _1f)
            foreach (var _2 in _2f)
            {
                var factors = new List<int>(_1.ToList());
                factors.AddRange(_2);
                _factors.Add(factors);
            }
        }

        public bool IsDivisibleBy(int value)
        {
            foreach (var factors in _factors)
            {
                if (!factors.Any(_ => _ == value)) return false;
            }

            return true;
        }

        private int[] _primes = new[] {
            2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41,
            43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97
        };

        private IEnumerable<int> Factorize(long value)
        {
            var idx = 0;
            while (value > 1)
            {
                var prime = _primes[idx];
                while (value % prime == 0)
                {
                    yield return prime;
                    value /= prime;
                }
                idx++;
            }
        }

        public override string ToString()
        {
            return string.Join(" + ", _factors.Select(_ => $"({string.Join(" * ", _)})"));
        }
    }

    public class Monkey2
    {
        public List<BigNumber> Items { get; }
        public int TestDivisibleBy { get; }
        public int TestTrueThrowTo { get; }
        public int TestFalseThrowTo { get; }
        public string Operation { get; } = "";
        public int Operand { get; }

        public Monkey2(IEnumerable<(string, string)> data)
        {
            Items = new();
            foreach (var (label, args) in data)
            {
                switch (label)
                {
                    case "Starting items":
                        Items.AddRange(args.Split(",").Select(_ => new BigNumber(int.Parse(_))));
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

    public async Task<Dictionary<int, Monkey2>> ReadInput2(string filename)
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

        return monkeyRows.ToDictionary(_ => _.Key, _ => new Monkey2(_.Value));
    }
}
