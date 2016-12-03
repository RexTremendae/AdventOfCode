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

            HashSet<Point> visited = new HashSet<Point>();
            Point santa = new Point(0, 0);
            Point roboSanta = new Point(0, 0);
            visited.Add(santa);
            bool roboSantaMoves = false;

            foreach (var move in input)
            {
                int dX = 0, dY = 0;
    
                switch(move)
                {
                    case '^':
                        dY = -1;
                        break;
                    case '<':
                        dX = -1;
                        break;
                    case '>':
                        dX = 1;
                        break;
                    case 'v':
                        dY = 1;
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                if (roboSantaMoves)
                {
                    roboSanta = new Point(roboSanta.X + dX, roboSanta.Y + dY);
                    visited.Add(roboSanta);
                    roboSantaMoves = false;
                }
                else
                {
                    santa = new Point(santa.X + dX, santa.Y + dY);
                    visited.Add(santa);
                    roboSantaMoves = true;
                }
            }

            WriteLine(visited.Count());
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