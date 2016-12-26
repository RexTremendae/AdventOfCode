using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
 
using static System.Console;
using static System.ConsoleColor;
using static Day21.ColorWriter;

namespace Day21
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
 
            StringBuilder sb = new StringBuilder("abcdefgh");
            int opIdx = 0;
            while (!reader.EndOfStream)
            {
                var input = reader.ReadLine();
                if (string.IsNullOrEmpty(input)) break;
 
                string indata = sb.ToString();
                try
                {
                    DoOperation(sb, input);
                    //PrintResult(sb.ToString(), opIdx);
                    opIdx++;
                }
                catch
                {
                    WriteLine($"Error at operation {opIdx}, performed on data '{indata}'");
                    throw;
                }
            }
            WriteLine(sb.ToString());
        }

        void PrintResult(string data, int opIdx)
        {
            Write("After operation ");
            Write(opIdx.ToString(), Cyan);
            Write(": ");
            WriteLine(data, Green);
        }

        void DoOperation(StringBuilder sb, string input)
        {
            var split = input.Split(' ');
            if (split[0] == "swap")
            {
                if (split[1] == "position")
                {
                    int posX = int.Parse(split[2]);
                    int posY = int.Parse(split[5]);
 
                    char tmp = sb[posX];
                    sb[posX] = sb[posY];
                    sb[posY] = tmp;
                }
                else if (split[1] == "letter")
                {
                    char letterX = split[2][0];
                    char letterY = split[5][0];

                    char tempReplacement = '¤';

                    sb.Replace(letterY, tempReplacement);
                    sb.Replace(letterX, letterY);
                    sb.Replace(tempReplacement, letterX);
                }
                else
                {
                    WriteLine("Invalid swap operation!");
                    Environment.Exit(1);
                }
            }
            else if (split[0] == "rotate")
            {
                if (split[1] == "based")
                {
                    int len = sb.Length;
                    char letter = split[6][0];
                    int i;
                    for (i = 0; i < len; i++)
                    {
                        if (sb[i] == letter) break;
                    }
                    //if (i >= len) i = 0;
 
                    int steps = 1 + i;
                    if (i >= 4) steps++;
 
                    while (steps > len) steps -= len;
 
                    sb.Insert(0, sb.ToString(len - steps, steps));
                    sb.Remove(len, steps);
                }
                else if (split[1] == "right" || split[1] == "left")
                {
                    string direction = split[1];
                    int steps = int.Parse(split[2]);
 
                    if (direction == "left")
                    {
                        sb.Append(sb.ToString(0, steps));
                        sb.Remove(0, steps);
                    }
                    else
                    {
                        int len = sb.Length;
                        sb.Insert(0, sb.ToString(sb.Length - steps, steps));
                        sb.Remove(len, steps);
                    }
                }
                else
                {
                    WriteLine("Invalid rotate operation!");
                    Environment.Exit(1);
                }
            }
            else if (split[0] == "reverse")
            {
                int posX = int.Parse(split[2]);
                int posY = int.Parse(split[4]);
 
                for (int i = 0; i <= (posY-posX)/2; i++)
                {
                    char tmp = sb[posX + i];
                    sb[posX + i] = sb[posY - i];
                    sb[posY - i] = tmp;
                }
            }
            else if (split[0] == "move")
            {
                int posX = int.Parse(split[2]);
                int posY = int.Parse(split[5]);
 
                char c = sb[posX];
                sb.Remove(posX, 1);
                if (sb.Length <= posY)
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Insert(posY, c);
                }
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

    public static class ColorWriter
    {
        public static void Write(string text, ConsoleColor color)
        {
            var oldColor = ForegroundColor;
            ForegroundColor = color;
            Console.Write(text);
            ForegroundColor = oldColor;
        }

        public static void WriteLine(string text, ConsoleColor color)
        {
            Write(text, color);
            Console.WriteLine();
        }
    }
}