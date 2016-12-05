using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static System.Console;

namespace Day2
{
    public class Program
    {
        private const int ProgressBarLength = 20;
        private long progressSteps;
        private long progress;
        private long totalProgressLength;
        private int lastProgressLength;

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

            int lastLength = 0;
            for (int i = 0; i < 50; i++)
            {
                Write($"{i+1} ");
                InitProgressBar(input.Length);

                string newString = string.Empty;
                int idx = 0;
                while (idx < input.Length)
                {
                    char current = input[idx];
                    int counter = 0;
                    
                    while (idx < input.Length && input[idx] == current)
                    {
                        idx++;
                        counter++;
                    }

                    newString += counter.ToString();
                    newString += current;

                    UpdateProgress(idx);
                }

                //WriteLine(newString);
                input = newString;
                lastLength = newString.Length;
                WriteLine();
            }

            WriteLine(lastLength);
        }

        private void UpdateProgress(long latestProgress)
        {
            progress = latestProgress;

            int len = (int)(ProgressBarLength * progress / (double)totalProgressLength);
            if (len == lastProgressLength)
                return;
            
            for (int i = 0; i < len; i++)
                Write("o");
            for (int i = 0; i < len; i++)
                Write("\b");
            
            lastProgressLength = len;
        }

        private void InitProgressBar(long totalLength)
        {
            Write("[");
            for (int i = 0; i < ProgressBarLength; i ++)
                Write(" ");
            Write("]\b");
            for (int i = 0; i < ProgressBarLength; i ++)
                Write("\b");

            progressSteps = totalLength / ProgressBarLength;
            totalProgressLength = totalLength;
            progress = 0;
            lastProgressLength = 0;
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