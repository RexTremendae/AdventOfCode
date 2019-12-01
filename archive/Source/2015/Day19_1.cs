using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using static System.Console;
using static System.ConsoleColor;
using static Day19.ColorWriter;

namespace Day19
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

            bool replacements = true;
            var machine = new Machine();
            var inputs = new List<string>();

            while (!reader.EndOfStream)
            {
                var input = reader.ReadLine();
                if (string.IsNullOrEmpty(input))
                {
                    replacements = false;
                    continue;
                }

                if (replacements)
                {
                    machine.AddReplacement(input);
                }
                else
                {
                    inputs.Add(input.Trim());
                }
            }

            foreach (var input in inputs)
            {
                WriteLine("----");
                WriteLine("Input: " + input);
                WriteLine();
                WriteLine("Distinct molecules: " + Execute(machine, input));
            }
        }

        public int Execute(Machine machine, string input)
        {
            var distincts = new HashSet<string>();

            for (int i = 0; i < input.Length; i++)
            {
                string part = input[i].ToString();
                var replacements = machine.GetReplacements(part);
                if (!replacements.Any())
                {
                    if (i+1 < input.Length)
                        part += input[i+1];
                    else
                        continue;

                    replacements = machine.GetReplacements(part);
                }

                if (!replacements.Any())
                    continue;

//                WriteLine("Part: " + part);

                var firstHalf = input.Substring(0, i);
                var secondHalf = string.Empty;
                int cutLength = i+part.Length;
                if (cutLength < input.Length)
                {
                    secondHalf = input.Substring(cutLength, input.Length - cutLength);
                }

//                WriteLine(firstHalf + " * " + secondHalf);
//                WriteLine();

                foreach (var rpl in replacements)
                {
                    distincts.Add(firstHalf + rpl + secondHalf);
                }
            }

            return distincts.Count;
        }
    }

    public class Machine
    {
        private Dictionary<string, List<string>> _replacements;

        public Machine()
        {
            _replacements = new Dictionary<string, List<string>>();
        }

        public void AddReplacement(string rawReplacement)
        {
            var split = rawReplacement.Split(' ');
            if (split.Length != 3) return;

            var key = split[0];
            var value = split[2];

            if (!_replacements.TryGetValue(key, out var valueCollection))
            {
                valueCollection = new List<string>();
                _replacements.Add(key, valueCollection);
            }

            valueCollection.Add(value);
        }

        public IEnumerable<string> GetReplacements(string key)
        {
            if (!_replacements.TryGetValue(key, out var values))
            {
                return new string[]{};
            }

            return values;
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