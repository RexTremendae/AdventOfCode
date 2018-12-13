using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static System.Console;

namespace Day23
{
    public class Program
    {
        public static void Main()
        {
            new Program().Run();
        }

        public void Run()
        {
            var program = ParseInput("Day23.txt").ToList();

            long a = 1, b = 0;
            int idx = 0;
            var exitProgram = false;

            while (!exitProgram)
            {
                if (idx < 0)
                {
                    WriteLine("Instruction pointer below 0.");
                    break;
                }
                if (idx >= program.Count)
                {
                    WriteLine("Instruction pointer above program length.");
                    break;
                }

                var instr = program[idx].instruction.ToLower();
                var args = program[idx].arguments.Select(x => x.ToLower()).ToList();

                WriteLine($"Executing {instr} ({String.Join(", ", args)})");

                switch (instr.ToLower())
                {
                    case "hlf":
                        if (args[0].ToLower() == "a") a /= 2;
                        if (args[0].ToLower() == "b") b /= 2;
                        break;

                    case "tpl":
                        if (args[0].ToLower() == "a") a *= 3;
                        if (args[0].ToLower() == "b") b *= 3;
                        break;

                    case "inc":
                        if (args[0].ToLower() == "a") a++;
                        if (args[0].ToLower() == "b") b++;
                        break;

                    case "jmp":
                    {
                        var offset = int.Parse(args[0]) - 1;
                        idx += offset;
                        break;
                    }

                    case "jio":
                    {
                        var offset = int.Parse(args[1]) - 1;
                        if (args[0].ToLower() == "a" && a == 1) idx += offset;
                        else if (args[0].ToLower() == "b" && b == 1) idx += offset;
                        break;
                    }

                    case "jie":
                    {
                        var offset = int.Parse(args[1]) - 1;
                        if (args[0].ToLower() == "a" && a%2 == 0) idx += offset;
                        else if (args[0].ToLower() == "b" && b%2 == 0) idx += offset;
                        break;
                    }

                    default:
                        WriteLine($"Unrecognized instruction: {instr}");
                        exitProgram = true;
                        break;
                }

                if (exitProgram) break;
                idx++;
            }

            WriteLine($"A: {a}");
            WriteLine($"B: {b}");
        }

        private IEnumerable<(string instruction, string[] arguments)> ParseInput(string filepath)
        {
            foreach (var line in File.ReadAllLines(filepath))
            {
                if (string.IsNullOrEmpty(line)) { continue; }

                var instruction = line.Substring(0, 3);
                var args = line.Substring(4)
                               .Split(",", StringSplitOptions.RemoveEmptyEntries)
                               .Select(x => x.Trim());
                
                yield return (instruction, args.ToArray());
            }
        }
    }
}