using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static System.Console;

namespace Day14
{
    public class Program
    {
        class RainDeer
        {
            public RainDeer()
            {
                Statistics = new Statistics();
            }

            public string Name;
            public int Speed;
            public int RaceTime;
            public int RestTime;

            public Statistics Statistics;
        }

        class Statistics
        {
            public int Score;
            public long TotalDistance;
            public int CurrentRestTime;
            public int CurrentRaceTime;
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

            for (int i = 0; i < time; i++)
            {
                foreach (var deer in raindeers)
                {
                    if (deer.Statistics.CurrentRaceTime >= deer.RaceTime)
                    {
                        deer.Statistics.CurrentRestTime++;
                        if (deer.Statistics.CurrentRestTime >= deer.RestTime)
                        {
                            deer.Statistics.CurrentRaceTime = 0;
                            deer.Statistics.CurrentRestTime = 0;
                        }
                    }
                    else
                    {
                        deer.Statistics.CurrentRaceTime++;
                        deer.Statistics.TotalDistance += deer.Speed;
                    }
                }

                var ordered = raindeers.OrderByDescending(x => x.Statistics.TotalDistance);
                var max = ordered.First().Statistics.TotalDistance;

                foreach (var deer in ordered)
                {
                    if (deer.Statistics.TotalDistance == max)
                    {
                        deer.Statistics.Score++;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            var raceResult = raindeers.OrderByDescending(x => x.Statistics.Score);

            foreach (var deer in raceResult)
            {
                WriteLine($"{deer.Name} scored {deer.Statistics.Score}.");
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
}