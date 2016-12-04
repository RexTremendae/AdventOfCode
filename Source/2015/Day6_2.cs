using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static System.Console;

namespace Day2
{
    public enum Operation
    {
        NoOperation = 0,
        TurnOn,
        TurnOff,
        Toggle
    }

    public class Program
    {
        private static readonly Point NoPoint = new Point(int.MinValue, int.MinValue);

        public static void Main(string[] args)
        {
            new Program().Run(args);
        }

        public void Run(string[] args)
        {
            int matrixSize = 1000;
            IReader reader;
            if (args.Length > 0)
            {
                reader = new FileReader(args[0]);
            }
            else
            {
                reader = new ConsoleReader();
            }

            int[] lights = new int[matrixSize*matrixSize];

            while (!reader.EndOfStream)
            {
                //PrintMatrix(lights);
                var inputRaw = reader.ReadLine();
                if (string.IsNullOrEmpty(inputRaw)) break;
                Tuple<Operation, Point, Point> input;

                try
                {
                    input = ParseInput(inputRaw);
                    //WriteLine($"{input.Item1} [{input.Item2.X}, {input.Item2.Y}]::[{input.Item3.X}, {input.Item3.Y}]");                    
                }
                catch (System.Exception)
                {
                    WriteLine("Invalid input, skipping");
                    continue;
                }

                var op = input.Item1;
                var p1 = input.Item2;
                var p2 = input.Item3;

                int minX, minY;
                int maxX, maxY;

                if (p1.X > p2.X)
                {
                    minX = p2.X;
                    maxX = p1.X;
                }
                else
                {
                    minX = p1.X;
                    maxX = p2.X;
                }

                if (p1.Y > p2.Y)
                {
                    minY = p2.Y;
                    maxY = p1.Y;
                }
                else
                {
                    minY = p1.Y;
                    maxY = p2.Y;
                }

                for (int y = minY; y <= maxY; y++)
                {
                    for (int x = minX; x <= maxX; x++)
                    {
                        int pos = y*matrixSize + x;
                        if (op == Operation.TurnOn)
                            lights[pos]++;
                        else if (op == Operation.TurnOff)
                        {
                            lights[pos]--;
                            if (lights[pos] < 0) lights[pos] = 0;
                        }
                        else if (op == Operation.Toggle)
                            lights[pos]+=2;
                    }
                }
            }

            var totalSize = matrixSize*matrixSize;
            long lightsCount = 0;

            for(int i = 0; i < totalSize; i++)
                lightsCount+=lights[i];

            WriteLine(lightsCount);
        }

        private void PrintMatrix(int[] matrix)
        {
            int size = (int) Math.Sqrt(matrix.Length);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Write(matrix[y*size+x] > 0 ? "o" : "·");
                }
                WriteLine();
            }
        }

        private Tuple<Operation, Point, Point> ParseInput(string input)
        {
            var splitInput = input.Split(' ');

            Operation operation = Operation.NoOperation;
            int coordIndex;

            if (input.StartsWith("turn on"))
            {
                operation = Operation.TurnOn;
                coordIndex = 2;
            }
            else if (input.StartsWith("turn off"))
            {
                operation = Operation.TurnOff;
                coordIndex = 2;
            }
            else if (input.StartsWith("toggle"))
            {
                operation = Operation.Toggle;
                coordIndex = 1;
            }
            else return Tuple.Create(Operation.NoOperation, NoPoint, NoPoint);

            var Point1 = Point.Parse(splitInput[coordIndex]);
            var Point2 = Point.Parse(splitInput[coordIndex + 2]);
            return Tuple.Create(operation, Point1, Point2);
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