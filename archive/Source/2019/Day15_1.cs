using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static System.Console;

namespace Day15
{
    public class Program
    {
        public static void Main()
        {
            new Program().Run();
        }

        public void Run()
        {
            Console.OutputEncoding = Encoding.UTF8;
            var instructions = File.ReadAllText("Day15.txt")
                                   .Trim()
                                   .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                   .Select(x => long.Parse(x))
                                   .ToList();

            var repairDroid = new RepairDroid(interactive: false);
            var computer = new Computer(new Memory(instructions), repairDroid);
            computer.Run(debug: false);
            repairDroid.PrintMap();
        }
    }

    public class RepairDroid : IConnector
    {
        private Dictionary<(long x, long y), (Entity entity, long distance)> _map;
        private long _minX, _minY;
        private long _maxX, _maxY;
        private (long x, long y) _position;
        private (long x, long y)? _oxygenPosition;
        private Direction _lastMovement;
        private bool _interactive;
        private long _totalMoves;

        public enum Entity
        {
            Unknown = -1,
            Wall = 0,
            Empty = 1,
            OxygenSystem = 2
        }

        public enum Direction 
        {
            North = 1,
            South = 2,
            West = 3,
            East = 4
        }

        public bool RequestTermination { get; private set; }

        public RepairDroid(bool interactive)
        {
            _interactive = interactive;
            _map = new Dictionary<(long, long), (Entity, long)> {{(0, 0), (Entity.Empty, 0)}};
        }

        public void PrintMap()
        {
            Console.ResetColor();
            WriteLine("------------------------------------");
            for (long y = _minY; y <= _maxY; y ++)
            {
                for (long x = _minX; x <= _maxX; x ++)
                {
                    if ((x, y) == (0, 0))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Write('Δ');
                        Console.ResetColor();
                        continue;
                    }
                    if ((x, y) == _oxygenPosition)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Write('☺');
                        Console.ResetColor();
                        continue;
                    }
                    if ((x, y) == _position)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Write('¤');
                        Console.ResetColor();
                        continue;
                    }
                    if (!_map.TryGetValue((x, y), out var data)) data = (Entity.Unknown, -1);
                    Write(data.entity switch {
                        Entity.Unknown => ' ',
                        Entity.Empty => '.',
                        Entity.Wall => '#',
                        _ => throw new Exception()
                    });
                }

                WriteLine();
            }

            if (_oxygenPosition != null)
            {
                var ox = _oxygenPosition.Value;
                WriteLine($"Oxygen at ({ox.x}, {ox.y}), distance: {_map[ox].distance}");
            }
        }

        public long Output()
        {
            if (_interactive)
            {
                PrintMap();
                WriteLine($"P: ({_position.x}, {_position.y})  D: {_map[_position].distance}");

                _lastMovement = Console.ReadKey().Key switch {
                    ConsoleKey.UpArrow => Direction.North,
                    ConsoleKey.DownArrow => Direction.South,
                    ConsoleKey.LeftArrow => Direction.West,
                    ConsoleKey.RightArrow => Direction.East,
                    _ => 0
                };
            }
            else
            {
                _lastMovement = Move();
            }

            _totalMoves++;
            /*
            if (_totalMoves >= 140)
            {
                var pos = _map[_position];
                var south = _map[(_position.x, _position.y+1)];
                var north = _map[(_position.x, _position.y-1)];
                var west = _map[(_position.x-1, _position.y)];
                var east = _map[(_position.x+1, _position.y)];
                WriteLine($"{_lastMovement}: ({_position.x}, {_position.y}) P:{pos.distance} N:{north.distance} S:{south.distance} W: {west.distance} E:{east.distance}");
            }
            */
            if (_totalMoves >= 2050) RequestTermination = true;

            return (long)_lastMovement;
        }

        private Direction Move()
        {
            if (!_map.TryGetValue((_position.x, _position.y-1), out var north)) return Direction.North;
            if (!_map.TryGetValue((_position.x, _position.y+1), out var south)) return Direction.South;
            if (!_map.TryGetValue((_position.x+1, _position.y), out var east)) return Direction.East;
            if (!_map.TryGetValue((_position.x-1, _position.y), out var west)) return Direction.West;

            var candidates = new List<(Direction direction, long distance)>();
            if (north.distance >= 0) candidates.Add((Direction.North, north.distance));
            if (south.distance >= 0) candidates.Add((Direction.South, south.distance));
            if (west.distance >= 0) candidates.Add((Direction.West, west.distance));
            if (east.distance >= 0) candidates.Add((Direction.East, east.distance));

            var min = (direction: (Direction)0, distance: long.MaxValue);
            foreach (var c in candidates)
            {
                if (min.distance > c.distance)
                {
                    min = c;
                }
            }
            if (min.distance == 0) throw new Exception("No available moves!");

            return min.direction;
        }

        public void ReceiveInput(long input)
        {
            var newPos = _lastMovement switch {
                Direction.North => (x: _position.x, y: _position.y-1),
                Direction.South => (x: _position.x, y: _position.y+1),
                Direction.East => (x: _position.x+1, y: _position.y),
                Direction.West => (x: _position.x-1, y: _position.y),
                _ => _position
            };

            _minX = Math.Min(_minX, newPos.x);
            _maxX = Math.Max(_maxX, newPos.x);
            _minY = Math.Min(_minY, newPos.y);
            _maxY = Math.Max(_maxY, newPos.y);

            var entity = (Entity)input;

            if (entity == Entity.OxygenSystem)
            {
                RequestTermination = true;
                _oxygenPosition = newPos;
            }

            switch (entity)
            {
                case Entity.Wall:
                    _map[newPos] = (entity, -1);
                    break;

                case Entity.OxygenSystem:
                case Entity.Empty:
                    if (!_map.ContainsKey(newPos))
                    {
                        _map[newPos] = (entity, _map[_position].distance+1);
                    }
                    _position = newPos;
                    break;
            }
        }
    }

    public interface IConnector
    {
        long Output();
        void ReceiveInput(long input);
        bool RequestTermination { get; }
    }

    public class Computer
    {
        private Memory _memory;
        private long _jumpTo;
        private Computer _next;
        private InstructionEntity _entity;
        private long _index;
        private IConnector _connector;
        private bool _requestTermination;

        public long LastOutput { get; private set; }
        public long RelativeBase { get; private set; }
        public bool Debug { get; private set; }

        public Computer(Memory memory, IConnector connector)
        {
            _connector = connector;
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
            while(_entity.Instruction != Instruction.Terminate && !_requestTermination)
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
            var output = _connector.Output();
            if (_connector.RequestTermination)
            {
                _requestTermination = true;
            }
            return output;
        }

        public void Output(long output)
        {
            LastOutput = output;
            _connector.ReceiveInput(output);
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