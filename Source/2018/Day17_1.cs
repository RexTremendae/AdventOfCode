using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

using static System.Console;

namespace Day17
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int drops = 0;
            if (args.Length > 0) int.TryParse(args[0], out drops);

            new Program().Run(drops);
        }

        private void Run(int drops)
        {
            var data = ParseInput("Day17.txt");
            Trickle(data, drops);
            Print(data);

            var dropCount = 0;
            foreach (var line in data.data)
            {
                foreach (var ch in line)
                {
                    if (ch.In('~', '|')) dropCount++;
                }
            }
            WriteLine($"Water volume: {dropCount}");
        }

        private void Trickle((List<char[]> data, int offsetX) data, int drops)
        {
            var queue = new DropQueue();
            queue.Enqueue((x: 500-data.offsetX, y: 0));
            bool countDrops = drops > 0;

            while((!countDrops || drops-- > 0) && !queue.IsEmpty)
            {
                var current = queue.Dequeue();
                var next = (x: current.x, y: current.y + 1);

                if (next.y >= data.data.Count) continue;

                if (data.data[next.y][next.x].NotIn('#', '~'))
                {
                    data.data[next.y][next.x] = '|';
                    queue.Enqueue(next);
                    continue;
                }

                next = (current.x + 1, current.y);
                if (next.x < data.data[next.y].Length)
                {
                    var nextChar = data.data[next.y][next.x];
                    if (nextChar == '#')
                    {
                        var prev = (current.x, current.y);
                        var entry = (x: 0, y: 0);
                        while (prev.x > 0 && data.data[prev.y][prev.x] == '|')
                        {
                            if (data.data[prev.y-1][prev.x] == '|') entry = (prev.x, prev.y-1);
                            prev = (prev.x-1, prev.y);
                        }
                        if (data.data[prev.y][prev.x] == '#')
                        {
                            if (entry.x-1 >= 0 && data.data[entry.y][entry.x-1] != '#')
                                queue.Enqueue((entry.x-1, entry.y-1));
                            if (entry.x+1 < data.data[entry.y].Length && data.data[entry.y][entry.x+1] != '#')
                                queue.Enqueue((entry.x+1, entry.y-1));
                            prev = (prev.x+1, prev.y);
                            while (data.data[prev.y][prev.x] == '|')
                            {
                                data.data[prev.y][prev.x] = '~';
                                prev = (prev.x+1, prev.y);
                            }
                        }
                    }
                    else if (nextChar.NotIn('|', '~'))
                    {
                        data.data[next.y][next.x] = '|';
                        queue.Enqueue(next);
                    }
                }

                next = (current.x - 1, current.y);
                if (next.x >= 0)
                {
                    var nextChar = data.data[next.y][next.x];
                    if (nextChar == '#')
                    {
                        var prev = (current.x, current.y);
                        var entry = (x: 0, y: 0);
                        while (prev.x > 0 && data.data[prev.y][prev.x] == '|')
                        {
                            if (data.data[prev.y-1][prev.x] == '|') entry = (prev.x, prev.y-1);
                            prev = (prev.x+1, prev.y);
                        }
                        if (data.data[prev.y][prev.x] == '#')
                        {
                            if (entry.x-1 >= 0 && data.data[entry.y][entry.x-1] != '#')
                                queue.Enqueue((entry.x-1, entry.y-1));
                            if (entry.x+1 < data.data[entry.y].Length && data.data[entry.y][entry.x+1] != '#')
                                queue.Enqueue((entry.x+1, entry.y-1));
                            prev = (prev.x-1, prev.y);
                            while (data.data[prev.y][prev.x] == '|')
                            {
                                data.data[prev.y][prev.x] = '~';
                                prev = (prev.x-1, prev.y);
                            }
                        }
                    }
                    else if (nextChar.NotIn('|', '~'))
                    {
                        data.data[next.y][next.x] = '|';
                        queue.Enqueue(next);
                    }
                }
            }
        }

        private class DropQueue
        {
            public DropQueue()
            {
                _queue = new Queue<(int x, int y)>();
            }

            private Queue<(int x, int y)> _queue;

            public void Enqueue((int x, int y) drop, [CallerLineNumber] int? lineNumber = null) => _queue.Enqueue(drop);
            public (int x, int y) Dequeue([CallerLineNumber] int? lineNumber = null) => _queue.Dequeue();
            public (int x, int y) Peek() => _queue.Peek();

            public bool IsEmpty => !_queue.Any();
        }

        private (List<char[]> data, int offsetX) ParseInput(string filepath)
        {
            int lineNumber = 1;
            int minX = int.MaxValue;
            int maxX = 0, maxY = 0;

            var intervals = new List<(int? x1, int? x2, int? y1, int? y2)>();
            foreach (var line in File.ReadAllLines(filepath).Select(x => x.Trim()))
            {
                if (string.IsNullOrEmpty(line)) continue;

                var split = line.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());
                if (split.Count() != 2) throw new InvalidDataException($"@{lineNumber}: contains illegal data. Should contain exactly one ',' character but was not.");

                (int?, int?) x = (null, null);
                (int?, int?) y = (null, null);

                foreach (var s in split)
                {
                    var interval = s.Split(".", StringSplitOptions.RemoveEmptyEntries);
                    int first = 0;
                    int? second = null;

                    if (!int.TryParse(interval[0].Substring(2), out first))
                    {
                        throw new InvalidDataException($"@{lineNumber}: Could not parse integer data.");
                    }
                    if (interval.Length > 1)
                    {
                        if (!int.TryParse(interval[1], out var secondNonNullable)) throw new InvalidDataException($"@{lineNumber}: Could not parse integer data.");
                        second = secondNonNullable;
                    }

                    if (second.HasValue && second.Value < first) throw new InvalidDataException($"@{lineNumber}: Invalid interval, left part should be lower than right part.");

                    switch (char.ToLower(interval[0][0]))
                    {
                        case 'x':
                            x = (first, second);
                            break;

                        case 'y':
                            y = (first, second);
                            break;

                        default:
                            throw new InvalidDataException($"@{lineNumber}: Comma separated data must start with either 'x' or 'y'.");
                    }
                }

                intervals.Add((x.Item1, x.Item2, y.Item1, y.Item2));

                if (x.Item1.Value < minX) minX = x.Item1.Value;
                if (x.Item1.Value > maxX) maxX = x.Item1.Value;
                if (x.Item2.HasValue)
                {
                    if (x.Item2.Value < minX) minX = x.Item2.Value;
                    if (x.Item2.Value > maxX) maxX = x.Item2.Value;
                }

                if (y.Item1.Value > maxY) maxY = y.Item1.Value;
                if (y.Item2.HasValue)
                {
                    if (y.Item2.Value > maxY) maxY = y.Item2.Value;
                }
            }

            int offsetX = minX - 1;
            int width = maxX - minX + 3;

            var result = new List<char[]>();
            for (int y = 0; y <= maxY; y++)
            {
                result.Add(new char[width]);
                for (var x = 0; x < width; x++) result[y][x] = '.';
            }

            foreach (var interval in intervals)
            {
                if (interval.Item2 == null)
                {
                    for (int y = interval.Item3.Value; y <= interval.Item4.Value; y++)
                    {
                        result[y][interval.Item1.Value-offsetX] = '#';
                    }
                }
                else
                {
                    for (int x = interval.Item1.Value; x <= interval.Item2.Value; x++)
                    {
                        result[interval.Item3.Value][x-offsetX] = '#';
                    }
                }
            }

            return (result, offsetX);
        }

        private void Print((List<char[]> data, int offsetX) data)
        {
            bool firstLine = true;
            foreach (var line in data.data)
            {
                for (int x = 0; x < line.Length; x++)
                {
                    if (firstLine && x + data.offsetX == 500) Write('+');
                    else Write(line[x]);
                }
                WriteLine();
                firstLine = false;
            }
            WriteLine($"X offset = {data.offsetX}");
            WriteLine();
        }
    }

    public static class Extensions
    {
        public static bool NotIn(this char ch, params char[] array)
        {
            return !ch.In(array);
        }

        public static bool In(this char ch, params char[] array)
        {
            return array.Contains(ch);
        }
    }
}