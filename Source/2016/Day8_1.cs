using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static System.Console;

namespace Day2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new Program().Run(args);
        }

        public void Run(string[] args)
        {
            const int width = 50, height = 6;
            bool[] display = new bool[width*height];

            IReader reader;
            if (args.Length > 0)
            {
                reader = new FileReader(args[0]);
            }
            else
            {
                reader = new ConsoleReader();
            }

            while (!reader.EndOfStream)
            {
                PrintDisplay(display, width, height);
                var input = reader.ReadLine();
                if (string.IsNullOrEmpty(input)) break;

                var split = input.Split(' ');
                if (split[0] == "rect")
                {
                    var rectSize = split[1].Split('x');
                    int xx = int.Parse(rectSize[0]);
                    int yy = int.Parse(rectSize[1]);

                    for (int y = 0; y < yy; y++)
                        for (int x = 0; x < xx; x++)
                            display[y * width + x] = true;
                }
                else
                {
                    int index = int.Parse(split[2].Split('=')[1]);
                    int offset = int.Parse(split[4]);

                    char c = split[1][0];
                    if (c == 'r')
                    {
                        int y = index;
                        for (int steps = 0; steps < offset; steps++)
                        {
                            bool b = display[y*width + width - 1];
                            for (int x = width - 1; x > 0; x --)
                            {
                                //WriteLine($"{y*width + x} {y*width + x - 1}");
                                display[y*width + x] = display[y*width + x - 1];
                            }
                            
                            display[y*width] = b;
                        }
                    }
                    else
                    {
                        int x = index;
                        for (int steps = 0; steps < offset; steps++)
                        {
                            bool b = display[(height-1)*width + x];
                            for (int y = height - 1; y > 0; y --)
                            {
                                //WriteLine($"{y*width + x} {y*width + x - 1}");
                                display[y*width + x] = display[(y-1)*width + x];
                            }
                            
                            display[x] = b;
                        }
                    }
                }
                WriteLine(input);
                //ReadKey();
            }
            PrintDisplay(display, width, height);
            
            int sum = 0;
            for (int i = 0; i < width*height; i++)
                if (display[i]) sum++;
            
            WriteLine(sum);
        }

        void PrintDisplay(bool[] data, int width, int height)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Write(data[y*width + x] ? '#' : '.');
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

    public class Point
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }

        public int Y { get; }

        public static Point Parse(string input)
        {
            var split = input.Split(',');
            int x = int.Parse(split[0]);
            int y = int.Parse(split[1]);

            return new Point(x, y);
        }

        public override int GetHashCode()
        {
            return Tuple.Create(X, Y).GetHashCode();
        }

        public override bool Equals(object other)
        {
            Point otherPoint = other as Point;
            if (otherPoint == null)
            {
                return false;
            }

            return X == otherPoint.X && Y == otherPoint.Y;
        }
    }
}