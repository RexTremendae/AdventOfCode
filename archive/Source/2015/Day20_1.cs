using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using static System.Console;
using static System.ConsoleColor;
using static Day20.ColorWriter;

namespace Day20
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new Program().Run(args);
        }

        public void Run(string[] args)
        {
            long max = 2_000_000;
            var data = new long [max+1];
            long goal = 29_000_000;

            long i = 1;
            for (;i <= max;)
            {
                for (long j = i; j <= max || j < i+50; j += i)
                {
                    data[j] += i*10;
                }

                if (i % 10_000 == 0)
                {
                    WriteLine($"Progress: {i:### ### ### ###} - {data[i]:### ### ### ###}");
                }

                if (data[i] >= goal)
                {
                    WriteLine($"GOAL reached: {i:### ### ### ###} - {data[i]:### ### ### ###}");
                    break;
                }

                i++;
            }
        }

        long[] CloneAndIncrease(long[] data, long max)
        {
            var ret = new long[max];
            for(long i = 0; i < data.Length; i++)
            {
                ret[i] = data[i];
            }

            return ret;
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