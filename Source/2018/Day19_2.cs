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
            //new Program().RunOptimized(974);
            new Program().RunOptimized(10551374);
            //new Program().Run();
        }

        private void RunOptimized(long maxCount)
        {
            var reg = new long[6];

            for (reg[4] = 1; reg[4] <= maxCount; reg[4] ++)
            {
                if (maxCount % reg[4] == 0) reg[0] += reg[4];
            }

            WriteLine(reg[0]);
        }

        private void Run()
        {
            var instructionDefinitions = GetInstructions();
            var registers = new long[] 
            //{1, 0, 0, 0, 0, 0};
            //{0, 21102744, 4, 10551374, 2, 10551372};
            {3,976,4,974,2,488};
            var (program, ip) = ParseInput("Day19.txt");

            WriteLine("Starting executing program...");
            var lastLines = new List<long[]>();
            int max = 1_000;
            //while (max-- >= 0)
            while (true)
            {
                if (registers[ip] < 0 || registers[ip] >= program.Count)
                {
                    WriteLine("Program halt!");
                    break;
                }

                var current = program[(int)registers[ip]];
                //WriteLine($"Executing instruction at {registers[ip]} ({current.opcode} {Utilities.ToString(current.args)})...");

                registers = instructionDefinitions[current.opcode].Execute(registers, current.args[0], current.args[1], current.args[2]);
                registers[ip]++;
                lastLines.Add(Utilities.Clone(registers));
                if (lastLines.Count > max) lastLines.RemoveAt(0);

                if (registers[0] > 3) break;
            }

            foreach (var line in lastLines)
            {
                foreach (var reg in line)
                {
                    Write($"{reg.ToString().PadLeft(12)}");
                }
                WriteLine();
            }
        }

        public static (List<(string opcode, long[] args)> instructions, long ip) ParseInput(string filepath)
        {
            bool first = true;
            long ip = 0;
            var instructions = new List<(string, long[])>();

            foreach (var lineRaw in File.ReadAllLines(filepath))
            {
                var line = lineRaw.Trim();

                if (string.IsNullOrEmpty(line)) continue;

                if (first)
                {
                    first = false;
                    ip = (long)line.Last() - '0';
                    continue;
                }

                var split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (split.Length < 4) continue;

                var args = split.Skip(1).Select(x => long.Parse(x)).ToArray();
                instructions.Add((split.First(), args));
            }

            return (instructions, ip);
        }

        public static long[] Parse(string rawData)
        {
            var noBrackets = rawData.Substring(1, rawData.Length - 2);
            var data = noBrackets.Split(',');
            var result = new long[data.Length];
            for (int i = 0; i < data.Length; i ++)
            {
                result[i] = long.Parse(data[i]);
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
        long[] Execute(long[] registers, long a, long b, long c);
    }

    public class AddRegister : IInstruction
    {
        public string Identifier => "addr";
        public long[] Execute(long[] registers, long a, long b, long c)
        {
            registers[c] = registers[a] + registers[b];
            return registers;
        }
    }

    public class AddImmediate : IInstruction
    {
        public string Identifier => "addi";
        public long[] Execute(long[] registers, long a, long b, long c)
        {
            registers[c] = registers[a] + b;
            return registers;
        }
    }

    public class MultiplyRegister : IInstruction
    {
        public string Identifier => "mulr";
        public long[] Execute(long[] registers, long a, long b, long c)
        {
            registers[c] = registers[a] * registers[b];
            return registers;
        }
    }

    public class MultiplyImmediate : IInstruction
    {
        public string Identifier => "muli";
        public long[] Execute(long[] registers, long a, long b, long c)
        {
            registers[c] = registers[a] * b;
            return registers;
        }
    }

    public class SetRegister : IInstruction
    {
        public string Identifier => "setr";
        public long[] Execute(long[] registers, long a, long b, long c)
        {
            registers[c] = registers[a];
            return registers;
        }
    }

    public class SetImmediate : IInstruction
    {
        public string Identifier => "seti";
        public long[] Execute(long[] registers, long a, long b, long c)
        {
            registers[c] = a;
            return registers;
        }
    }

    public class GreaterThanImmediateRegister : IInstruction
    {
        public string Identifier => "gtir";
        public long[] Execute(long[] registers, long a, long b, long c)
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
        public long[] Execute(long[] registers, long a, long b, long c)
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
        public long[] Execute(long[] registers, long a, long b, long c)
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
        public long[] Execute(long[] registers, long a, long b, long c)
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
        public long[] Execute(long[] registers, long a, long b, long c)
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
        public long[] Execute(long[] registers, long a, long b, long c)
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

        public static long[] Clone(long[] input)
        {
            var result = new long[input.Length];
            for (long i = 0; i < input.Length; i++)
                result[i] = input[i];
            return result;
        }
    }
}