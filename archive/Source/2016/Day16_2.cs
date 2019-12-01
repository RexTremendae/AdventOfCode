using System;
using System.Collections;
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

            BitArray bitArray = new BitArray(size);
            int filledLength = input.Length;

            for (int i = 0; i < input.Length; i ++)
                bitArray[i] = input[i] == '1' ? true : false;

            while (filledLength < size)
            {
                filledLength = Generate(bitArray, filledLength);
            }

//            Print(bitArray);
//            WriteLine($" [{filledLength}]");

            var checksum = bitArray;
            var checksumLength = bitArray.Length;
            checksumLength = CalculateChecksum(checksum, checksumLength);
            while (checksumLength % 2 == 0)
            {
                checksumLength = CalculateChecksum(checksum, checksumLength);
            }

            Print(checksum, checksumLength);
        }

        void Print(BitArray bitArray, int length = 0)
        {
            if (length == 0) length = bitArray.Length;

            for (int i = 0; i < length; i++)
                Write(bitArray[i] ? "1" : "0");
            WriteLine();
        }

        int CalculateChecksum(BitArray input, int length)
        {
            int j = 0;
            for (int i = 0; i < length; i += 2, j++)
            {
                if (input[i] == input[i+1])
                    input[j] = true;
                else
                    input[j] = false;
            }
 
            return j;
        }

        int Generate(BitArray input, int filledLength)
        {
            int newLength = filledLength * 2 + 1;
            int i = 0;

            if (newLength > input.Length)
            {
                i = newLength - input.Length;
                newLength = input.Length;
            }

            for (; i < filledLength; i ++)
            {
                bool b = input[i];
                int pos = 2*filledLength - i;
                input[pos] = !b;
            }

            return newLength;
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