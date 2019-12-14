using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static System.Console;

namespace Day13
{
    public class Program
    {
        public static void Main()
        {
            new Program().Run();
        }

        public void Run()
        {
            var instructions = File.ReadAllText("Day13.txt")
                                   .Trim()
                                   .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                   .Select(x => long.Parse(x))
                                   .ToList();

            var arcade = new Arcade();
            var computer = new Computer(new Memory(instructions), arcade: arcade);
            computer.Run(debug: false);
            Console.OutputEncoding = Encoding.UTF8;
            arcade.Print();
        }
    }

    public class Arcade
    {
        private Dictionary<(long x, long y), ArcadeEntity> _gameBoard;
        private long _minX, _minY;
        private long _maxX, _maxY;
        private long[] _currentInput;

        public Arcade()
        {
            _gameBoard = new Dictionary<(long x, long y), ArcadeEntity>();
            _currentInput = new long[3];
            for (int i = 0; i < 3; i ++) _currentInput[i] = long.MinValue;
        }

        public void Print()
        {
            long blockCount = 0;
            for (long y = _minY; y <= _maxY; y ++)
            {
                for (long x = _minX; x <= _maxX; x ++)
                {
                    if (!_gameBoard.TryGetValue((x, y), out var entity)) entity = ArcadeEntity.Empty;
                    Write(entity switch {
                        ArcadeEntity.Empty => ' ',
                        ArcadeEntity.Ball => 'o',
                        ArcadeEntity.Wall => '#',
                        ArcadeEntity.HorizontalPaddle => '_',
                        ArcadeEntity.Block => '□',
                        _ => throw new Exception()
                    });

                    if (entity == ArcadeEntity.Block) blockCount++;
                }

                WriteLine();
            }

            WriteLine($"#blocks: {blockCount}");
        }

        internal long? Output()
        {
            throw new NotImplementedException();
        }

        internal void ReceiveInput(long output)
        {
            int i = 0;
            for (i = 0; i < 3; i ++)
            {
                if (_currentInput[i] == long.MinValue)
                {
                    _currentInput[i] = output;
                    break;
                }
            }

            if (i == 2)
            {
                var x = _currentInput[0];
                var y = _currentInput[1];
                var e = (ArcadeEntity) _currentInput[2];

                _gameBoard.Add((x, y), e);

                _minX = Math.Min(_minX, x);
                _maxX = Math.Max(_maxX, x);
                _minY = Math.Min(_minY, y);
                _maxY = Math.Max(_maxY, y);

                for (i = 0; i < 3; i++) _currentInput[i] = long.MinValue;
            }
        }
    }

    public class Computer
    {
        private Memory _memory;
        private long _jumpTo;
        private Computer _next;
        private InstructionEntity _entity;
        private long _index;
        private Arcade _arcade;

        public long LastOutput { get; private set; }
        public long RelativeBase { get; private set; }
        public bool Debug { get; private set; }

        public Computer(Memory memory, Arcade arcade)
        {
            _arcade = arcade;
            _memory = memory;
            _jumpTo = -1;
            _entity = InstructionEntity.NoInstruction;
            _index = 0;
        }

        public void Connect(Computer next)
        {
            _next = next;
        }

        public State Run(bool debug = false)
        {
            Debug = debug;
            if (debug) _memory.SetDebugMode();
            while(_entity.Instruction != Instruction.Terminate)
            {
                _entity = GetInstruction(_index);
                if (debug)
                {
                    WriteLine($"Executing {_entity.Instruction} [{_index}]");
                }

                if (!_entity.Execute()) return State.WaitingForInput;

                if (_jumpTo >= 0)
                {
                    _index = _jumpTo;
                    _jumpTo = -1;
                }
                else
                {
                    _index += _entity.Step;
                }
            }

            return State.Finished;
        }

        public void Goto(long newIndex)
        {
            _jumpTo = newIndex;
        }

        public void AdjustRelativeBase(long adjustment)
        {
            RelativeBase += adjustment;
        }

        public long? RequestInput()
        {
            return _arcade.Output();
        }

        public void Output(long output)
        {
            LastOutput = output;
            _arcade.ReceiveInput(output);
        }

