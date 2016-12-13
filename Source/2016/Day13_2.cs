using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static System.Console;

namespace Day13
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

            Solver solver = new Solver();
            //WriteLine(solver.Solve(10, 5));
            WriteLine(solver.Solve(1358, 50));
        }
    }

    public class Solver
    {
        private Queue<Tuple<Point, List<Point>>> _solveQueue;
        private int _seed;

        public Solver()
        {
            _solveQueue = new Queue<Tuple<Point, List<Point>>>();
        }

        public void PrintMaze(int width, int height)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                    Write(GetMazePiece(x, y));
                WriteLine();
            }
            WriteLine();
        }

        public int Solve(int seed, int goal)
        {
            _seed = seed;
            HashSet<Point> globallyVisited = new HashSet<Point>();
            var start = new Point(1, 1);
            EnqueuePossibleMoves(start, new List<Point>());
            globallyVisited.Add(start);

            while(_solveQueue.Count > 0)
            {
                var dequeued = _solveQueue.Dequeue();
                var pos = dequeued.Item1;
                var visited = dequeued.Item2;

                if (visited.Count > goal)
                    continue;

                //WriteLine($"{pos.X}, {pos.Y}");
                
                var mazePiece = GetMazePiece(pos.X, pos.Y);

                if (mazePiece == '#')
                    continue;

                globallyVisited.Add(pos);
                EnqueuePossibleMoves(dequeued.Item1, dequeued.Item2);
            }

            return globallyVisited.Count;
        }

        private char GetMazePiece(int x, int y)
        {
            long code = x*x + 3*x + 2*x*y + y + y*y + _seed;

            int bitSum = 0;
            while (code > 0)
            {
                if ((code & 1L) == 1)
                    bitSum++;

                code >>= 1;
            }

            return bitSum % 2 == 0 ? '.' : '#';
        }

        private void EnqueuePossibleMoves(Point currentPosition, List<Point> visited)
        {
            var possibleMoves = new[]
            {
                new Point(currentPosition.X + 1, currentPosition.Y), 
                new Point(currentPosition.X - 1, currentPosition.Y),
                new Point(currentPosition.X, currentPosition.Y + 1),
                new Point(currentPosition.X, currentPosition.Y - 1),
            };

            foreach(var move in possibleMoves)
            {
                if (move.X < 0 || move.Y < 0)
                    continue;

                if (!visited.Contains(move))
                {
                    var visitedClone = visited.Clone();
                    visitedClone.Add(currentPosition);
                    _solveQueue.Enqueue(Tuple.Create(move, visitedClone));
                }
            }
        }
    }

    public static class ListExtension
    {
        public static List<Point> Clone(this List<Point> input)
        {
            var result = new List<Point>();
            result.AddRange(input);
            return result;
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