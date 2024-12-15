using static Day14.ColorWriter;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;

public class Day14
{
    public async Task Part1()
    {
        var gridWidth = 101;
        var gridHeight = 103;

        var bots = await ReadIndata("Day14.txt");

        for (int i = 0; i < 100; i++)
        {
            MoveBots(bots, gridWidth, gridHeight);
        }

        var q = (long[])[0, 0, 0, 0];

        var mX = gridWidth / 2;
        var mY = gridHeight / 2;

        for (var bIdx = 0; bIdx < bots.Count; bIdx++)
        {
            var (bp, bv) = bots[bIdx];

            if (bp.x == mX || bp.y == mY) continue;

            if (bp.x < mX && bp.y < mY) q[0]++;
            if (bp.x > mX && bp.y < mY) q[1]++;
            if (bp.x > mX && bp.y > mY) q[2]++;
            if (bp.x < mX && bp.y > mY) q[3]++;
        }

        WriteLine(q[0]*q[1]*q[2]*q[3]);
    }

    public async Task Part2()
    {
        var gridWidth = 101;
        var gridHeight = 103;

        var bots = await ReadIndata("Day14.txt");

        for (int i = 0; i < 10_000; i++)
        {
            MoveBots(bots, gridWidth, gridHeight);

            var spaceScore = CalculateSpaceScore(bots);

            if (spaceScore > 1000)
            {
                Write("  --------------------  ", ConsoleColor.White);
                Write($"{i} steps (");
                Write("correct answer: ");
                Write(i+1, ConsoleColor.Blue);
                Write(", space score: ");
                Write(spaceScore, ConsoleColor.DarkBlue);
                Write(")");
                Write("  --------------------  ", ConsoleColor.White);
                WriteLine();

                PrintBots(bots, gridWidth, gridHeight);

                WriteLine();

                return;
            }
        }
    }

    private int CalculateSpaceScore(List<((int x, int y) p, (int x, int y) v)> bots)
    {
        var botPositions = bots.Select(_ => _.p).ToHashSet();

        var score = 0;

        foreach (var bot in botPositions)
        {
            foreach (var delta in ((int x, int y)[])[(-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1)])
            {
                if (botPositions.Contains((bot.x + delta.x, bot.y + delta.y)))
                {
                    score++;
                }
            }
        }

        return score;
    }

    private void PrintBots(List<((int x, int y) p, (int x, int y) v)> bots, int gridWidth, int gridHeight)
    {
        var botPositions = bots.Select(_ => _.p).ToHashSet();

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (botPositions.Contains((x, y)))
                {
                    Write('*');
                }
                else
                {
                    Write(' ');
                }
            }
            WriteLine();
        }
    }

    private async Task<List<((int x, int y) p, (int x, int y) v)>> ReadIndata(string inputFile)
    {
        var lines = await File.ReadAllLinesAsync(inputFile);
        var bots = new List<((int x, int y) p, (int x, int y) v)>();

        foreach (var l in lines)
        {
            var pv = l.Trim().Split(' ');
            var pCoord = pv[0][2..].Split(',');
            var vCoord = pv[1][2..].Split(',');

            bots.Add((
                (int.Parse(pCoord[0]), int.Parse(pCoord[1])),
                (int.Parse(vCoord[0]), int.Parse(vCoord[1]))
            ));
        }

        return bots;
    }

    private void MoveBots(List<((int x, int y) p, (int x, int y) v)> bots, int gridWidth, int gridHeight)
    {
        for (var bIdx = 0; bIdx < bots.Count; bIdx++)
        {
            var p = bots[bIdx].p;
            var v = bots[bIdx].v;

            var newP = (x: p.x + v.x, y: p.y + v.y);

            if (newP.x < 0)
            {
                newP = (gridWidth + newP.x, newP.y);
            }
            else if (newP.x >= gridWidth)
            {
                newP = (newP.x - gridWidth, newP.y);
            }

            if (newP.y < 0)
            {
                newP = (newP.x, gridHeight + newP.y);
            }
            else if (newP.y >= gridHeight)
            {
                newP = (newP.x, newP.y - gridHeight);
            }

            bots[bIdx] = (newP, v);
        }
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
