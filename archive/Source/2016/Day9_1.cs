using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
            IReader reader;
            if (args.Length > 0)
            {
                reader = new FileReader(args[0]);
            }
            else
            {
                reader = new ConsoleReader();
            }

            var input = reader.ReadLine();
            StringBuilder output = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '(')
                {
                    int start = i+1;
                    while (input[i] != ')') i++;
                    int end = i;
                    i++;

                    string data = input.Substring(start, end-start);
                    var split = data.Split('x');

                    int letterCount = int.Parse(split[0]);
                    int repeatCount = int.Parse(split[1]);

                    for (int j = 0; j < repeatCount; j++)
                        output.Append(input.Substring(i, letterCount));
                    
                    i += letterCount-1;
                }
                else
                {
                    output.Append(input[i]);
                }
            }

            WriteLine($"{output.ToString()}: {output.Length}");
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