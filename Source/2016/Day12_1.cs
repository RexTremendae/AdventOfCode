//#define DEBUG

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static System.Console;

namespace Day2
{
    public class Program
    {
        int[] registers = new int[4];
        int instructionPointer;

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

            List<string[]> instructions = new List<string[]>();
            while (!reader.EndOfStream)
            {
                var input = reader.ReadLine();
                if (string.IsNullOrEmpty(input)) break;

                var split = input.Split(' ');
                instructions.Add(split);
            }

            RunInstructions(instructions);
            WriteLine(registers[0]);
        }

        private void RunInstructions(List<string[]> instructions)
        {
            while(true)
            {
                if (instructionPointer >= instructions.Count)
                    break;

                string[] instr = instructions[instructionPointer];

#if DEBUG
                Write($"Executing [{instructionPointer}] ");
                for(int i = 0; i < instr.Count(); i++)
                    Write($"{instr[i]} ");
#endif

                switch(instr[0])
                {
                    case "cpy":
                        Copy(instr[1], instr[2]);
                        break;

                    case "inc":
                        Increase(instr[1]);
                        break;

                    case "dec":
                        Decrease(instr[1]);
                        break;

                    case "jnz":
                        if (JumpIfNotZero(instr[1], instr[2]))
                            instructionPointer--;
                        break;
                }
#if DEBUG
                WriteLine();
#endif

                instructionPointer++;
            }
        }

        private bool JumpIfNotZero(string compare, string jump)
        {
            int value = 0;
            if (int.TryParse(compare, out value))
            {
                if (value != 0)
                {
                    instructionPointer += int.Parse(jump);
                    return true;
                }
            }
            else
            {
                if (registers[compare[0]-'a'] != 0)
                {
                    instructionPointer += int.Parse(jump);
                    return true;
                }
            }

            return false;
        }

        private void Decrease(string register)
        {
            int r = register[0];
            registers[r-'a']--;

#if DEBUG
            Write($"[{(char)r}: {registers[r-'a']}]");
#endif
        }

        private void Increase(string register)
        {
            char r = register[0];
            registers[r-'a']++;

#if DEBUG
            Write($"[{(char)r}: {registers[r-'a']}]");
#endif
        }

        private void Copy(string from, string to)
        {
            int value;
            if(int.TryParse(from, out value))
            {
                registers[to[0]-'a'] = value;
            }
            else
            {
                registers[to[0]-'a'] = registers[from[0]-'a'];
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