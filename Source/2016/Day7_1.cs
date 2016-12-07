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

            int supports = 0;
            while (!reader.EndOfStream)
            {
                var input = reader.ReadLine();
                if (string.IsNullOrEmpty(input)) break;

                if (SupportTLS(input)) supports++;
            }

            WriteLine(supports);
        }

        bool ContainsABBA(string input)
        {
            if (input.Length < 4)
                return false;

            for (int i = 0; i <= input.Length-4; i++)
            {
                if (input[i] == input[i+1] || input[i+2] == input[i+3])
                    continue;
                
                if (input[i] != input[i+3] || input[i+1] != input[i+2])
                    continue;
                
                return true;
            }

            return false;
        }

        bool SupportTLS(string input)
        {
            List<string> insides = new List<string>();
            List<string> outsides = new List<string>();

            bool isInside = false;
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '[')
                {
                    if (sb.Length > 0)
                    {
                        outsides.Add(sb.ToString());
                    }
                    sb.Clear();
                    isInside = true;
                }
                else if (input[i] == ']')
                {
                    if (sb.Length > 0)
                    {
                        insides.Add(sb.ToString());
                    }
                    sb.Clear();
                    isInside = false;
                }
                else
                {
                    sb.Append(input[i]);
                }
            }
            if (isInside)
            {
                insides.Add(sb.ToString());
            }
            else
            {
                outsides.Add(sb.ToString());
            }

            return !insides.Any(ContainsABBA) && outsides.Any(ContainsABBA);
/*
            WriteLine("Insides:");
            foreach (var ins in insides)
            {
                string abba = ContainsABBA(ins) ? "(ABBA)" : "";
                Write($"{ins}{abba} ");
            }
            WriteLine();

            WriteLine("Outsides:");
            foreach (var outs in outsides)
            {
                string abba = ContainsABBA(outs) ? "(ABBA)" : "";
                Write($"{outs}{abba} ");
            }
            WriteLine();
*/
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