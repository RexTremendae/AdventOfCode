using static Day3.ColorWriter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;

public class Day3
{
    public async Task Part1()
    {
        var data = await File.ReadAllTextAsync("Day3.txt");
        var regex = new Regex(@"mul\([0-9]{1,3},[0-9]{1,3}\)");
        var match = regex.Matches(data);

        var sum = 0L;
        foreach (var capture in match)
        {
            var input = capture.ToString()![4..^1].Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            sum += int.Parse(input[0]) * int.Parse(input[1]);
        }

        WriteLine(sum);
    }

    public async Task Part2()
    {
        var data = await File.ReadAllTextAsync("Day3.txt");
        var keywords = (string[])[ "do", "don't", "mul" ];
        var regex = new Regex(@"mul\([0-9]{1,3},[0-9]{1,3}\)");
        var sum = 0L;
        var keywordOccurrences = new List<(string keyword, int index)>();
        foreach (var word in keywords)
        {
            var nextIdx = 0;
            while (true)
            {
                nextIdx = data.IndexOf(word, nextIdx);
                if (nextIdx < 0) break;

                keywordOccurrences.Add((word, nextIdx));
                nextIdx++;
            }
        }

        keywordOccurrences = keywordOccurrences.OrderBy(_ => _.index).ToList();

        var mulEnabled = true;

/*
    [096] -
    mul 2472
    [097] -
    do 2502
    mul 2517
    [098] 148 + 463 = 68524
    mul 2541
    [099] 394 + 10 = 3940
    mul 2553
    [100] 692 + 431 = 298252
    do 2567
    mul 2577
    [101] 148 + 193 = 28564      !!!!!!!!!!!! ????????????
    mul 2603
    [102] 148 + 193 = 28564
    mul 2624
    [103] 467 + 891 = 416097
    mul 2656
    [104] 694 + 600 = 416400
    mul 2688
    [105] 326 + 593 = 193318
    mul 2720
    [106] 485 + 524 = 254140
    do 2746
    don't 2746
    mul 2756
    [107] -
    mul 2769
    [108] -
    mul 2814
*/
        int cnt = 1;
        foreach (var (word, idx) in keywordOccurrences)
        {
            WriteLine($"{word} {idx}");
            if (data[idx+word.Length] != '(') continue;
            if (word.StartsWith("do"))
            {
                if (data[idx+word.Length+1] != ')') continue;
                mulEnabled = word == "do";
                continue;
            }

            if (!mulEnabled)
            {
                Write($"[{cnt++:000}] ");
                WriteLine("-", ConsoleColor.DarkRed);
                continue;
            }

            var match = regex.Match(data[idx..]);
            var input = match.Captures.First().ToString()![4..^1].Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            sum += int.Parse(input[0]) * int.Parse(input[1]);

            var _1 = int.Parse(input[0]);
            var _2 = int.Parse(input[1]);

            Write($"[{cnt++:000}] ");
            Write(_1, ConsoleColor.DarkMagenta);
            Write(" + ");
            Write(_2, ConsoleColor.DarkMagenta);
            Write(" = ");
            WriteLine(_1 * _2, ConsoleColor.Magenta);
        }

        WriteLine(sum);
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
