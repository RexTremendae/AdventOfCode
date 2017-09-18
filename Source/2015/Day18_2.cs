using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using static System.Console;
using static System.ConsoleColor;

namespace Day18
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new Program().Run(args);
        }

        public void Run(string[] args)
        {
            IReader reader;
            if (args.Length > 0)
            {
                reader = new FileReader(args[0]);
            }
            else
            {
                reader = new ConsoleReader();
            }

            int width = 0;

            var rawData = new List<string>();

            int line = 0;
            while (!reader.EndOfStream)
            {
                var input = reader.ReadLine().Trim();
                if (string.IsNullOrEmpty(input)) break;

                if (width == 0)
                {
                    width = input.Length;
                }
                else if (width != input.Length)
                {
                    WriteLine($"ERROR! Inconsistent width on line {line}.");
                    return;
                }

                rawData.Add(input);
                line++;
            }

            var data = RefineData(rawData);

            for (int i = 0; i < 100; i++)
            {
                data = NextState(data);
            }

            int count = 0;
            for (int y = 0; y < data.Length; y++)
                for (int x = 0; x < data.Length; x++)
                    if (data[y][x] == '#') count++;
            
            WriteLine(count);
        }

        private char[][] RefineData(List<string> rawData)
        {
            var height = rawData.Count;
            var width = rawData[0].Length;

            var refined = new char[height][];

            for (int y = 0; y < height; y++)
            {
                refined[y] = new char[width];
                for (int x = 0; x < width; x++)
                {
                    refined[y][x] = rawData[y][x];
                }
            }

            return refined;
        }

        private char[][] NextState(char[][] state)
        {
            int width = state[0].Length;
            int height = state.Length;

            var framedState = FrameState(state);
            var nextState = new char[height][];

            for (int y = 0; y < height; y++)
            {
                nextState[y] = new char[width];
                for (int x = 0; x < width; x++)
                {
                    if ((x == 0 && y == 0) || 
                        (x == 0 && y == height-1) ||
                        (x == width-1 && y == 0) ||
                        (x == width-1 && y == height-1))
                    {
                        nextState[y][x] = '#';
                        continue;
                    }

                    int neighbors = 0;

                    for (int yy = -1; yy < 2; yy++)
                        for (int xx = -1; xx < 2; xx++)
                        {
                            if (xx == 0 && yy == 0) continue;

                            if (framedState[y+1 + yy][x+1 + xx] == '#') neighbors++;
                        }

                    if (neighbors < 2)
                    {
                        nextState[y][x] = '.';
                    }
                    else if (neighbors == 2)
                    {
                        nextState[y][x] = state[y][x];
                    }
                    else if (neighbors == 3)
                    {
                        nextState[y][x] = '#';
                    }
                    else if (neighbors > 3)
                    {
                        nextState[y][x] = '.';
                    }
                }
            }

            return nextState;
        }

        private char[][] FrameState(char[][] state)
        {
            int width = state[0].Length;
            int height = state.Length;

            var framedState = new char[height+2][];
            for (int y = 0; y < height+2; y++)
            {
                framedState[y] = new char[width+2];
                if (y == 0 || y == height+1)
                {
                    for (int x = 0; x < width+2; x++)
                    {
                        framedState[y][x] = '.';
                    }
                }
                else
                {
                    framedState[y][0] = '.';
                    framedState[y][width+1] = '.';
                    for (int x = 1; x < width+1; x++)
                    {
                        var c = state[y-1][x-1];
                        framedState[y][x] = c;
                    }
                }
            }

            return framedState;
        }

        private void Write(char[][] data)
        {
            for (int y = 0; y < data.Length; y++)
            {
                for (int x = 0; x < data[0].Length; x++)
                {
                    Console.Write(data[y][x]);
                }

                WriteLine();
            }
        }
    }

    interface IReader
    {
        string ReadLine();
        bool EndOfStream { get; }
    }

    public class ConsoleReader : IReader
    {
        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public bool EndOfStream => false;
    }

    public class FileReader : IReader
    {
        StreamReader _reader;
        
        public FileReader(string filename)
        {
            _reader = new StreamReader(filename);
        }

        public string ReadLine()
        {
            return _reader.ReadLine();
        }

        public bool EndOfStream 
        {
            get
            {
                return _reader.EndOfStream;
            }
        }
    }
}