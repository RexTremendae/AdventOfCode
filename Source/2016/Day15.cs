using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using static System.Console;

namespace Day15
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

            List<int> sizes = new List<int>();
            List<int> discs = new List<int>();
            int discCount = 0;

            while (!reader.EndOfStream)
            {
                var input = reader.ReadLine();
                if (string.IsNullOrEmpty(input)) break;

                var split = input.Split(' ');
                var size = int.Parse(split[3]);
                var start = int.Parse(split[11].Split('.')[0]);

                sizes.Add(size);
                discs.Add(start);
                discCount++;
            }

            WriteLine(Solve(discs, sizes, discCount)-1);
        }

        public int Solve(List<int> discs, List<int> sizes, int discCount)
        {
            int t = 0;

            while(true)
            {
                while(discs[0] != 0)
                {
                    Step(discs, sizes, discCount);
                    t++;
                }

                List<int> discsCopy = new List<int>();
                discsCopy.AddRange(discs);

                WriteLine($"t: {t}");
                // WriteLine($"  #1: {discs[0]}");
                int i = 1;
                for (; i < discCount; i++)
                {
                    Step(discsCopy, sizes, discCount);
                    // WriteLine($"  #{i+1}: {discsCopy[i]}");
                    if (discsCopy[i] != 0) break;
                }

                if (i == discCount)
                    return t;

                Step(discs, sizes, discCount);
                t++;
            }
        }

        public void Step(List<int> discs, List<int> sizes, int discCount)
        {
            for (int i = 0; i < discCount; i++)
            {
                discs[i]++;
                if (discs[i] >= sizes[i])
                    discs[i] = 0;
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

    public static class MD5
    {
        private static System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();

        public static string Calculate(string input)
        {
            // step 1, calculate MD5 hash from input
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }
    }
}