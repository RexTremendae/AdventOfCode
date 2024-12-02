using static Day1.ColorWriter;

using System;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class Day1
{
    public async Task Part1()
    {
        var lines = await File.ReadAllLinesAsync("Day1.txt");
        var regex = new Regex(@"([0-9]+)\s+([0-9]+)");

        var list1 = new List<long>();
        var list2 = new List<long>();

        foreach (var line in lines)
        {
            var match = regex.Match(line);

            var _1 = long.Parse(match.Groups[1].Value);
            var _2 = long.Parse(match.Groups[2].Value);

            list1.Add(_1);
            list2.Add(_2);
        }

        list1.Sort();
        list2.Sort();

        var sum = 0L;

        for (int i = 0; i < list1.Count; i++)
        {
            sum += long.Abs(list1[i] - list2[i]);
        }

        WriteLine(sum);
    }

    public async Task Part2()
    {
        var lines = await File.ReadAllLinesAsync("Day1.txt");
        var regex = new Regex(@"([0-9]+)\s+([0-9]+)");

        var inputList = new List<long>();
        var lookup = new Dictionary<long, long>();

        foreach (var line in lines)
        {
            var match = regex.Match(line);

            var _1 = long.Parse(match.Groups[1].Value);
            var _2 = long.Parse(match.Groups[2].Value);

            if (!lookup.ContainsKey(_1))
            {
                lookup.Add(_1, 0);
            }

            if (!lookup.TryGetValue(_2, out var count))
            {
                count = 0;
            }

            count++;
            lookup[_2] = count;

            inputList.Add(_1);
        }

        var sum = 0L;

        foreach (var input in inputList)
        {
            sum += input * lookup[input];
        }

        WriteLine(sum);
    }

    public static class ColorWriter
    {
        public static void Write(object? text, ConsoleColor? color = null)
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

        public static void WriteLine(object? text, ConsoleColor? color = null)
        {
            Write(text, color);
            Console.WriteLine();
        }
    }
}