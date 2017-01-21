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

            int niceWordsCount = 0;

            while (!reader.EndOfStream)
            {
                var input = reader.ReadLine();
                if (string.IsNullOrEmpty(input))
                    break;
    
                if (IsNice(input)) niceWordsCount++;
            }

            WriteLine(niceWordsCount);
        }

        public bool IsNice(string input)
        {
            bool repeatWithOneInbetween = false;
            for (int i = 0; i < input.Length -2; i++)
            {
                if (input[i] == input[i+2])
                {
                    repeatWithOneInbetween = true;
                    break;
                }
            }

            if (!repeatWithOneInbetween) return false;

            bool repeatedPairs = false;
            for (int i = 0; i < input.Length-1; i++)
            {
                for (int j = i+2; j < input.Length-1; j++)
                {
                    if (input[i] == input[j] && input[i+1] == input[j+1])
                    {
                        repeatedPairs = true;
                        break;
                    }
                }

                if (repeatedPairs) break;
            }

            return repeatedPairs;
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