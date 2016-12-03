using System;
using System.Collections.Generic;
using System.IO;

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

            int validTriangleCount = 0;
            while(!reader.EndOfStream)
            {
                var input = reader.ReadLine();
                if (string.IsNullOrEmpty(input))
                    break;

                if(CheckTriangle(input))
                {
                    validTriangleCount++;
                }
            }
        
            WriteLine($"# valid triangles: {validTriangleCount}");
        }

        public bool CheckTriangle(string rawInput)
        {
            List<int> sides = new List<int>();

            int idx = 0;
            for (int i = 0; i < 3; i++)
            {
                while(rawInput[idx] == ' ')
                    idx++;
                string sideInput = string.Empty;
                
                while(idx < rawInput.Length && rawInput[idx] != ' ')
                {
                    sideInput += rawInput[idx];
                    idx++;
                }

                sides.Add(int.Parse(sideInput));
            }

            if (sides[0] + sides[1] <= sides[2]) return false;
            if (sides[0] + sides[2] <= sides[1]) return false;
            if (sides[2] + sides[1] <= sides[0]) return false;

            return true;
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