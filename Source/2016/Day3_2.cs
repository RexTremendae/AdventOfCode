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

            int validTriangleCount = 0;
            while(true)
            {
                List<List<int>> inputs = new List<List<int>>();

                for (int i = 0; i < 3; i++)
                {
                    var rawInput = reader.ReadLine();
                    if (string.IsNullOrEmpty(rawInput)) break;
                    inputs.Add(GetSides(rawInput).ToList());
                }

                if (inputs.Count < 3) break;

                for (int i = 0; i < 3; i++)
                {
                    if (CheckTriangle(inputs[0][i], inputs[1][i], inputs[2][i]))
                    {
                        validTriangleCount++;
                    }
                }
            }
        
            WriteLine($"# valid triangles: {validTriangleCount}");
        }

        public IEnumerable<int> GetSides(string input)
        {
            int idx = 0;
            for (int i = 0; i < 3; i++)
            {
                while(input[idx] == ' ')
                    idx++;
                string sideInput = string.Empty;
                
                while(idx < input.Length && input[idx] != ' ')
                {
                    sideInput += input[idx];
                    idx++;
                }

                yield return int.Parse(sideInput);
            }
        }

        public bool CheckTriangle(int a, int b, int c)
        {
            Write($"Checking {a} {b} {c}: ");
            if (a + b <= c) {WriteLine("false");return false;}
            if (b + c <= a) {WriteLine("false");return false;}
            if (a + c <= b) {WriteLine("false");return false;}

            WriteLine("true");
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