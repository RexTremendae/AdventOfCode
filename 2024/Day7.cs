using static Day7.ColorWriter;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;

public class Day7
{
    public async Task Part1()
    {
        var equations = await ReadIndata("Day7.txt");

        var sum = 0L;
        foreach (var eq in equations)
        {
            var validation = Validate(eq.result, eq.operands, includeConcatenation: false);
            if (string.IsNullOrEmpty(validation))
            {
                continue;
            }

            sum += eq.result;
        }

        WriteLine(sum);
    }

    public async Task Part2()
    {
        var equations = await ReadIndata("Day7.txt");

        var sum = 0L;
        foreach (var eq in equations)
        {
            var validation = Validate(eq.result, eq.operands, includeConcatenation: true);
            if (string.IsNullOrEmpty(validation))
            {
                continue;
            }

            sum += eq.result;
        }

        WriteLine(sum);
    }

    private string Validate(long result, int[] operands, bool includeConcatenation)
    {
        return ValidateRecursive(result, operands[0], operands[1..], operands[0].ToString(), includeConcatenation);
    }

    private string ValidateRecursive(long expectedResult, long resultSoFar, int[] operands, string evaluation, bool includeConcatenation)
    {
        if (resultSoFar > expectedResult) return string.Empty;

        if (!operands.Any())
        {
            if (expectedResult != resultSoFar) return string.Empty;
            return $"{evaluation} = {expectedResult}";
        }

        var multi = ValidateRecursive(expectedResult, resultSoFar*operands[0], operands[1..], $"{evaluation} * {operands[0]}", includeConcatenation);
        if (!string.IsNullOrEmpty(multi))
        {
            return multi;
        }

        var add = ValidateRecursive(expectedResult, resultSoFar+operands[0], operands[1..], $"{evaluation} + {operands[0]}", includeConcatenation);
        if (!string.IsNullOrEmpty(add))
        {
            return add;
        }

        if (includeConcatenation)
        {
            var concat = ValidateRecursive(expectedResult, long.Parse($"{resultSoFar}{operands[0]}"), operands[1..], $"{evaluation} || {operands[0]}", includeConcatenation);
            if (!string.IsNullOrEmpty(concat))
            {
                return concat;
            }
        }

        return string.Empty;
    }

    private async Task<List<(long result, int[] operands)>> ReadIndata(string inputFile)
    {
        var resultList = new List<(long, int[])>();

        foreach (var line in await File.ReadAllLinesAsync(inputFile))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var split = line.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var result = long.Parse(split[0]);
            var operands = split[1]
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(_ => int.Parse(_))
                .ToArray();

            resultList.Add((result, operands));
        }

        return resultList;
    }

    public static class ColorWriter
    {
        public static void Write(object? text = null, ConsoleColor? color = null)
        {
            if (!color.HasValue)
            {
                Console.Write(text);
                return;
            }

            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color.Value;
            Console.Write(text);
            Console.ForegroundColor = oldColor;
        }

        public static void WriteLine(object? text = null, ConsoleColor? color = null)
        {
            Write(text, color);
            Console.WriteLine();
        }
    }
}
