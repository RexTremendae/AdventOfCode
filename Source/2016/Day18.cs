using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

            // Part 1
            // int totalRows = 40;

            // Part 2
            int totalRows = 400000;

            var input = reader.ReadLine();
            int safeTiles = GetSafeTiles(input);
            for (int i = 0; i < totalRows-1; i ++)
            {
                input = NextLine(input);
                //WriteLine(input);
                safeTiles += GetSafeTiles(input);
            }

            WriteLine(safeTiles);
        }

        int GetSafeTiles(string input)
        {
            int sum = 0;
            for (int i = 0; i < input.Length; i++)
                if (input[i] == '.') sum++;
            
            return sum;
        }

        string NextLine(string input)
        {
            input = "." + input + ".";
            StringBuilder sb = new StringBuilder();

            for (int i = 1; i < input.Length - 1; i++)
            {
                char left = input[i-1];
                char center = input[i];
                char right = input[i+1];
                
                if (left == '^' && center == '^' && right == '.')
                    sb.Append('^');
                else if (right == '^' && center == '^' && left == '.')
                    sb.Append('^');
                else if (left == '^' && center == '.' && right == '.')
                    sb.Append('^');
                else if (right == '^' && center == '.' && left == '.')
                    sb.Append('^');
                else
                    sb.Append('.');
            }

            return sb.ToString();
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