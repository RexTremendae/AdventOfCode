using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static System.Console;

namespace Day2
{
    public class Entity
    {
        public Entity(string data, string checksum, int sector)
        {
            Data = data;
            Checksum = checksum;
            Sector = sector;
        }

        public string Data {get;}
        public string Checksum {get;}
        public int Sector {get;}
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

            while (!reader.EndOfStream)
            {    
                var input = reader.ReadLine();
                if (string.IsNullOrEmpty(input)) break;
                
                var entity = ParseInput(input);
                //WriteLine($"{entity.Data}, {entity.Sector} [{entity.Checksum}]");
                if(IsReal(entity)) WriteLine($"{Decrypt(entity)} ({entity.Sector})");
            }
        }

        public string Decrypt(Entity entity)
        {
            string decrypted = string.Empty;
            foreach (var c in entity.Data)
            {
                if (c == '-')
                {
                    decrypted += ' ';
                    continue;
                }

                var cc = c;
                for (int i = 0; i < entity.Sector; i++)
                {       
                    cc++;
                    if (cc > 'z') cc = 'a';
                }

                decrypted += cc;
            }

            return decrypted;
        }

        public bool IsReal(Entity entity)
        {
            Dictionary<char, int> frequencies = new Dictionary<char, int>();

            foreach (var c in entity.Data)
            {
                if (c == '-') continue;

                if (!frequencies.ContainsKey(c))
                {
                    frequencies.Add(c, 0);
                }

                frequencies[c]++;
            }

            var mostFrequent = frequencies.Keys.OrderByDescending(x => frequencies[x]).ThenBy(x => x).ToList();
            var tmp = string.Empty;
            for (int i = 0; i < 5; i++) tmp += mostFrequent[i];

            return tmp == entity.Checksum;
        }

        public Entity ParseInput(string input)
        {
            int sectorStartIndex = input.IndexOfAny(new char[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'});
            int i;
            string data = string.Empty;
            for (i = 0; i < sectorStartIndex; i++)
            {
                data += input[i];
            }

            string sectorStr = string.Empty;
            while (input[i] != '[')
            {
                sectorStr += input[i++];
            }
            int sector = int.Parse(sectorStr);

            i++;
            string checksum = string.Empty;
            while (input[i] != ']')
            {
                checksum += input[i++];
            }

            return new Entity(data, checksum, sector);
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