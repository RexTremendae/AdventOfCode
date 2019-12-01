using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
 
using static System.Console;
 
namespace Day16
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
 
            var size = int.Parse(reader.ReadLine());
            var input = reader.ReadLine();
 
            while (input.Length < size)
            {
                input = Generate(input);
            }
 
            input = input.Substring(0, size);
 
            var checksum = CalculateChecksum(input);
            while (checksum.Length % 2 == 0)
            {
                checksum = CalculateChecksum(checksum);
            }
 
            WriteLine(checksum);
        }
 
        string CalculateChecksum(string input)
        {
            StringBuilder checksum = new StringBuilder();
 
            for (int i = 0; i < input.Length; i += 2)
            {
                if (input[i] == input[i+1])
                    checksum.Append('1');
                else
                    checksum.Append('0');
            }
 
            return checksum.ToString();
        }
 
        string Generate(string input)
        {
            var a = input;
 
            StringBuilder b = new StringBuilder();
            foreach (var c in input)
                b.Insert(0, c == '1' ? '0' : '1');
 
            return a + '0' + b;
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