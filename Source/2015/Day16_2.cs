using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static System.Console;

namespace Day16
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

            Dictionary<string, int> propertiesToLookFor = new Dictionary<string, int>()
            {
                { "children", 3 },
                { "cats", 7 },
                { "samoyeds", 2 },
                { "pomeranians", 3 },
                { "akitas", 0 },
                { "vizslas", 0 },
                { "goldfish", 5 },
                { "trees", 3 },
                { "cars", 2 },
                { "perfumes", 1 }
            };

            int auntCounter = 1;
            while (!reader.EndOfStream)
            {
                var input = reader.ReadLine();
                if (string.IsNullOrEmpty(input)) break;

                var propertiesStartIndex = input.IndexOf(':') + 2;
                input = input.Substring(propertiesStartIndex, input.Length - propertiesStartIndex);
                var properties = input.Split(',');

                bool isThisAunt = true;
                //WriteLine($"[Aunt #{auntCounter}] ");
                foreach (var p in properties)
                {
                    var pp = p.Split(':');
                    var propertyName = pp[0].Trim();
                    var propertyValue = int.Parse(pp[1]);
                    //WriteLine($"  {propertyName}: {propertyValue}");

                    if (propertyName == "cats" || propertyName == "trees")
                    {
                        if (propertiesToLookFor[propertyName] >= propertyValue)
                        {
                            isThisAunt = false;
                            break;
                        }
                    }
                    else if (propertyName == "pomeranians" || propertyName == "goldfish")
                    {
                        if (propertiesToLookFor[propertyName] <= propertyValue)
                        {
                            isThisAunt = false;
                            break;
                        }
                    }
                    else if (propertiesToLookFor[propertyName] != propertyValue)
                    {
                        isThisAunt = false;
                        break;
                    }
                }

                if (isThisAunt)
                {
                    WriteLine($"Gift is from aunt {auntCounter}");
                }
                auntCounter++;
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
}