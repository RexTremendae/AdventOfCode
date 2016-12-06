using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static System.Console;

namespace Day2
{
    public class Program
    {
        class RainDeer
        {
            public string Name;
            public int Speed;
            public int RaceTime;
            public int RestTime;
        }

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

            List<RainDeer> raindeers = new List<RainDeer>();
            //int time = int.Parse(reader.ReadLine());
            int time = 2503;

            while (!reader.EndOfStream)
            {
                var input = reader.ReadLine();
                if (string.IsNullOrEmpty(input)) break;

                var split = input.Split(' ');
                var deer = new RainDeer()
                {
                    Name = split[0],
                    Speed = int.Parse(split[3]),
                    RaceTime = int.Parse(split[6]),
                    RestTime = int.Parse(split[13])
                };

                raindeers.Add(deer);
            }

            long maxDistance = 0;
            string winner = string.Empty;
            foreach(var deer in raindeers)
            {
                var distance = CalculateDistance(deer, time);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    winner = deer.Name;
                }
                WriteLine($"{deer.Name} traveled {distance} km.");
            }

            WriteLine();
            WriteLine($"Winner: {winner} with {maxDistance} km.");
        }

        long CalculateDistance(RainDeer deer, int time)
        {
            long totalDistance = 0;

            while (time > 0)
            {
                var timeToRace = time < deer.RaceTime ? time : deer.RaceTime;
                totalDistance += deer.Speed * timeToRace;
                time -= timeToRace;

                time -= deer.RestTime;
            }

            return totalDistance;
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