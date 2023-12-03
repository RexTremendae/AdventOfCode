using System.Numerics;
using static ColorWriter;

public class Day3
{
    public async Task Part1()
    {
        var schema = await ReadData("Day3.txt");

        var sum = 0;
        foreach (var y in 1.To(schema.Length - 1))
        {
            var line = schema[y];
            var x = 1;
            for(;;)
            {
                while (x < line.Length && !char.IsDigit(line[x])) x++;
                if (x >= line.Length) break;
                var numStart = x;

                while (char.IsDigit(line[x])) x++;
                var numLength = x - numStart;
                var isEnginePart = schema[y][numStart-1] != '.' || schema[y][numStart+numLength] != '.';

                foreach (var xx in (numStart-1).To(numStart+numLength+1))
                {
                    isEnginePart |= schema[y-1][xx] != '.';
                    isEnginePart |= schema[y+1][xx] != '.';
                }

                if (isEnginePart)
                {
                    var partNumber = int.Parse(schema[y][numStart..(numStart+numLength)]);
                    sum += partNumber;
                    WriteLine(partNumber);
                }
            }
        }

        WriteLine(sum, ConsoleColor.White);
    }

    public async Task Part2()
    {
        var debug = false;
        var schema = await ReadData("Day3.txt");

        var sum = new BigInteger();
        foreach (var y in 1.To(schema.Length - 1))
        {
            var line = schema[y];
            var x = 1;
            for(;;)
            {
                while (x < line.Length && line[x] != '*') x++;
                if (x >= line.Length) break;

                var partNumbers = new List<int>();

                foreach (var yy in (y-1).To(y+2))
                foreach (var xx in (x-1).To(x+2))
                {
                    var partNumber = ExtractPartNumber(schema, xx, yy);
                    if (partNumber >= 0)
                    {
                        if (debug)
                        {
                            Write($"({xx}, {yy}) ", ConsoleColor.DarkGray);
                            WriteLine(partNumber);
                        }
                        partNumbers.Add(partNumber);
                    }

                    if (partNumber >= 0 && (xx > x-1 || char.IsDigit(schema[yy][x]))) break;
                }

                if (partNumbers.Count == 2)
                {
                    if (debug)
                    {
                        Write(partNumbers[0], ConsoleColor.Cyan);
                        Write(" * ");
                        WriteLine(partNumbers[1], ConsoleColor.Cyan);
                    }
                    sum += partNumbers[0] * partNumbers[1];
                }
                x++;
            }
        }

        WriteLine(sum, ConsoleColor.White);
    }

    private int ExtractPartNumber(string[] schema, int x, int y)
    {
        if (!char.IsDigit(schema[y][x])) return -1;

        while (char.IsDigit(schema[y][x])) x--;
        x++;
        var start = x;

        while (char.IsDigit(schema[y][x])) x++;
        return int.Parse(schema[y][start..x]);
    }

    private async Task<string[]> ReadData(string filename)
    {
        var rawLines = new List<string>();
        foreach (var line in await File.ReadAllLinesAsync(filename))
        {
            if (string.IsNullOrEmpty(line)) continue;
            rawLines.Add(line);
        }

        var emptyLine = "".PadLeft(rawLines.First().Length, '.');
        var lines = new List<string>();

        lines.Add(emptyLine);
        lines.AddRange(rawLines.Select(_ => $".{_}."));
        lines.Add(emptyLine);

        return lines.ToArray();
    }
}
