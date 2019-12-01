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

                if (SupportSSL(input)) supports++;
            }

            WriteLine(supports);
        }

        IEnumerable<string> GetABAs(string input)
        {
            if (input.Length < 3)
                yield break;

            for (int i = 0; i <= input.Length-3; i++)
            {
                if (input[i] == input[i+1] || input[i+1] == input[i+2])
                    continue;

                if (input[i] != input[i+2])
                    continue;

                yield return input.Substring(i, 3);
            }
        }

        bool SupportSSL(string input)
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

            foreach (var outs in outsides)
            {
                var ABAs = GetABAs(outs);
                foreach(var aba in ABAs)
                {
                    var bab = $"{aba[1]}{aba[0]}{aba[1]}";
                    if (insides.Any(x => x.Contains(bab)))
                        return true;
                }
            }

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