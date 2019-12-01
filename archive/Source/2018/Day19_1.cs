using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static System.Console;

namespace Day16
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var instructionDefinitions = GetInstructions();
            var registers = new[] {0, 0, 0, 0, 0, 0};
            var (program, ip) = ParseInput("Day19.txt");

            WriteLine("Starting executing program...");
            while (true)
            {
                /*
                foreach (var reg in registers)
                {
                    Write($"{reg} ");
                }
                WriteLine();
                */

                if (registers[ip] < 0 || registers[ip] >= program.Count)
                {
                    WriteLine("Program halt!");
                    break;
                }

                var current = program[registers[ip]];
                //WriteLine($"Executing instruction at {registers[ip]} ({current.opcode} {Utilities.ToString(current.args)})...");

                registers = instructionDefinitions[current.opcode].Execute(registers, current.args[0], current.args[1], current.args[2]);
                registers[ip]++;
            }

            foreach (var reg in registers)
            {
                Write($"{reg} ");
            }
            WriteLine();
        }

        public static (List<(string opcode, int[] args)> instructions, int ip) ParseInput(string filepath)
        {
            bool first = true;
            int ip = 0;
            var instructions = new List<(string, int[])>();

            foreach (var lineRaw in File.ReadAllLines(filepath))
            {
                var line = lineRaw.Trim();

                if (string.IsNullOrEmpty(line)) continue;

                if (first)
                {
                    first = false;
                    ip = (int)line.Last() - '0';
                    continue;
                }

                var split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (split.Length < 4) continue;

                var args = split.Skip(1).Select(x => int.Parse(x)).ToArray();
                instructions.Add((split.First(), args));
            }

            return (instructions, ip);
        }

        public static int[] Parse(string rawData)
        {
            var noBrackets = rawData.Substring(1, rawData.Length - 2);
            var data = noBrackets.Split(',');
            var result = new int[data.Length];
            for (int i = 0; i < data.Length; i ++)
            {
                result[i] = int.Parse(data[i]);
            }
            return result;
        }

        public static Dictionary<string, IInstruction> GetInstructions()
        {
            var instructions = new List<IInstruction>();
            var instructionType = typeof(IInstruction);
            foreach (var type in instructionType.Assembly
                .GetTypes()
                .Where(t => instructionType.IsAssignableFrom(t))
                .Where(t => !t.IsAbstract))
            {
                var instance = Activator.CreateInstance(type) as IInstruction;
                instructions.Add(instance);
            }

            return instructions.ToDictionary(key => key.Identifier);
        }
    }

    public interface IInstruction
    {
        string Identifier { get; }
        int[] Execute(int[] registers, int a, int b, int c);
    }

    public class AddRegister : IInstruction
    {
        public string Identifier => "addr";
        public int[] Execute(int[] registers, int a, int b, int c)
        {
            registers[c] = registers[a] + registers[b];
            return registers;
        }
    }

    public class AddImmediate : IInstruction
    {
        public string Identifier => "addi";
        public int[] Execute(int[] registers, int a, int b, int c)
        {
            registers[c] = registers[a] + b;
            return registers;
        }
    }

    public class MultiplyRegister : IInstruction
    {
        public string Identifier => "mulr";
        public int[] Execute(int[] registers, int a, int b, int c)
        {
            registers[c] = registers[a] * registers[b];
            return registers;
        }
    }

    public class MultiplyImmediate : IInstruction
    {
        public string Identifier => "muli";
        public int[] Execute(int[] registers, int a, int b, int c)
        {
            registers[c] = registers[a] * b;
            return registers;
        }
    }

    public class SetRegister : IInstruction
    {
        public string Identifier => "setr";
        public int[] Execute(int[] registers, int a, int b, int c)
        {
            registers[c] = registers[a];
            return registers;
        }
    }

    public class SetImmediate : IInstruction
    {
        public string Identifier => "seti";
        public int[] Execute(int[] registers, int a, int b, int c)
        {
            registers[c] = a;
            return registers;
        }
    }

    public class GreaterThanImmediateRegister : IInstruction
    {
        public string Identifier => "gtir";
        public int[] Execute(int[] registers, int a, int b, int c)
        {
            if (a > registers[b])
            {
                registers[c] = 1;
            }
            else
            {
                registers[c] = 0;
            }
            return registers;
        }
    }

    public class GreaterThanRegisterImmediate : IInstruction
    {
        public string Identifier => "gtri";
        public int[] Execute(int[] registers, int a, int b, int c)
        {
            if (registers[a] > b)
            {
                registers[c] = 1;
            }
            else
            {
                registers[c] = 0;
            }
            return registers;
        }
    }

    public class GreaterThanRegisterRegister : IInstruction
    {
        public string Identifier => "gtrr";
        public int[] Execute(int[] registers, int a, int b, int c)
        {
            if (registers[a] > registers[b])
            {
                registers[c] = 1;
            }
            else
            {
                registers[c] = 0;
            }
            return registers;
        }
    }

    public class EqualImmediateRegister : IInstruction
    {
        public string Identifier => "eqir";
        public int[] Execute(int[] registers, int a, int b, int c)
        {
            if (a == registers[b])
            {
                registers[c] = 1;
            }
            else
            {
                registers[c] = 0;
            }

            return registers;
        }
    }

    public class EqualRegisterImmediate : IInstruction
    {
        public string Identifier => "eqri";
        public int[] Execute(int[] registers, int a, int b, int c)
        {
            if (registers[a] == b)
            {
                registers[c] = 1;
            }
            else
            {
                registers[c] = 0;
            }
            return registers;
        }
    }

    public class EqualRegisterRegister : IInstruction
    {
        public string Identifier => "eqrr";
        public int[] Execute(int[] registers, int a, int b, int c)
        {
            if (registers[a] == registers[b])
            {
                registers[c] = 1;
            }
            else
            {
                registers[c] = 0;
            }
            return registers;
        }
    }

    public static class Utilities
    {
        public static string ToString(int[] input)
        {
            return $"[{string.Join(", ", input)}]";
        }

        public static int[] Clone(int[] input)
        {
            var result = new int[input.Length];
            for (int i = 0; i < input.Length; i++)
                result[i] = input[i];
            return result;
        }
    }
}