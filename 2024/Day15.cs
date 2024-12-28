using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using static Day15.ColorWriter;

public class Day15
{
    public async Task Part1()
    {
        Console.CursorVisible = false;
        var (map, start, movements) = await ReadDataAsync("Day15.txt");

        Console.Clear();

        var botPos = start;
        PrintMap(map, start);
        var history = string.Empty;

        foreach (var mve in movements)
        {
            //await Task.Delay(100);
            botPos = Move(map, botPos, mve);
            //PrintMap(map, botPos);
            //Write(history, ConsoleColor.DarkGray);
            //WriteLine(mve, ConsoleColor.White);
            history += mve;
        }

        PrintMap(map, botPos);

        var coordSum = 0L;
        var coord = 0;
        for (var y = 0; y < map.Length; y++)
        {
            for (var x = 0; x < map[0].Length; x++)
            {
                if (map[y][x] == 'O') coordSum += coord;
                coord ++;
            }

            coord += 100;
            coord -= map[0].Length;
        }

        WriteLine(coordSum);
    }

    private (int x, int y) Move(char[][] map, (int x, int y) bot, char mve)
    {
        var height = map.Length;
        var width = map[0].Length;

        var delta = (x: 0, y: 0);

        switch (mve)
        {
            case '^':
                delta = (0, -1);
                break;

            case 'v':
                delta = (0, +1);
                break;

            case '>':
                delta = (+1, 0);
                break;

            case '<':
                delta = (-1, 0);
                break;
        }

        var newBot = (x: bot.x + delta.x, y: bot.y + delta.y);

        if (newBot.x < 0 || newBot.y < 0 || newBot.x >= width || newBot.y >= height)
            return bot;

        if (map[newBot.y][newBot.x] == '#')
            return bot;

        var canMove = false;
        var movePos = bot;

        var boxesToMove = new List<(int x, int y)>();

        for (;;)
        {
            movePos = (movePos.x + delta.x, movePos.y + delta.y);

            var mapChar = map[movePos.y][movePos.x];

            if (mapChar == 'O')
            {
                boxesToMove.Add(movePos);
                continue;
            }

            if (mapChar == '.')
            {
                canMove = true;
                break;
            }

            break;
        }

        if (!canMove)
            return bot;

        if (boxesToMove.Any())
        {
            var firstBox = boxesToMove.First();
            var lastBox = boxesToMove.Last();

            map[lastBox.y+delta.y][lastBox.x+delta.x] = 'O';

            map[firstBox.y][firstBox.x] = '.';
        }

        return newBot;
    }

    private void PrintMap(char[][] map, (int x, int y) start)
    {
        Console.CursorLeft = 0;
        Console.CursorTop = 0;

        for (var y = 0; y < map.Length; y++)
        {
            for (var x = 0; x < map[0].Length; x++)
            {
                var color = map[y][x] switch
                {
                    '#' => ConsoleColor.DarkGray,
                    '.' => ConsoleColor.DarkGray,
                    'O' => ConsoleColor.White,
                    _ => ConsoleColor.Gray
                };

                Write(map[y][x], color);
            }
            WriteLine();
        }

        var cursorX = Console.CursorLeft;
        var cursorY = Console.CursorTop;

        Console.CursorLeft = start.x;
        Console.CursorTop = start.y;

        Write('@', ConsoleColor.Cyan);

        Console.CursorLeft = cursorX;
        Console.CursorTop = cursorY;
    }

    private async Task<(char[][] map, (int x, int y) start, char[] movements)> ReadDataAsync(string filename)
    {
        var lines = await File.ReadAllLinesAsync(filename);

        List<char[]> map = new();
        List<char> movements = new();

        var isMap = true;

        foreach (var lne in lines.Select(_ => _.Trim()))
        {
            if (string.IsNullOrEmpty(lne))
            {
                isMap = false;
                continue;
            }

            if (isMap)
            {
                map.Add(lne.ToCharArray());
            }
            else
            {
                movements.AddRange(lne.ToCharArray());
            }
        }

        var start = (x: -1, y: -1);
        for (var y = 0; y < map.Count; y++)
        {
            for (var x = 0; x < map[0].Length; x++)
            {
                if (map[y][x] == '@')
                {
                    start = (x, y);
                    map[y][x] = '.';
                    break;
                }
            }

            if (start.x >= 0) break;
        }

        return (map.ToArray(), start, movements.ToArray());
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
