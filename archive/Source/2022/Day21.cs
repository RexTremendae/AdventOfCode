using System.Text;
using static System.Console;

public class Day21
{
    public async Task Part1()
    {
        await Task.CompletedTask;

        var monkeyYells = new Dictionary<string, long>();

        var monkeyData =
        await ReadInput("Day21.txt")
        .ToListAsync();

        while (monkeyData.Any())
        {
            var data = monkeyData.First();
            monkeyData.RemoveAt(0);

            if (data.number != null)
            {
                monkeyYells[data.id] = data.number.Value;
                continue;
            }

            if (monkeyYells.TryGetValue(data.operand1!, out var operand1) && monkeyYells.TryGetValue(data.operand2!, out var operand2))
            {
                monkeyYells[data.id] = data.operation switch
                {
                    '+' => operand1 + operand2,
                    '-' => operand1 - operand2,
                    '*' => operand1 * operand2,
                    '/' => operand1 / operand2,
                    _ => throw new InvalidOperationException(data.operation.ToString())
                };
            }
            else
            {
                monkeyData.Add(data);
            }
        }

        // Example: root yells 152
        WriteLine(monkeyYells["root"]);
    }

    public abstract class Node
    {
        public Node(string id)
        {
            Id = id;
        }

        public string Id { get; }
        public abstract Node Simplify();
    }

    public class DataNode : Node
    {
        public DataNode(string id, long data) : base(id)
        {
            Data = data;
        }

        public long Data { get; }

        public override Node Simplify()
        {
            return this;
        }

        public override string ToString()
        {
            return Data.ToString();
        }
    }

    public class HumanNode : Node
    {
        public HumanNode() : base("humn")
        {}

        public override Node Simplify()
        {
            return this;
        }

        public override string ToString()
        {
            return "humn";
        }
    }

    public class CalculationNode : Node
    {
        public CalculationNode(string id, char operation, Node operand1, Node operand2) : base(id)
        {
            Operation = operation;
            Operand1 = operand1;
            Operand2 = operand2;
        }

        public char Operation { get; }
        public Node Operand1 { get; private set; }
        public Node Operand2 { get; private set; }

        public override Node Simplify()
        {
            Operand1 = Operand1.Simplify();
            Operand2 = Operand2.Simplify();

            if (Operand1 is DataNode data1 && Operand2 is DataNode data2)
            {
                return new DataNode(Id, Operation switch{
                    '+' => data1.Data + data2.Data,
                    '-' => data1.Data - data2.Data,
                    '*' => data1.Data * data2.Data,
                    '/' => data1.Data / data2.Data
                });
            }
            return this;
        }

        public override string ToString()
        {
            return $"({Operand1.ToString()} {Operation} {Operand2.ToString()})";
        }
    }

    private Dictionary<string, (string id, long? number, char? operation, string? operand1, string? operand2)> _monkeyYells = new();
    public async Task Part2()
    {
        checked
        {
        _monkeyYells =
        await ReadInput("Day21.txt")
        .ToDictionaryAsync(key => key.id, value => value);

        WriteLine("Left side:");
        var leftSide = BuildExpression(_monkeyYells["root"].operand1!);
        WriteLine(leftSide.ToString());
        WriteLine("------------------");
        leftSide = leftSide.Simplify();
        WriteLine(leftSide.ToString());

        WriteLine();

        WriteLine("Right side:");
        var rightSide = BuildExpression(_monkeyYells["root"].operand2!);
        WriteLine(rightSide.ToString());
        WriteLine("------------------");
        rightSide = rightSide.Simplify();
        WriteLine(rightSide.ToString());
        WriteLine();

        WriteLine("==================");

        WriteLine();

        var rightVal = ((DataNode)rightSide).Data;

        WriteLine(leftSide.ToString());
        WriteLine("------------------");
        WriteLine(rightVal);
        WriteLine();

        for (;;)
        {
            if (leftSide is HumanNode)
            {
                WriteLine($"humn = {rightVal}");
                break;
            }

            var calcNode = (CalculationNode)leftSide;

            if (calcNode.Operand2 is DataNode dataNode2)
            {
                rightVal = calcNode.Operation switch
                {
                    '-' => rightVal + dataNode2.Data,
                    '+' => rightVal - dataNode2.Data,
                    '*' => rightVal / dataNode2.Data,
                    '/' => rightVal * dataNode2.Data
                };
                leftSide = calcNode.Operand1;
            }
            else if (calcNode.Operand1 is DataNode dataNode1)
            {
                switch (calcNode.Operation)
                {
                    case '+':
                        rightVal -= dataNode1.Data;
                        break;

                    case '-':
                        rightVal = dataNode1.Data - rightVal;
                        break;

                    case '*':
                        if (rightVal % dataNode1.Data != 0)
                            throw new NotImplementedException("*");

                        rightVal /= dataNode1.Data;
                        break;

                    case '/':
                        throw new NotImplementedException("/");
                }
                leftSide = calcNode.Operand2;
            }

            WriteLine(leftSide.ToString());
            WriteLine("------------------");
            WriteLine(rightVal);
            WriteLine();
        }
        }
    }

    private Node BuildExpression(string id)
    {
        if (id == "humn")
        {
            return new HumanNode();
        }

        var yell = _monkeyYells[id];
        if (yell.number.HasValue)
        {
            return new DataNode(id, yell.number.Value);
        }

        var operand1 = BuildExpression(_monkeyYells[yell.operand1!].id);
        var operand2 = BuildExpression(_monkeyYells[yell.operand2!].id);

        return new CalculationNode(yell.id, yell.operation!.Value, operand1, operand2);
    }

    public async IAsyncEnumerable<(string id, long? number, char? operation, string? operand1, string? operand2)> ReadInput(string filename)
    {
        await foreach(var line in System.IO.File.ReadLinesAsync(filename))
        {
            if (string.IsNullOrEmpty(line)) continue;
            var tmp = line.Split(":", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var id = tmp[0];
            var data = tmp[1].Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (data.Length == 1)
            {
                yield return (id, long.Parse(data[0]), null, null, null);
                continue;
            }
            else if (data.Length != 3)
            {
                throw new InvalidOperationException();
            }
            yield return (id, null, data[1][0], data[0], data[2]);
        }
    }
}
