using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static System.Console;

namespace Day22
{
    public class Program
    {
        public static void Main()
        {
            new Program().Run();
        }

        public void Run()
        {
            //var grid = ReadIndata("Day22_small.txt");
            var grid = ReadIndata("Day22.txt");
            var maxX = grid.Keys.Select(key => key.x).Max();
            var maxY = grid.Keys.Select(key => key.y).Max();

            var sizeIntervalSmallerThan100 = (min: 100_000, max: 0);
            var sizeIntervalLargerThan100 = (min: 100_000, max: 0);

            var usedIntervalSmallerThan100 = (min: 100_000, max: 0);
            var usedIntervalLargerThan100 = (min: 100_000, max: 0);

            var abstractGrid = new List<char[]>();

            var emptyPos = (x: 0, y: 0);
            var goalPos = (x: 0, y: 0);

            for (int y = 0; y <= maxY; y++)
            {
                abstractGrid.Add(new char[maxX+1]);

                for (int x = 0; x <= maxX; x++)
                {
                    var (used, size) = grid[(x, y)];

                    if (used != 0 && used < 100)
                    {
                        if (usedIntervalSmallerThan100.min > used)
                        {
                            usedIntervalSmallerThan100 = (used, usedIntervalSmallerThan100.max);
                        }
                        if (usedIntervalSmallerThan100.max < used)
                        {
                            usedIntervalSmallerThan100 = (usedIntervalSmallerThan100.min, used);
                        }
                    }
                    else if (used != 0 && used > 100)
                    {
                        if (usedIntervalLargerThan100.min > used)
                        {
                            usedIntervalLargerThan100 = (used, usedIntervalLargerThan100.max);
                        }
                        if (usedIntervalLargerThan100.max < used)
                        {
                            usedIntervalLargerThan100 = (usedIntervalLargerThan100.min, used);
                        }
                    }

                    if (size < 100)
                    {
                        if (sizeIntervalSmallerThan100.min > size)
                        {
                            sizeIntervalSmallerThan100 = (size, sizeIntervalSmallerThan100.max);
                        }
                        if (sizeIntervalSmallerThan100.max < size)
                        {
                            sizeIntervalSmallerThan100 = (sizeIntervalSmallerThan100.min, size);
                        }
                    }
                    else
                    {
                        if (sizeIntervalLargerThan100.min > size)
                        {
                            sizeIntervalLargerThan100 = (size, sizeIntervalLargerThan100.max);
                        }
                        if (sizeIntervalLargerThan100.max < size)
                        {
                            sizeIntervalLargerThan100 = (sizeIntervalLargerThan100.min, size);
                        }
                    }

                    if (used == 0) emptyPos = (x, y);
                    if (x == maxX && y == 0) goalPos = (x, y);
                    abstractGrid[y][x] = (used < 100 ? '.' : '#');
                }
            }

            WriteLine();

            WriteLine("Used intervals:");
            WriteLine($"{usedIntervalSmallerThan100.min} - {usedIntervalSmallerThan100.max}");
            WriteLine($"{usedIntervalLargerThan100.min} - {usedIntervalLargerThan100.max}");
            WriteLine();

            WriteLine("Size intervals:");
            WriteLine($"{sizeIntervalSmallerThan100.min} - {sizeIntervalSmallerThan100.max}");
            WriteLine($"{sizeIntervalLargerThan100.min} - {sizeIntervalLargerThan100.max}");
            WriteLine();

            WriteLine("Press a key to start...");

            ReadKey();
            var count = 0;

            for(;;)
            {
                Clear();
                for (int y = 0; y < abstractGrid.Count; y++)
                {
                    Write (y == 0 ? "(" : " ");
                    var line = abstractGrid[y];
                    var endChar = (y == 0 ? ")" : " ");
                    for (int x = 0; x < line.Length; x++)
                    {
                        if ((x, y) == emptyPos)
                        {
                            Write($"_{endChar}");
                        }
                        else if ((x, y) == goalPos)
                        {
                            Write($"G{endChar}");
                        }
                        else
                        {
                            Write($"{line[x]}{endChar}");
                        }
                        endChar = " ";
                    }
                    WriteLine();
                }
                WriteLine(count);

                if (goalPos == (0, 0)) break;
                var input = ReadKey();

                var prevEmptyPos = emptyPos;
                if (input.Key == ConsoleKey.UpArrow && emptyPos.y > 0 && abstractGrid[emptyPos.y-1][emptyPos.x] != '#')
                {
                    emptyPos = (emptyPos.x, emptyPos.y-1);
                    count++;
                }
                else if (input.Key == ConsoleKey.DownArrow && emptyPos.y < maxY && abstractGrid[emptyPos.y+1][emptyPos.x] != '#')
                {
                    emptyPos = (emptyPos.x, emptyPos.y+1);
                    count++;
                }
                else if (input.Key == ConsoleKey.LeftArrow && emptyPos.x > 0 && abstractGrid[emptyPos.y][emptyPos.x-1] != '#')
                {
                    emptyPos = (emptyPos.x-1, emptyPos.y);
                    count++;
                }
                else if (input.Key == ConsoleKey.RightArrow && emptyPos.x < maxX && abstractGrid[emptyPos.y][emptyPos.x+1] != '#')
                {
                    emptyPos = (emptyPos.x+1, emptyPos.y);
                    count++;
                }
                if (emptyPos == goalPos) goalPos = prevEmptyPos;
            }
        }

        public Dictionary<(int x, int y), (int used, int size)>
        Clone(Dictionary<(int x, int y), (int used, int size)> input)
        {
            var clone = new Dictionary<(int x, int y), (int used, int Size)>();
            foreach ((var key, var val) in input) clone.Add(key, val);
            return clone;
        }

        public Dictionary<(int x, int y), (int used, int size)> ReadIndata(string inputFilename)
        {
            var grid = new Dictionary<(int,int), (int,int)>();
            using (var reader = new StreamReader(inputFilename))
            {
                int count = 0;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line)) break;
                    count++;
                    if (count <= 2) continue;

                    int startIdx = 15;
                    int endIdx = startIdx;
                    while (line[endIdx] != ' ') endIdx++;

                    var xyRaw = line.Substring(startIdx, endIdx - startIdx);
                    var xySplit = xyRaw.Split('-');
                    var x = Convert.ToInt32(xySplit[0].Substring(1));
                    var y = Convert.ToInt32(xySplit[1].Substring(1));

                    startIdx = endIdx+1;
                    while (line[startIdx] == ' ') startIdx++;
                    endIdx = startIdx;
                    while (line[endIdx] != ' ') endIdx++;
                    var sizeRaw = line.Substring(startIdx, endIdx - startIdx);
                    var size = Convert.ToInt32(sizeRaw.Substring(0, sizeRaw.Length-1));

                    startIdx = endIdx+1;
                    while (line[startIdx] == ' ') startIdx++;
                    endIdx = startIdx;
                    while (line[endIdx] != ' ') endIdx++;
                    var usedRaw = line.Substring(startIdx, endIdx - startIdx);
                    var used = Convert.ToInt32(usedRaw.Substring(0, usedRaw.Length-1));

                    grid.Add((x, y), (used, size));
                }
            }

            return grid;
        }
    }
}