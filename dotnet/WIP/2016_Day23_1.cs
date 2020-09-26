using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace Day23
{
    public class Program
    {
        public static void Main()
        {
            new Program().Run(Sample_WithToggle);
        }

        public void Run(string input)
        {
            var computer = Computer.Create(input);
            computer.Execute();
            WriteLine(computer.Registers["a"]);
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

            public void Execute()
            {
                int pointer = 0;
                int executionLimit = 100;

                var executedInstructionsCount = 0;
                while (pointer < Instructions.Count)
                {
                    if (executedInstructionsCount >= executionLimit)
                    {
                        WriteLine("Execution limit exceeded, aborting!");
                        break;
                    }
                    executedInstructionsCount++;

                    var instruction = Instructions[pointer];

                    var definition = instruction.Definition + (instruction.IsToggled ? " <t>" : "");
                    WriteLine($"{pointer.ToString().PadLeft(3)}: {definition.PadRight(20)}" + string.Join("  ", Registers.Select(r => $"{r.Key} = {r.Value}")));

                    switch (instruction.Instruction)
                    {
                        case Instruction.Increase:
                        {
                            if (instruction.Arguments.Length != 1 || !(instruction.Arguments[0] is string register))
                            {
                                if (instruction.IsToggled) break;
                                throw new InvalidOperationException($"Invalid instruction: '{instruction.Definition}'");
                            }

                            Registers[register]++;
                            break;
                        }

                        case Instruction.Decrease:
                        {
                            if (instruction.Arguments.Length != 1 || !(instruction.Arguments[0] is string register))
                            {
                                if (instruction.IsToggled) break;
                                throw new InvalidOperationException($"Invalid instruction: '{instruction.Definition}'");
                            }

                            Registers[register]--;
                            break;
                        }

                        case Instruction.Copy:
                        {
                            if (instruction.Arguments.Length != 2 || !(instruction.Arguments[1] is string register))
                            {
                                if (instruction.IsToggled) break;
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
                                if (instruction.IsToggled) break;
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

                        case Instruction.Toggle:
                        {
                            if (instruction.Arguments.Length != 1)
                            {
                                if (instruction.IsToggled) break;
                                throw new InvalidOperationException($"Invalid instruction: '{instruction.Definition}'");
                            }

                            var offsetSrc = instruction.Arguments[0];
                            var offset = (offsetSrc is string offsetSrcStr) ? Registers[offsetSrcStr] : (int)offsetSrc;
                            var togglePointer = pointer + offset;
                            if (togglePointer < 0 || togglePointer >= Instructions.Count)
                            {
                                break;
                            }

                            Toggle(togglePointer);

                            break;
                        }

                        case Instruction.NoOperation:
                            break;
                    }
                    pointer++;
                }
            }

            private void Toggle(int index)
            {
                var instruction = Instructions[index];
                Instruction toggled;
                if (instruction.Arguments.Length == 1)
                {
                    toggled = (instruction.Instruction == Instruction.Increase) ? Instruction.Decrease : Instruction.Increase;
                }
                else if (instruction.Arguments.Length == 2)
                {
                    toggled = (instruction.Instruction == Instruction.Jump) ? Instruction.Copy : Instruction.Jump;
                }
                else
                {
                    throw new InvalidOperationException();
                }

                Instructions[index] = new InstructionDefinition(toggled, instruction.Arguments, instruction.Definition, true);
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
            NoOperation,
            Toggle,
            Copy,
            Increase,
            Decrease,
            Jump
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
                    "tgl" => Instruction.Toggle,
                    "jnz" => Instruction.Jump,
                    "nop" => Instruction.NoOperation,
                    _ => throw new InvalidOperationException($"Invalid instruction '{parts[0]}'.")
                };

                var parsedArguments = new object[parts.Length - 1];
                for (int i = 1; i < parts.Length; i++)
                {
                    parsedArguments[i-1] = int.TryParse(parts[i], out var parsedInt) ? parsedInt : parts[i];
                }

                return new InstructionDefinition(instruction, parsedArguments, input);
            }

            public InstructionDefinition(Instruction instruction, object[] arguments, string definition, bool isToggled = false)
            {
                Instruction = instruction;
                Arguments = arguments;
                Definition = definition;
                IsToggled = isToggled;
            }

            public Instruction Instruction { get; }
            public object[] Arguments { get; }
            public string Definition { get; }
            public bool IsToggled { get; }
        }

// Result: a = 42
private static readonly string Sample_ToggleFree_1 = @"
cpy 41 a
inc a
inc a
dec a
jnz a 2
dec a
";

// Result: a = 318117
private static readonly string Sample_ToggleFree_2 = @"
cpy 1 a
cpy 1 b
cpy 26 d
jnz c 2
jnz 1 5
cpy 7 c
inc d
dec c
jnz c -2
cpy a c
inc a
dec b
jnz b -2
cpy c b
dec d
jnz d -6
cpy 17 c
cpy 18 d
inc a
dec d
jnz d -2
dec c
jnz c -5
";

// Result: a = 3
private static readonly string Sample_WithToggle = @"
cpy 2 a
tgl a
tgl a
tgl a
cpy 1 a
dec a
dec a
";

private static readonly string Challenge_Original =
@"
cpy a b
dec b
cpy a d
cpy 0 a
cpy b c
inc a
dec c
jnz c -2
dec d
jnz d -5
dec b
cpy b c
cpy c d
dec d
inc c
jnz d -2
tgl c
cpy -16 c
jnz 1 c
cpy 94 c
jnz 99 d
inc a
inc d
jnz d -2
inc c
jnz c -5
";
    }
}