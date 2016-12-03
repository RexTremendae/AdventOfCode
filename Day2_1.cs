using System;
using System.Collections.Generic;
using System.IO;

namespace Day2
{
    public struct Point
    {
        public int X;
        public int Y;
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

            Point p = new Point { X = 1, Y = 1 };
            List<int> code = new List<int>();

            while(!reader.EndOfStream)
            {
                var input = reader.ReadLine();
                if (string.IsNullOrEmpty(input))
                    break;

                foreach(var c in input)
                {
                    switch(c.ToString().ToUpper()[0])
                    {
                        case 'D':
                            p.Y++;
                            if (p.Y > 2) p.Y = 2;
                            break;
                        case 'U':
                            p.Y--;
                            if (p.Y < 0) p.Y = 0;
                            break;
                        case 'L':
                            p.X--;
                            if (p.X < 0) p.X = 0;
                            break;
                        case 'R':
                            p.X++;
                            if (p.X > 2) p.X = 2;
                            break;
                    }
                }
                code.Add(p.Y*3 + p.X + 1);
            }

            foreach (var c in code)
                Console.Write(c);
            Console.WriteLine();
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