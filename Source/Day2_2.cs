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
            string keypad = 
                "*******" +
                "***1***" +
                "**234**" +
                "*56789*" +
                "**ABC**" +
                "***D***" +
                "*******";

            if (args.Length > 0)
            {
                reader = new FileReader(args[0]);
            }
            else
            {
                reader = new ConsoleReader();
            }

            Point p = new Point { X = 1, Y = 3 };
            List<char> code = new List<char>();

            while(!reader.EndOfStream)
            {
                var input = reader.ReadLine();
                if (string.IsNullOrEmpty(input))
                    break;

                foreach(var c in input)
                {
                    Point newP = new Point { X = p.X, Y = p.Y };
                    switch(c.ToString().ToUpper()[0])
                    {
                        case 'D':
                            newP.Y++;
                            break;
                        case 'U':
                            newP.Y--;
                            break;
                        case 'L':
                            newP.X--;
                            break;
                        case 'R':
                            newP.X++;
                            break;
                    }

                    int newIndex = newP.Y*7 + newP.X;
                    if (keypad[newIndex] != '*')
                        p = newP;
                }

                code.Add(keypad[p.Y*7 + p.X]);
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