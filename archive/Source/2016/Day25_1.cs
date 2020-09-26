using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace Day25
{
    public class Program
    {
        public static void Main()
        {
            new Program().Run(Code);
        }

        public void Run(string input)
        {
            for (int a = 0; a < 1000; a++)
            {
                var computer = Computer.Create(input);
                computer.Registers["a"] = a;
                var pattern = computer.Execute(10).ToArray();
                //WriteLine(string.Join(" ", pattern));
                if (Validate(pattern))
                {
                    WriteLine($"Found valid pattern when a = {a}");
                    break;
                }
            }
        }

        private bool Validate(int[] pattern)
        {
            var expected = 0;

            for (int i = 0; i < pattern.Length; i++)
            {
                if (pattern[i] != expected) return false;
                expected = (expected == 0) ? 1 : 0;
            }

            return true;
        }

        public class Computer
        {
            public Computer()
            {
                Registers = new Dictionary<string, int>
                {
                    { "a", 0 },
                    { "b", 0 },
                    { "c", 0 },
                    { "d", 0 }
                };

                Instructions = new List<InstructionDefinition>();
            }

            public IEnumerable<int> Execute(int maxPatternLength)
            {
                int pointer = 0;
                var outPatternCount = 0;

                while (pointer < Instructions.Count)
                {
                    var instruction = Instructions[pointer];

                    var definition = instruction.Definition;
                    //WriteLine($"{pointer.ToString().PadLeft(3)}: {definition.PadRight(20)}" + string.Join("  ", Registers.Select(r => $"{r.Key} = {r.Value}")));

                    switch (instruction.Instruction)
                    {
                        case Instruction.Increase:
                        {
                            if (instruction.Arguments.Length != 1 || !(instruction.Arguments[0] is string register))
                            {
                                throw new InvalidOperationException($"Invalid instruction: '{instruction.Definition}'");
                            }

                            Registers[register]++;
                            break;
                        }

                        case Instruction.Decrease:
                        {
                            if (instruction.Arguments.Length != 1 || !(instruction.Arguments[0] is string register))
                            {
                                throw new InvalidOperationException($"Invalid instruction: '{instruction.Definition}'");
                            }

                            Registers[register]--;
                            break;
                        }

                        case Instruction.Copy:
                        {
                            if (instruction.Arguments.Length != 2 || !(instruction.Arguments[1] is string register))
                            {
                                throw new InvalidOperationException($"Invalid instruction: '{instruction.Definition}'");
                            }

                            var src = instruction.Arguments[0];
                            Registers[register] = (src is string srcReg) ? Registers[srcReg] : (int)src;
                            break;
                        }

                        case Instruction.Jump:
                        {
                            if (instruction.Arguments.Length != 2)
                            {
                                throw new InvalidOperationException($"Invalid instruction: '{instruction.Definition}'");
                            }

                            var offSrc = instruction.Arguments[1];
                            var offset = (offSrc is string offReg) ? Registers[offReg] : (int)offSrc;

                            var condition = instruction.Arguments[0];
                            if ((condition is string condReg && Registers[condReg] != 0) ||
                                ((condition is int) && (int)condition != 0))
                            {
                                pointer += offset - 1;
                            }
                            break;
                        }

                        case Instruction.Out:
                        {
                            if (instruction.Arguments.Length != 1)
                            {
                                throw new InvalidOperationException($"Invalid instruction: '{instruction.Definition}'");
                            }

                            var outParam = instruction.Arguments[0];
                            yield return (outParam is string outReg) ? Registers[outReg] : (int)outParam;

                            outPatternCount++;
                            if (outPatternCount >= maxPatternLength)
                            {
                                yield break;
                            }

                            break;
                        }
                    }
                    pointer++;
                }
            }

            public static Computer Create(string input)
            {
                var computer = new Computer();
                computer.ParseInput(input);
                return computer;
            }

            private void ParseInput(string input)
            {
                foreach (var instructionRow in input
                    .Replace("\r", "")
                    .Split('\n', StringSplitOptions.RemoveEmptyEntries))
                {
                    var commentIndex = instructionRow.IndexOf('#');
                    if (commentIndex < 0) commentIndex = instructionRow.Length;
                    Instructions.Add(InstructionDefinition.Parse(instructionRow[..commentIndex].Trim()));
                }
            }

            public List<InstructionDefinition> Instructions;
            public Dictionary<string, int> Registers;
        }

        public enum Instruction
        {
            Copy,
            Increase,
            Decrease,
            Jump,
            Out
        }

        public class InstructionDefinition
        {
            public static InstructionDefinition Parse(string input)
            {
                var parts = input.Trim().Split(' ', System.StringSplitOptions.RemoveEmptyEntries);

                var instruction = parts[0] switch
                {
                    "cpy" => Instruction.Copy,
                    "inc" => Instruction.Increase,
                    "dec" => Instruction.Decrease,
                    "jnz" => Instruction.Jump,
                    "out" => Instruction.Out,
                    _ => throw new InvalidOperationException($"Invalid instruction '{parts[0]}'.")
                };

                var parsedArguments = new object[parts.Length - 1];
                for (int i = 1; i < parts.Length; i++)
                {
                    parsedArguments[i-1] = int.TryParse(parts[i], out var parsedInt) ? parsedInt : parts[i];
                }

                return new InstructionDefinition(instruction, parsedArguments, input);
            }

            public InstructionDefinition(Instruction instruction, object[] arguments, string definition)
            {
                Instruction = instruction;
                Arguments = arguments;
                Definition = definition;
            }

            public Instruction Instruction { get; }
            public object[] Arguments { get; }
            public string Definition { get; }
        }

private static readonly string Code = @"
cpy a d
cpy 11 c
cpy 231 b
inc d
dec b
jnz b -2
dec c
jnz c -5
cpy d a
jnz 0 0
cpy a b
cpy 0 a
cpy 2 c
jnz b 2
jnz 1 6
dec b
dec c
jnz c -4
inc a
jnz 1 -7
cpy 2 b
jnz c 2
jnz 1 4
dec b
dec c
jnz 1 -4
jnz 0 0
out b
jnz a -19
jnz 1 -21
";
    }
}