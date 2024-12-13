using static Day4.ColorWriter;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System;

public class Day4
{
    private (int x, int y) _offset = (1, 1);
    private (int x, int y) _pos = (0, 0);
    private List<string> _data = new();

    public async Task Part1()
    {
        _data =
            (await File.ReadAllLinesAsync("Day4.txt"))
            .Select(_ => $".{_}.")
            .ToList();

        var dataWidth = _data.First().Length-2;
        var dataHeight = _data.Count;

        _data.Insert(0, "".PadLeft(dataWidth+2, '.'));
        _data.Add("".PadLeft(dataWidth+2, '.'));

        var xmas = 0;

        for (int y = 0; y < dataHeight; y++)
        {
            for (int x = 0; x < dataWidth; x++)
            {
                xmas += GetXmas(x, y);
            }
        }

        WriteLine(xmas);
    }

    private int GetXmas(int x, int y)
    {
        _pos = (x, y);

        if (GetData() != 'X') return 0;

        var goal = "XMAS";
        var movements = ((int x, int y)[])
        [
            (1, 0), (1, 1),
            (0, 1), (-1, 1),
            (-1, 0), (-1, -1),
            (0, -1), (1, -1)
        ];

        var xmases = 0;

        foreach (var mvmt in movements)
        {
            _pos = (x, y);
            var word = "";

            for (int i = 0; i < goal.Length; i++)
            {
                var curr = GetData();
                if (curr == '.') break;
                word += curr;
                if (word == goal)
                {
                    xmases++;
                    break;
                }
                _pos = (_pos.x + mvmt.x, _pos.y + mvmt.y);
            }
        }

        return xmases;
    }

    private char GetData()
    {
        var x = _pos.x + _offset.x;
        var y = _pos.y + _offset.y;

        try
        {
            return _data[y][x];
        }
        catch
        {
            WriteLine();
            WriteLine($"pos: {_pos}  offset: {_offset}");
            Environment.Exit(-1);
            return ' ';
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
