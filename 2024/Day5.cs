using static Day5.ColorWriter;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System;

public class Day5
{
    public async Task Part1()
    {
        var (rules, updates) = await ReadIndata("Day5.txt");

        var sum = 0L;
        foreach (var upd in updates)
        {
            var allOk = true;
            foreach (var r in rules)
            {
                var r1 = -1;
                var r2 = -1;

                for (var uIdx = 0; uIdx < upd.Length; uIdx++)
                {
                    if (upd[uIdx] == r._1)
                    {
                        if (r1 >= 0)
                        {
                            throw new InvalidDataException();
                        }

                        r1 = uIdx;
                    }
                    if (upd[uIdx] == r._2)
                    {
                        if (r2 >= 0)
                        {
                            throw new InvalidDataException();
                        }

                        r2 = uIdx;
                    }
                }

                if (r1 == -1 || r2 == -1) continue;
                if (r1 > r2)
                {
                    allOk = false;
                    break;
                }
            }

            if (allOk)
            {
                sum += upd[upd.Length/2];
            }
        }

        WriteLine(sum);
    }

    private async Task<((int _1, int _2)[] rules, int[][] updates)> ReadIndata(string filename)
    {
        var lines = await File.ReadAllLinesAsync(filename);
        var rules = lines
            .TakeWhile(_ => !string.IsNullOrWhiteSpace(_))
            .Select(_ => _.Split('|').Select(_ => (int.Parse(_))))
            .Select(_ => (_.First(), _.Skip(1).First()))
            .ToArray();

        var updates = lines
            .SkipWhile(_ => !string.IsNullOrWhiteSpace(_))
            .Skip(1)
            .Select(_ => _.Split(','))
            .Select(_ => _.Select(_ => int.Parse(_)).ToArray())
            .ToArray();

        return (rules, updates);
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
