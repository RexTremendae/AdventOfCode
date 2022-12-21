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