        public InstructionEntity GetInstruction(long index)
        {
            var instruction = _memory.Read(index);
            var parameterModes = new List<ParameterMode>(new [] {ParameterMode.Position,ParameterMode.Position,ParameterMode.Position});

            if (instruction >= 10_000)
            {
                var pos = instruction / 10_000;
                parameterModes[2] = (ParameterMode)pos;
                instruction -= 10_000*pos;
            }
            if (instruction >= 1_000)
            {
                var pos = instruction / 1_000;
                parameterModes[1] = (ParameterMode)pos;
                instruction -= 1_000*pos;
            }
            if (instruction >= 100)
            {
                var pos = instruction / 100;
                parameterModes[0] = (ParameterMode)pos;
                instruction -= 100*pos;
            }

            var entity = (InstructionEntity)((Instruction)instruction switch {
                Instruction.Add => new InstructionEntities.AddInstruction(this, _memory, index+1, parameterModes),
                Instruction.Multiply => new InstructionEntities.MultiplyInstruction(this, _memory, index+1, parameterModes),
                Instruction.Input => new InstructionEntities.InputInstruction(this, _memory, index+1, parameterModes),
                Instruction.Output => new InstructionEntities.OutputInstruction(this, _memory, index+1, parameterModes),
                Instruction.JumpIfTrue => new InstructionEntities.JumpIfTrueInstruction(this, _memory, index+1, parameterModes),
                Instruction.JumpIfFalse => new InstructionEntities.JumpIfFalseInstruction(this, _memory, index+1, parameterModes),
                Instruction.LessThan => new InstructionEntities.LessThanInstruction(this, _memory, index+1, parameterModes),
                Instruction.Equals => new InstructionEntities.EqualsInstruction(this, _memory, index+1, parameterModes),
                Instruction.AdjustRelativeBase => new InstructionEntities.AdjustRelativeBaseInstruction(this, _memory, index+1, parameterModes),
                Instruction.Terminate => new InstructionEntities.TerminateInstruction(this),
                _ => throw new InvalidOperationException($"Invalid instruction: {_memory.Read(index)} at position {index}")
            });

            return entity;
        }
    }

    public class InstructionEntity
    {
        protected Memory _memory;
        protected List<long> _parameters;
        protected List<ParameterMode> _parameterModes;
        protected Computer _computer;

        public virtual Instruction Instruction { get; }
        public virtual int Step { get; }

        public InstructionEntity(Computer computer, Memory memory, List<long> parameters, List<ParameterMode> parameterModes)
        {
            _computer = computer;
            _parameters = parameters;
            _parameterModes = parameterModes;
            _memory = memory;
        }

        public InstructionEntity(Computer computer)
        {
            _computer = computer;
            _parameters = new List<long>();
            _memory = new Memory();
        }

        static InstructionEntity()
        {
            NoInstruction = new InstructionEntities.NoInstruction();
        }

        protected long GetWriteValue(int parameterIdx)
        {
            var mode = _parameterModes[parameterIdx];
            var value = _parameters[parameterIdx];

            switch (mode)
            {
                case ParameterMode.Position:
                    return value;
                case ParameterMode.Relative:
                    return value + _computer.RelativeBase;
                default:
                    throw new InvalidOperationException("Invalid parameter mode");
            }
        }

        protected long GetReadValue(int parameterIdx)
        {
            var mode = _parameterModes[parameterIdx];
            var value = _parameters[parameterIdx];

            switch (mode)
            {
                case ParameterMode.Immediate:
                    return value;
                case ParameterMode.Position:
                    return _memory.Read(value);
                case ParameterMode.Relative:
                    return _memory.Read(value + _computer.RelativeBase);
                default:
                    throw new InvalidOperationException("Invalid parameter mode");
            }
        }

        public virtual bool Execute()
        {
            return true;
        }

        public static InstructionEntity NoInstruction { get; private set; }

    }

    public class InstructionEntities
    {
        public class NoInstruction : InstructionEntity
        {
            public NoInstruction() : base(null) {}

            public override Instruction Instruction => Instruction.None;
            public override int Step => 0;
        }

        public class AddInstruction : InstructionEntity
        {
            public AddInstruction(Computer computer, Memory memory, long index, List<ParameterMode> parameterModes)
                : base(computer, memory, memory.Slice(index, 3), parameterModes) {}

            public override bool Execute()
            {
                _memory.Write(GetWriteValue(2), GetReadValue(0) + GetReadValue(1));
                return true;
            }

            public override Instruction Instruction => Instruction.Add;
            public override int Step => 4;
        }

        public class MultiplyInstruction : InstructionEntity
        {
            public MultiplyInstruction(Computer computer, Memory memory, long index, List<ParameterMode> parameterModes)
                : base(computer, memory, memory.Slice(index, 3), parameterModes) {}

            public override bool Execute()
            {
                _memory.Write(GetWriteValue(2), GetReadValue(0) * GetReadValue(1));
                return true;
            }

            public override Instruction Instruction => Instruction.Multiply;
            public override int Step => 4;
        }

        public class InputInstruction : InstructionEntity
        {
            public InputInstruction(Computer computer, Memory memory, long index, List<ParameterMode> parameterModes)
                : base(computer, memory, memory.Slice(index, 1), parameterModes) {}

            public override bool Execute()
            {
                var input = _computer.RequestInput();
                if (input == null) return false;

                _memory.Write(GetWriteValue(0), input.Value);
                return true;
            }

            public override Instruction Instruction => Instruction.Input;
            public override int Step => 2;
        }

