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

            do
            {
                input = Increase(input);                    
            } while (!IsValid(input));

            WriteLine(input);
        }

        bool IsValid(string input)
        {
            bool containsThree = false;
            for (int i = 0; i < input.Length - 2; i++)
            {
                if (input[i]+1 == input[i+1] && input[i+1]+1 == input[i+2])
                    containsThree = true;
            }

            bool containsForbiddenLetters = false;
            if (input.Contains('l') || input.Contains('i') || input.Contains('o')) containsForbiddenLetters = true;

            HashSet<char> doubles = new HashSet<char>();
            for (int i = 0; i < input.Length-1; i++)
            {
                if (input[i] == input[i+1])
                    doubles.Add(input[i]);
            }
            bool containsAtLeastTwoDoubles = doubles.Count() >= 2;

            return containsThree && !containsForbiddenLetters && containsAtLeastTwoDoubles;
        }

        string Increase(string input)
        {
            char[] str = input.ToArray();

            int idx = str.Length-1;

            bool done = false;
            while(!done)
            {
                str[idx]++;
                if (str[idx] > 'z')
                {
                    str[idx] = 'a';
                    idx--;
                    if (idx < 0)
                    {
                        char[] tmp = new char[str.Length+1];
                        for (int i = 0; i < str.Length; i++)
                            tmp[i+1] = str[i];
                        tmp[0] = 'a';

                        return new string(tmp);
                    }
                }
                else
                {
                    done = true;
                }
            }

            return new string(str);
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