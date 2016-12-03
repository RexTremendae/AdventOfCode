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
            string[] naughtySubStrings = new string[] { "ab", "cd", "pq", "xy" };
            int vowelCount = 0;
            bool duplicate = false;

            for (int i = 0; i < input.Length; i++)
            {
                if (IsVowel(input[i])) vowelCount++;

                if (i < input.Length - 1)
                {
                    if (!duplicate)
                    {
                        if (input[i] == input[i+1])
                            duplicate = true;
                    }

                    for (int j = 0; j < naughtySubStrings.Length; j++)
                    {
                        if (input[i] == naughtySubStrings[j][0] && input[i+1] == naughtySubStrings[j][1])
                            return false;
                    }
                }
            }

            if (vowelCount < 3) return false;
            if (!duplicate) return false;

            return true;
        }

        public bool IsVowel(char c)
        {
            if (c == 'a') return true;
            if (c == 'e') return true;
            if (c == 'i') return true;
            if (c == 'o') return true;
            if (c == 'u') return true;

            return false;
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