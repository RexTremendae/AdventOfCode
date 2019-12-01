using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using static System.Console;

namespace Day17
{
    public class Program
    {
        Queue<Tuple<string, char, Point>> _queue;
        string _seed;

        public static void Main(string[] args)
        {
            new Program().Run(args);
        }

        public void Run(string[] args)
        {
            _queue = new Queue<Tuple<string, char, Point>>();
            IReader reader;
            if (args.Length > 0)
            {
                reader = new FileReader(args[0]);
            }
            else
            {
                reader = new ConsoleReader();
            }

            _seed = reader.ReadLine();
            Enqueue("", new Point(0,0));
            long longestPath = 0;

            while (_queue.Count > 0)
            {
                var current = _queue.Dequeue();
                if (current.Item3.X == 3 && current.Item3.Y == 3)
                {
                    var currentPathLength = current.Item1.Length;
                    if (currentPathLength > longestPath)
                    {
                        longestPath = currentPathLength;
                        //WriteLine(longestPath);
                    }
                }
                else
                    Enqueue(current.Item1, current.Item3);
            }

            WriteLine(longestPath);
        }

        private bool IsOpen(char c)
        {
            return "bcdef".Contains(c);
        }

        private void Enqueue(string visited, Point current)
        {
            string input = _seed + visited;
            var md5 = MD5.Calculate(input);

            Point p;
            char dir;
            if (IsOpen(md5[0]) && current.Y > 0) // Up
            {
                p = new Point(current.X, current.Y-1);
                dir = 'U';
                _queue.Enqueue(Tuple.Create(visited + dir, dir, p));
            }
            if (IsOpen(md5[1]) && current.Y < 3) // Down
            {
                p = new Point(current.X, current.Y+1);
                dir = 'D';
                _queue.Enqueue(Tuple.Create(visited + dir, dir, p));
            }
            if (IsOpen(md5[2]) && current.X > 0) // Left
            {
                p = new Point(current.X-1, current.Y);
                dir = 'L';
                _queue.Enqueue(Tuple.Create(visited + dir, dir, p));
            }
            if (IsOpen(md5[3]) && current.X < 3) // Right
            {
                p = new Point(current.X+1, current.Y);
                dir = 'R';
                _queue.Enqueue(Tuple.Create(visited + dir, dir, p));
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

            return sb.ToString().ToLower();
        }
    }
}