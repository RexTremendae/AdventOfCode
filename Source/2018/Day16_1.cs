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
            if (args.Length < 1)
            {
                return;
            }

            var instructions = GetInstructions();

            var totalMatches = 0;
            foreach (var data in ParseInput(args[0]))
            {
                WriteLine($"Before:  {Utilities.ToString(data.before)}");
                WriteLine($"Execute: {Utilities.ToString(data.execution)}");
                WriteLine($"After:   {Utilities.ToString(data.after)}");
                WriteLine();

                var matches = MatchInstructions(data.before, data.execution, data.after, instructions).Select(x => x.Identifier).ToList();
                if (matches.Count >= 3) totalMatches ++;
                WriteLine($"Matches: {matches.Count}");
                WriteLine();
            }

            WriteLine(totalMatches);
        }

        public static IEnumerable<IInstruction> MatchInstructions(int[] before, int[] execution, int[] after, List<IInstruction> instructions)
        {
            foreach (var instr in instructions)
            {
                int opCode = execution[0];
                int a = execution[1];
                int b = execution[2];
                int c = execution[3];

                var result = instr.Execute(Utilities.Clone(before), opCode, a, b, c);
                var expected = after;
                bool match = true;
                for (int i = 0; i < 4; i ++)
                {
                    if (result[i] != expected[i])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    WriteLine(instr.Identifier);
                    yield return instr;
                }
            }
        }

        public static IEnumerable<(int[] before, int[] execution, int[] after)> ParseInput(string filepath)
        {
            var before = string.Empty;
            var after = string.Empty;
            var execution = string.Empty;

            foreach (var lineRaw in File.ReadAllLines(filepath))
            {
                var line = lineRaw.Trim();

                if (string.IsNullOrEmpty(line)) continue;

                if (line.StartsWith("Before:"))
                {
                    before = line.Substring(8).Replace(" ", "");
                }
                else if (line.StartsWith("After:"))
                {
                    after = line.Substring(8).Replace(" ", "");
                    yield return (Parse(before), Parse(execution), Parse(after));
                    before = string.Empty;
                }
                else if (!string.IsNullOrEmpty(before))
                {
                    execution = "[" + line.Replace(' ', ',') + "]";
                }
            }
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

        public static List<IInstruction> GetInstructions()
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

            return instructions;
        }
    }

    public interface IInstruction
    {
        string Identifier { get; }
        int[] Execute(int[] registers, int opcode, int a, int b, int c);
    }

    public class AddRegister : IInstruction
    {
        public string Identifier => "addr";
        public int[] Execute(int[] registers, int opcode, int a, int b, int c)
        {
            registers[c] = registers[a] + registers[b];
            return registers;
        }
    }

    public class AddImmediate : IInstruction
    {
        public string Identifier => "addi";
        public int[] Execute(int[] registers, int opcode, int a, int b, int c)
        {
            registers[c] = registers[a] + b;
            return registers;
        }
    }

    public class MultiplyRegister : IInstruction
    {
        public string Identifier => "mulr";
        public int[] Execute(int[] registers, int opcode, int a, int b, int c)
        {
            registers[c] = registers[a] * registers[b];
            return registers;
        }
    }

    public class MultiplyImmediate : IInstruction
    {
        public string Identifier => "muli";
        public int[] Execute(int[] registers, int opcode, int a, int b, int c)
        {
            registers[c] = registers[a] * b;
            return registers;
        }
    }

    public class BitwiseAndRegister : IInstruction
    {
        public string Identifier => "banr";
        public int[] Execute(int[] registers, int opcode, int a, int b, int c)
        {
            registers[c] = registers[a] & registers[b];
            return registers;
        }
    }

    public class BitwiseAndImmediate : IInstruction
    {
        public string Identifier => "bani";
        public int[] Execute(int[] registers, int opcode, int a, int b, int c)
        {
            registers[c] = registers[a] & b;
            return registers;
        }
    }

    public class BitwiseOrRegister : IInstruction
    {
        public string Identifier => "borr";
        public int[] Execute(int[] registers, int opcode, int a, int b, int c)
        {
            registers[c] = registers[a] | registers[b];
            return registers;
        }
    }

    public class BitwiseOrImmediate : IInstruction
    {
        public string Identifier => "bori";
        public int[] Execute(int[] registers, int opcode, int a, int b, int c)
        {
            registers[c] = registers[a] | b;
            return registers;
        }
    }

    public class SetRegister : IInstruction
    {
        public string Identifier => "setr";
        public int[] Execute(int[] registers, int opcode, int a, int b, int c)
        {
            registers[c] = registers[a];
            return registers;
        }
    }

    public class SetImmediate : IInstruction
    {
        public string Identifier => "seti";
        public int[] Execute(int[] registers, int opcode, int a, int b, int c)
        {
            registers[c] = a;
            return registers;
        }
    }

    public class GreaterThanImmediateRegister : IInstruction
    {
        public string Identifier => "gtir";
        public int[] Execute(int[] registers, int opcode, int a, int b, int c)
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
        public int[] Execute(int[] registers, int opcode, int a, int b, int c)
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
        public int[] Execute(int[] registers, int opcode, int a, int b, int c)
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
        public int[] Execute(int[] registers, int opcode, int a, int b, int c)
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
        public int[] Execute(int[] registers, int opcode, int a, int b, int c)
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
        public int[] Execute(int[] registers, int opcode, int a, int b, int c)
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