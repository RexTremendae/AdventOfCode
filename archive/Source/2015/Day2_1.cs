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
            
            int totalArea = 0;
            while(!reader.EndOfStream)
            {
                var input = reader.ReadLine();
                if (string.IsNullOrEmpty(input)) break;

                var dim = input.Split('x').Select(int.Parse).ToList();

                List<int> sides = new List<int>();
                sides.Add(dim[0] * dim[1]);
                sides.Add(dim[0] * dim[2]);
                sides.Add(dim[1] * dim[2]);

                int smallest = int.MaxValue;
                foreach(var s in sides)
                    if (smallest > s)
                        smallest = s;

                var area = 2*sides[0] + 2*sides[1] + 2*sides[2] + smallest;
                totalArea += area;
            }

            WriteLine(totalArea);
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