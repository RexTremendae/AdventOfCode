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

            var computer = new Computer(instructions);
            var result = 0L;
            var goal = 19690720;
            var noun = 0L;
            var verb = 0L;

            for (noun = 0; noun < 100; noun++)
            {
                for (verb = 0; verb < 100; verb++)
                {
                    result = computer.Run(noun: noun, verb: verb);
                    if (result == goal)
                    {
                        break;
                    }
                }

                if (result == goal) break;
            }

            WriteLine($"Noun: {noun}  Verb: {verb}");
        }
    }

    public class Computer
    {
        private List<long> _instructions;

        public Computer(List<long> instructions)
        {
            _instructions = new List<long>(instructions);
        }

        public long Run(long noun, long verb)
        {
            var instructions = new List<long>(_instructions);
            instructions[1] = noun;
            instructions[2] = verb;

            var entity = InstructionEntity.NoInstruction;
            int index = 0;

            while(entity.Instruction != Instruction.Terminate)
            {
                entity = InstructionEntity.Get(instructions, index);
                entity.Execute();
                index += entity.Size;
            }

            return instructions[0];
        }
    }

    public class InstructionEntity
    {
        protected List<long> _instructions;
        protected List<long> _parameters;
        public virtual Instruction Instruction { get; }
        public virtual int Size { get; }

        public InstructionEntity()
        {
            _parameters = new List<long>();
        }

        static InstructionEntity()
        {
            NoInstruction = new NoInstruction();
        }

        public virtual void Execute()
        {
        }

        public static InstructionEntity Get(List<long> instructions, int index)
        {
            var instruction = (Instruction)instructions[index];

            var entity = (InstructionEntity)(instruction switch {
                Instruction.Add => new AddInstruction(instructions, index+1),
                Instruction.Multiply => new MultiplyInstruction(instructions, index+1),
                Instruction.Terminate => new TerminateInstruction(),
                _ => throw new InvalidOperationException("Invalid instruction")
            });

            return entity;
        }

        public static InstructionEntity NoInstruction { get; private set; }
    }

    public class NoInstruction : InstructionEntity
    {
        public override Instruction Instruction => Instruction.None;
        public override int Size => 0;
    }

    public class AddInstruction : InstructionEntity
    {
        public AddInstruction(List<long> instructions, int index)
        {
            _instructions = instructions;
            _parameters.AddRange(instructions.Skip(index).Take(3));
        }

        public override void Execute()
        {
            _instructions[(int)_parameters[2]] = _instructions[(int)_parameters[0]] + _instructions[(int)_parameters[1]];
        }

        public override Instruction Instruction => Instruction.Add;
        public override int Size => 4;
    }

    public class MultiplyInstruction : InstructionEntity
    {
        public MultiplyInstruction(List<long> instructions, int index)
        {
            _instructions = instructions;
            _parameters.AddRange(instructions.Skip(index).Take(3));
        }

        public override void Execute()
        {
            _instructions[(int)_parameters[2]] = _instructions[(int)_parameters[0]] * _instructions[(int)_parameters[1]];
        }

        public override Instruction Instruction => Instruction.Multiply;
        public override int Size => 4;
    }

    public class TerminateInstruction : InstructionEntity
    {
        public override Instruction Instruction => Instruction.Terminate;
        public override int Size => 1;
    }

    public enum Instruction
    {
        None = 0,
        Add = 1,
        Multiply = 2,
        Terminate = 99
    }
}