        public class OutputInstruction : InstructionEntity
        {
            public OutputInstruction(Computer computer, Memory memory, long index, List<ParameterMode> parameterModes)
                : base(computer, memory, memory.Slice(index, 1), parameterModes) {}

            public override bool Execute()
            {
                _computer.Output(GetReadValue(0));
                return true;
            }

            public override Instruction Instruction => Instruction.Output;
            public override int Step => 2;
        }

        public class JumpIfTrueInstruction : InstructionEntity
        {
            public JumpIfTrueInstruction(Computer computer, Memory memory, long index, List<ParameterMode> parameterModes)
                : base(computer, memory, memory.Slice(index, 2), parameterModes) {}

            public override bool Execute()
            {
                if (GetReadValue(0) != 0)
                {
                    var pos = GetReadValue(1);
                    if (_computer.Debug) WriteLine($"Jump to [{pos}]");
                    _computer.Goto(pos);
                }
                else
                {
                    if (_computer.Debug) WriteLine("Don't jump");
                }
                return true;
            }

            public override Instruction Instruction => Instruction.JumpIfTrue;
            public override int Step => 3;
        }

        public class JumpIfFalseInstruction : InstructionEntity
        {
            public JumpIfFalseInstruction(Computer computer, Memory memory, long index, List<ParameterMode> parameterModes)
                : base(computer, memory, memory.Slice(index, 2), parameterModes) {}

            public override bool Execute()
            {
                if (GetReadValue(0) == 0)
                {
                    var pos = GetReadValue(1);
                    if (_computer.Debug) WriteLine($"Jump to [{pos}]");
                    _computer.Goto(pos);
                }
                else
                {
                    if (_computer.Debug) WriteLine("Don't jump");
                }
                return true;
            }

            public override Instruction Instruction => Instruction.JumpIfFalse;
            public override int Step => 3;
        }

        public class LessThanInstruction : InstructionEntity
        {
            public LessThanInstruction(Computer computer, Memory memory, long index, List<ParameterMode> parameterModes)
                : base(computer, memory, memory.Slice(index, 3), parameterModes) {}

            public override bool Execute()
            {
                _memory.Write(GetWriteValue(2), GetReadValue(0) < GetReadValue(1) ? 1 : 0);
                return true;
            }

            public override Instruction Instruction => Instruction.LessThan;
            public override int Step => 4;
        }

        public class EqualsInstruction : InstructionEntity
        {
            public EqualsInstruction(Computer computer, Memory memory, long index, List<ParameterMode> parameterModes)
                : base(computer, memory, memory.Slice(index, 3), parameterModes) {}

            public override bool Execute()
            {
                _memory.Write(GetWriteValue(2), GetReadValue(0) == GetReadValue(1) ? 1 : 0);
                return true;
            }

            public override Instruction Instruction => Instruction.Equals;
            public override int Step => 4;
        }

        public class AdjustRelativeBaseInstruction : InstructionEntity
        {
            public AdjustRelativeBaseInstruction(Computer computer, Memory memory, long index, List<ParameterMode> parameterModes)
                : base(computer, memory, memory.Slice(index, 1), parameterModes) {}

            public override bool Execute()
            {
                var val = GetReadValue(0);
                _computer.AdjustRelativeBase(val);
                if (_computer.Debug) WriteLine($"Adjusting relative base with {val}. New value: {_computer.RelativeBase}");
                return true;
            }

            public override Instruction Instruction => Instruction.AdjustRelativeBase;
            public override int Step => 2;
        }

        public class TerminateInstruction : InstructionEntity
        {
            public TerminateInstruction(Computer computer) : base(computer) {}

            public override Instruction Instruction => Instruction.Terminate;
            public override int Step => 1;
        }
    }

    public class Memory
    {
        private Dictionary<long, long> _memory;
        private bool _debug;

        public void SetDebugMode()
        {
            _debug = true;
        }

        public Memory(List<long> instructions = null)
        {
            if (instructions == null) instructions = new List<long>();
            _memory = new Dictionary<long, long>();
            for (int i = 0; i < instructions.Count; i++)
            {
                _memory.Add(i, instructions[i]);
            }
        }

        public List<long> Slice(long start, long length)
        {
            var result = new List<long>();
            for (long i = start; i < start+length; i++)
            {
                result.Add(Read(i));
            }
            return result;
        }

        public void Write(long position, long data)
        {
            if (_debug) WriteLine($"Write {data} to [{position}]");
            _memory[position] = data;
        }

        public long Read(long position)
        {
            if (_memory.TryGetValue(position, out var data)) return data;

            return 0;
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
        AdjustRelativeBase = 9,
        Terminate = 99
    }

    public enum ParameterMode
    {
        Position = 0,
        Immediate = 1,
        Relative = 2
    }

    public enum State
    {
        Running,
        WaitingForInput,
        Finished
    }

    public enum ArcadeEntity
    {
        Empty = 0,
        Wall = 1,
        Block = 2,
        HorizontalPaddle = 3,
        Ball = 4
    }
}