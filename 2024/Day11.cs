using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using static System.Console;

public class Day11
{
    public async Task Part1()
    {
        var stones = await ReadDataAsync("Day11.txt");

        for (int i = 0; i < 25; i++)
        {
            var next = new List<string>();
            foreach (var s in stones)
            {
                if (s == "0")
                {
                    next.Add("1");
                    continue;
                }

                var half = s.Length / 2;
                if (s.Length % 2 == 0)
                {

                    next.Add(s[..half]);
                    var secondHalf = s[half..].TrimStart('0');
                    if (string.IsNullOrWhiteSpace(secondHalf)) secondHalf = "0";
                    next.Add(secondHalf);
                    continue;
                }

                var val = long.Parse(s);
                next.Add((val*2024).ToString());
            }

            stones = [..next];
        }

        WriteLine(stones.Length);
    }

    private async Task<string[]> ReadDataAsync(string filename)
    {
        return (await File.ReadAllLinesAsync(filename))
            .First()
            .Trim()
            .Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToArray();
    }
}
