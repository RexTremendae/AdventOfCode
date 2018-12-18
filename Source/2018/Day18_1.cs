using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static System.Console;

namespace Day15
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int rounds = 1;
            if (args.Length > 0) int.TryParse(args[0], out rounds);
            new Program().Run(rounds);
        }

        public void Run(int rounds)
        {
            var board = ParseInput("Day18.txt");
            foreach (var line in board)
            {
                foreach (var c in line)
                    Write(c);
                WriteLine();
            }

            List<char[]> nextIteration = null;

            for (int r = 0; r < rounds; r++)
            {
            nextIteration = new List<char[]>();
            var newLine = new char[board[0].Length];
            for (int i = 0; i < board[0].Length; i++) newLine[i] = '.';
            nextIteration.Add(newLine);

            for (int y = 1; y < board.Count-1; y++)
            {
                newLine = new char[board[0].Length];
                newLine[0] = '.';
                newLine[board[0].Length - 1] = '.';
                nextIteration.Add(newLine);

                for (int x = 1; x < board[0].Length-1; x++)
                {
                    var neighborCount = new Dictionary<char, int>();
                    neighborCount['.'] = 0;
                    neighborCount['#'] = 0;
                    neighborCount['|'] = 0;

                    for (int yy = y-1; yy <= y+1; yy++)
                    for (int xx = x-1; xx <= x+1; xx++)
                    {
                        if (xx == x && yy == y) continue;

                        var chr = board[yy][xx];
                        neighborCount[chr] ++;
                    }

                    nextIteration[y][x] = board[y][x];

                    if (board[y][x] == '.' && neighborCount['|'] >= 3) nextIteration[y][x] = '|';
                    if (board[y][x] == '|' && neighborCount['#'] >= 3) nextIteration[y][x] = '#';
                    if (board[y][x] == '#' && (neighborCount['#'] < 1 || neighborCount['|'] < 1)) nextIteration[y][x] = '.';
                }
            }

            newLine = new char[board[0].Length];
            for (int i = 0; i < board[0].Length; i++) newLine[i] = '.';
            nextIteration.Add(newLine);

            board = nextIteration;
            }

            WriteLine();

            var resourceCount = new Dictionary<char, int>();
            resourceCount['#'] = 0;
            resourceCount['|'] = 0;
            resourceCount['.'] = 0;

            foreach (var line in nextIteration)
            {
                foreach (var c in line)
                {
                    resourceCount[c]++;
                    Write(c);
                }
                WriteLine();
            }

            WriteLine($"'|' {resourceCount['|']} * '#' {resourceCount['#']} = {resourceCount['|'] * resourceCount['#']}");
        }

        public List<char[]> ParseInput(string filepath)
        {
            var board = new List<char[]>();

            foreach (var lineRaw in File.ReadAllLines(filepath))
            {
                if (string.IsNullOrEmpty(lineRaw)) { break; }

                var line = new List<char>();

                line.Add('.');
                foreach (var c in lineRaw)
                {
                    line.Add(c);
                }
                line.Add('.');

                board.Add(line.ToArray());
            }

            int width = board[0].Length;
            var emptyLine = new char[width];
            for (int i = 0; i < width; i++) emptyLine[i] = '.';
            board.Insert(0, emptyLine);

            emptyLine = new char[width];
            for (int i = 0; i < width; i++) emptyLine[i] = '.';
            board.Add(emptyLine);

            return board;
        }
    }


    public static class Extensions
    {
        public static bool None<T>(this IEnumerable<T> source)
        {
            return !source.Any();
        }

        public static bool None<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return !source.Any(predicate);
        }
    }
}
