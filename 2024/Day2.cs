using static Day1.ColorWriter;

using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

public class Day2
{
    public async Task Part1()
    {
        var lines = await File.ReadAllLinesAsync("Day2.txt");

        var sum = 0;

        foreach (var ln in lines)
        {
            var data = ln
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(_ => int.Parse(_))
                .ToArray();

            if (IsSafe(data)) sum ++;
        }

        WriteLine(sum);
    }

    public async Task Part2()
    {
        var lines = await File.ReadAllLinesAsync("Day2.txt");

        var sum = 0;

        foreach (var ln in lines)
        {
            var data = ln
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(_ => int.Parse(_))
                .ToArray();

            if (IsSafe(data))
            {
                sum ++;
            }
            else
            {
                for (int i = 0; i < data.Length; i++)
                {
                    if (IsSafe([..data[..i].Concat(data[(i+1)..])]))
                    {
                        sum ++;
                        break;
                    }
                }
            }
        }

        WriteLine(sum);
    }

    private bool IsSafe(int[] data)
    {
        var dir = int.Sign(data[1] - data[0]);

        for (int i = 1; i < data.Length; i++)
        {
            var _1 = data[i-1];
            var _2 = data[i];

            var diff = int.Abs(_1 - _2);
            if (diff < 1 || diff > 3) return false;
            if (int.Sign(_2 - _1) != dir) return false;
        }

        return true;
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