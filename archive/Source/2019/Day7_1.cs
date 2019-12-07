using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using static System.Console;

namespace Day5
{
    public class Program
    {
        public static void Main()
        {
            new Program().Run();
        }

        public void Run()
        {
            var instructions = File.ReadAllText("Day5.txt")
                                   .Trim()
                                   .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                   .Select(x => long.Parse(x))
                                   .ToList();

            var computers = new Computer[5];

            long maxOutput = 0L;
            foreach (var phaseSettings in GetPhaseSettingsPermutations())
            {
                var input = 0L;
                for (int i = 0; i < phaseSettings.Length; i++)
                {
                    var computer = new Computer(instructions: instructions);
                    var output = computer.Run(inputs: new[] { phaseSettings[i], input }, debug: false);
                    input = output;
                }

                maxOutput = Math.Max(maxOutput, input);
            }

            WriteLine(maxOutput);
        }

        private HashSet<int[]> _permuations = new HashSet<int[]>();
        private int[] _lastPhaseSetting = new[] { 0, 1, 2, 3, 4 };

        private IEnumerable<int[]> GetPhaseSettingsPermutations()
        {
            do
            {
                var setting = GetNextSetting();
                if (!ValidSetting(setting)) continue;

                yield return _lastPhaseSetting;
            }
            while (!ArrayEquals(_lastPhaseSetting, new[] {0, 1, 2, 3, 4}));
        }

