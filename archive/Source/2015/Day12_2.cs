using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static System.Console;

namespace Day12
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

            while (!reader.EndOfStream)
            {
                var input = reader.ReadLine();
                long sum = Parser.Parse(input);
                WriteLine(sum);
            }
        }
    }

    public class Parser
    {
        public static long Parse(string input)
        {
            return Parse(input, 0).sum;
        }

        private static (long sum, int index, bool isRed) Parse(string input, int index)
        {
            int startIndex = index;

            if (input[index] == '{')
            {
                var result = ParseObject(input, index);
                return (result.sum, result.index, false);
            }
            else if (input[index] == '[')
            {
                var result = ParseArray(input, index);
                return (result.sum, result.index, false);
            }
            else if (IsNumeric(input[index]))
            {
                var result = ParseNumeric(input, index);
                return (result.sum, result.index, false);
            }
            else if (input[index] == '\"')
            {
                index++;
                string attrValue = "";
                while (input[index] != '\"')
                {
                    attrValue += input[index];
                    index++;
                }

                bool isRed = false;
                //WriteLine($"String value: {attrValue}");
                if (attrValue == "red")
                    isRed = true;
                return (0, index, isRed);
            }

            WriteLine($"ERROR: at index {index} (start at {startIndex})!");
            return (0, index, false);
        }

        private static (long sum, int index) ParseObject(string input, int index)
        {
            int startIndex = index;
            long sum = 0;
            index++;
            bool isRed = false;

            while (true)
            {
                if (index >= input.Length)
                {
                    WriteLine($"ERROR: Could not find end of object starting at index {startIndex}.");
                    return (0, index);
                }

                if (input[index] == '}')
                {
                    if (isRed) sum = 0;
                    return (sum, index);
                }
                else if (input[index] == ' ' || input[index] == ',')
                {
                }
                else if (input[index] == '\"')
                {
                    string attrName = "";
                    index++;
                    while (input[index] != '\"')
                    {
                        attrName += input[index];
                        index++;
                    }
                    //WriteLine($"Found attribute {attrName}.");
                    index++;

                    if (input[index] != ':')
                    {
                        WriteLine($"ERROR: Expected ':' at index {index}.");
                        return (0, index);
                    }
                    index++;

                    var result = Parse(input, index);
                    isRed |= result.isRed;
                    sum += result.sum;
                    index = result.index;
                }
                else
                {
                    var result = Parse(input, index);
                    sum += result.sum;
                    index = result.index;
                }
                index++;
            }
        }

        private static (long sum, int index) ParseArray(string input, int index)
        {
            int startIndex = index;
            long sum = 0;
            index++;

            while (true)
            {
                if (index >= input.Length)
                {
                    WriteLine($"ERROR: Could not find end of array starting at index {startIndex}.");
                    return (0, index);
                }

                if (input[index] == ']')
                {
                    return (sum, index);
                }
                else if (input[index] == ' ' || input[index] == ',')
                {
                }
                else
                {
                    var result = Parse(input, index);
                    sum += result.sum;
                    index = result.index;
                }
                index++;
            }
        }

        private static (long sum, int index) ParseNumeric(string input, int index)
        {
            string number = string.Empty;

            while (IsNumeric(input[index]))
            {
                number += input[index];
                index++;
            }

            return (int.Parse(number), index-1);
        }

        private static bool IsNumeric(char c)
        {
            return c == '-' || (c >= '0' && c <= '9');
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