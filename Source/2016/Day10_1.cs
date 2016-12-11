using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static System.Console;

namespace Day2
{
    public enum EntityType
    {
        None = 0,
        Bot,
        Output
    }

    public class Entity
    {
        public Entity(EntityType type, int id)
        {
            Type = type;
            Id = id;
        }

        private int? value1, value2;

        public int High
        {
            get
            {
                if (Type != EntityType.Bot)
                {
                    throw new InvalidOperationException("Only bots have high and low values, this is not a bot!");
                }

                if (!value1.HasValue && !value2.HasValue)
                {
                    throw new InvalidOperationException("Bot doesn't have two values, cannot calculate high and low.");
                }

                return value1.Value > value2.Value ? value1.Value : value2.Value;
            }
        }

        public int Low
        {
            get
            {
                if (Type != EntityType.Bot)
                {
                    throw new InvalidOperationException("Only bots have high and low values, this is not a bot!");
                }

                if (!value1.HasValue && !value2.HasValue)
                {
                    throw new InvalidOperationException("Bot doesn't have two values, cannot calculate high and low.");
                }

                return value1.Value < value2.Value ? value1.Value : value2.Value;
            }
        }

        public bool HasBothValues()
        {
            return value1.HasValue && value2.HasValue;
        }

        public void ResetValues()
        {
            value1 = null;
            value2 = null;
        }

        public int Id { get; }
        public EntityType Type { get; }
        public string Key { get { return $"{Type}_{Id}"; }}

        public int HighReceiverId { get; set; }
        public int LowReceiverId { get; set; }

        public EntityType HighReceiverType { get; set; }
        public EntityType LowReceiverType { get; set; }

        public void NotifyIfGoalBot()
        {
            if (!value1.HasValue || !value2.HasValue)
                return;

            int goal1 = 61;
            int goal2 = 17;

            if ((value1 == goal1 && value2 == goal2) || (value1 == goal2 && value2 == goal1))
            {
                WriteLine($"=====  Bot #{Id} is comparing microchips with values {goal1} and {goal2}.  ====");
            }
        }

        public void AddValue(int value, string sender)
        {
            //WriteLine($"{Key} got value {value} added from {sender}.");
            switch (Type)
            {
                case EntityType.Bot:
                    if (!value1.HasValue)
                        value1 = value;
                    else if (!value2.HasValue)
                        value2 = value;
                    else
                        throw new InvalidOperationException($"Bot {Id} already got two values!");
                    break;

                case EntityType.Output:
                    if (!value1.HasValue)
                        value1 = value;
                    else
                        throw new InvalidOperationException($"Output {Id} already got one value!");
                    break;

                default:
                    throw new InvalidOperationException($"Invalid entity type {Type}.");
            }
        }
    }

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

            Dictionary<string, Entity> entities = new Dictionary<string, Entity>();
            while (!reader.EndOfStream)
            {
                var input = reader.ReadLine();
                if (string.IsNullOrEmpty(input)) break;

                ParseInput(input, entities);
            }

            while(Process(entities));
        }

        private bool Process(Dictionary<string, Entity> entities)
        {
            bool anyChanges = false;
            foreach (var kvp in entities)
            {
                var bot = kvp.Value;
                if (bot.Type != EntityType.Bot)
                {
                    continue;
                }

                bot.NotifyIfGoalBot();

                if (bot.HasBothValues())
                {
                    var highReceiver = entities[$"{bot.HighReceiverType}_{bot.HighReceiverId}"];
                    highReceiver.AddValue(bot.High, bot.Key);

                    var lowReceiver = entities[$"{bot.LowReceiverType}_{bot.LowReceiverId}"];
                    lowReceiver.AddValue(bot.Low, bot.Key);

                    bot.ResetValues();
                    anyChanges = true;
                }
            }

            return anyChanges;
        }

        private void ParseInput(string input, Dictionary<string, Entity> entities)
        {
            var split = input.Split(' ');
            if (split[0] == "value")
            {
                int value = int.Parse(split[1]);
                int botId = int.Parse(split[5]);

                var bot = new Entity(EntityType.Bot, botId);
                if (!entities.TryGetValue(bot.Key, out bot))
                {
                    bot = new Entity(EntityType.Bot, botId);
                    entities.Add(bot.Key, bot);
                }

                bot.AddValue(value, "Input");
            }
            else if (split[0] == "bot")
            {
                int botId = int.Parse(split[1]);

                var bot = new Entity(EntityType.Bot, botId);
                if (!entities.TryGetValue(bot.Key, out bot))
                {
                    bot = new Entity(EntityType.Bot, botId);
                    entities.Add(bot.Key, bot);
                }

                string lowTypeRaw = split[5];
                string highTypeRaw = split[10];

                bot.LowReceiverType = (EntityType)Enum.Parse(typeof(EntityType), lowTypeRaw, true);
                bot.LowReceiverId = int.Parse(split[6]);

                bot.HighReceiverType = (EntityType)Enum.Parse(typeof(EntityType), highTypeRaw, true);
                bot.HighReceiverId = int.Parse(split[11]);

                if (bot.LowReceiverType == EntityType.Output)
                {
                    var output = new Entity(EntityType.Output, bot.LowReceiverId);
                    entities.Add(output.Key, output);
                }

                if (bot.HighReceiverType == EntityType.Output)
                {
                    var output = new Entity(EntityType.Output, bot.HighReceiverId);
                    entities.Add(output.Key, output);
                }
            }
            else
            {
                throw new NotImplementedException($"Value {split[0]} is not implemented.");
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