        private bool ArrayEquals(int[] array1, int[] array2)
        {
            if (array1.Length != array2.Length) return false;

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i]) return false;
            }

            return true;
        }

        private bool ValidSetting(int[] setting)
        {
            int i = 0, j = 0;
            for (i = 0; i < setting.Length-1; i++)
            {
                for (j = i+1; j < setting.Length; j++)
                {
                    if (setting[i] == setting[j]) return false;
                }
            }

            return true;
        }

        private int[] GetNextSetting()
        {
            int i = _lastPhaseSetting.Length - 1;
            _lastPhaseSetting[i]++;
            while (_lastPhaseSetting[i] > 4)
            {
                _lastPhaseSetting[i] = 0;
                if (i == 0) break;

                i--;
                _lastPhaseSetting[i]++;
            }

            return _lastPhaseSetting;
        }
    }

    public class Computer
    {
        private List<long> _instructions;
        private int _jumpTo;
        private List<long> _inputs;
        private long _output;

        public Computer(List<long> instructions)
        {
            _inputs = new List<long>();
            _instructions = new List<long>(instructions);
            _jumpTo = -1;
        }

        public long Run(IEnumerable<long> inputs, long? noun = null, long? verb = null, bool debug = false)
        {
            _inputs = inputs.ToList();

            var instructions = new List<long>(_instructions);
            if (noun != null) instructions[1] = noun.Value;
            if (verb != null) instructions[2] = verb.Value;

            var entity = InstructionEntity.NoInstruction;
            int index = 0;

            while(entity.Instruction != Instruction.Terminate)
            {
                entity = GetInstruction(instructions, index);
                if (debug)
                {
                    WriteLine($"Executing {entity.Instruction} [{index}]");
                }

                entity.Execute();
                
                if (_jumpTo >= 0)
                {
                    index = _jumpTo;
                    _jumpTo = -1;
                }
                else
                {
                    index += entity.Step;
                }
            }

            return _output;
        }

        public void Goto(int newIndex)
        {
            _jumpTo = newIndex;
        }

        public long GetInput()
        {
            var input = _inputs.First();
            WriteLine($"Input signal: {input}");
            _inputs.RemoveAt(0);
            return input;
        }

        public void Output(long output)
        {
            _output = output;
            WriteLine($"Output instruction: {output}");
        }

        public InstructionEntity GetInstruction(List<long> instructions, int index)
        {
            var instruction = instructions[index];
            var parameterModes = new List<ParameterMode>(new [] {ParameterMode.Position,ParameterMode.Position,ParameterMode.Position});

            if (instruction >= 10_000)
            {
                parameterModes[2] = ParameterMode.Immediate;
                instruction -= 10_000;
            }
            if (instruction >= 1_000)
            {
                parameterModes[1] = ParameterMode.Immediate;
                instruction -= 1_000;
            }
            if (instruction >= 100)
            {
                parameterModes[0] = ParameterMode.Immediate;
                instruction -= 100;
            }

            var entity = (InstructionEntity)((Instruction)instruction switch {
                Instruction.Add => new AddInstruction(this, instructions, index+1, parameterModes),
                Instruction.Multiply => new MultiplyInstruction(this, instructions, index+1, parameterModes),
                Instruction.Input => new InputInstruction(this, instructions, index+1, parameterModes),
                Instruction.Output => new OutputInstruction(this, instructions, index+1, parameterModes),
                Instruction.JumpIfTrue => new JumpIfTrueInstruction(this, instructions, index+1, parameterModes),
                Instruction.JumpIfFalse => new JumpIfFalseInstruction(this, instructions, index+1, parameterModes),
                Instruction.LessThan => new LessThanInstruction(this, instructions, index+1, parameterModes),
                Instruction.Equals => new EqualsInstruction(this, instructions, index+1, parameterModes),
                Instruction.Terminate => new TerminateInstruction(this),
                _ => throw new InvalidOperationException($"Invalid instruction: {instructions[index]} at position {index}")
            });

            return entity;
        }
    }

    public class InstructionEntity
    {
        protected List<long> _instructions;
        protected List<long> _parameters;
        protected List<ParameterMode> _parameterModes;
        protected Computer _computer;

        public virtual Instruction Instruction { get; }
        public virtual int Step { get; }

        public InstructionEntity(Computer computer, List<long> instructions, List<long> parameters, List<ParameterMode> parameterModes)
        {
            _computer = computer;
            _parameters = parameters;
            _parameterModes = parameterModes;
            _instructions = instructions;
        }

        public InstructionEntity(Computer computer)
        {
            _computer = computer;
            _parameters = new List<long>();
            _instructions = new List<long>();
        }

        static InstructionEntity()
        {
            NoInstruction = new NoInstruction();
        }

        protected long ReadValue(int parameter)
        {
            if (_parameterModes[parameter] == ParameterMode.Position)
            {
                return _instructions[(int)_parameters[parameter]];
            }

            return _parameters[parameter];
        }

        public virtual void Execute()
        {
        }

        public static InstructionEntity NoInstruction { get; private set; }
    }

    public class NoInstruction : InstructionEntity
    {
        public NoInstruction() : base(null) {}

        public override Instruction Instruction => Instruction.None;
        public override int Step => 0;
    }

    public class AddInstruction : InstructionEntity
    {
        public AddInstruction(Computer computer, List<long> instructions, int index, List<ParameterMode> parameterModes)
            : base(computer, instructions, instructions.Slice(index, 3), parameterModes) {}

        public override void Execute()
        {
            _instructions[(int)_parameters[2]] = ReadValue(0) + ReadValue(1);
        }

        public override Instruction Instruction => Instruction.Add;
        public override int Step => 4;
    }

    public class MultiplyInstruction : InstructionEntity
    {
        public MultiplyInstruction(Computer computer, List<long> instructions, int index, List<ParameterMode> parameterModes)
            : base(computer, instructions, instructions.Slice(index, 3), parameterModes) {}

        public override void Execute()
        {
            _instructions[(int)_parameters[2]] = ReadValue(0) * ReadValue(1);
        }

        public override Instruction Instruction => Instruction.Multiply;
        public override int Step => 4;
    }

    public class InputInstruction : InstructionEntity
    {
        public InputInstruction(Computer computer, List<long> instructions, int index, List<ParameterMode> parameterModes)
            : base(computer, instructions, instructions.Slice(index, 1), parameterModes) {}

        public override void Execute()
        {
            _instructions[(int)_parameters[0]] = _computer.GetInput();
        }

        public override Instruction Instruction => Instruction.Input;
        public override int Step => 2;
    }

    public class OutputInstruction : InstructionEntity
    {
        public OutputInstruction(Computer computer, List<long> instructions, int index, List<ParameterMode> parameterModes)
            : base(computer, instructions, instructions.Slice(index, 1), parameterModes) {}

        public override void Execute()
        {
            _computer.Output(ReadValue(0));
        }

        public override Instruction Instruction => Instruction.Output;
        public override int Step => 2;
    }

    public class JumpIfTrueInstruction : InstructionEntity
    {
        public JumpIfTrueInstruction(Computer computer, List<long> instructions, int index, List<ParameterMode> parameterModes)
            : base(computer, instructions, instructions.Slice(index, 2), parameterModes) {}

        public override void Execute()
        {
            if (ReadValue(0) != 0)
            {
                _computer.Goto((int)ReadValue(1));
            }
        }

        public override Instruction Instruction => Instruction.JumpIfTrue;
        public override int Step => 3;
    }

    public class JumpIfFalseInstruction : InstructionEntity
    {
        public JumpIfFalseInstruction(Computer computer, List<long> instructions, int index, List<ParameterMode> parameterModes)
            : base(computer, instructions, instructions.Slice(index, 2), parameterModes) {}

        public override void Execute()
        {
            if (ReadValue(0) == 0)
            {
                _computer.Goto((int)ReadValue(1));
            }
        }

        public override Instruction Instruction => Instruction.JumpIfFalse;
        public override int Step => 3;
    }

    public class LessThanInstruction : InstructionEntity
    {
        public LessThanInstruction(Computer computer, List<long> instructions, int index, List<ParameterMode> parameterModes)
            : base(computer, instructions, instructions.Slice(index, 3), parameterModes) {}

        public override void Execute()
        {
            _instructions[(int)_parameters[2]] =
                ReadValue(0) < ReadValue(1) ? 1 : 0;
        }

        public override Instruction Instruction => Instruction.LessThan;
        public override int Step => 4;
    }

    public class EqualsInstruction : InstructionEntity
    {
        public EqualsInstruction(Computer computer, List<long> instructions, int index, List<ParameterMode> parameterModes)
            : base(computer, instructions, instructions.Slice(index, 3), parameterModes) {}

        public override void Execute()
        {
            _instructions[(int)_parameters[2]] =
                ReadValue(0) == ReadValue(1) ? 1 : 0;
        }

        public override Instruction Instruction => Instruction.Equals;
        public override int Step => 4;
    }

    public class TerminateInstruction : InstructionEntity
    {
        public TerminateInstruction(Computer computer) : base(computer) {}

        public override Instruction Instruction => Instruction.Terminate;
        public override int Step => 1;
    }

    public static class ListExtensions
    {
        public static List<T> Slice<T>(this List<T> input, int startIndex, int length)
        {
            return input.Skip(startIndex).Take(length).ToList();
        }
    }

    public enum Instruction
    {
        None = 0,
        Add = 1,
        Multiply = 2,
        Input = 3,
        Output = 4,
        JumpIfTrue = 5,
        JumpIfFalse = 6,
        LessThan = 7,
        Equals = 8,
        Terminate = 99
    }

    public enum ParameterMode
    {
        Position = 0,
        Immediate = 1
    }
}
