using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using static System.Console;

TestClass.TestAll();

var code = new List<string>();
foreach (var line in File
    .ReadAllLines("Day23.txt")
    .Where(l => !string.IsNullOrWhiteSpace(l)))
{
    code.Add(line);
}
var program = AssembunnyProgram.Load(code.ToArray(), 7);
AssembunnyInterpretor.Execute(program);
WriteLine(program.GetRegister(Register.A));

public enum Register { A, B, C, D }
public enum Instruction { Cpy, Inc, Dec, Jnz, Tgl, Terminate }

public class AssembunnyProgram
{
    private readonly List<(Instruction, object[] args, bool toggled)> _code = new();
    private readonly Dictionary<Register, int> _registers = new ();

    public int GetRegister(Register reg) => _registers[reg];
    public void SetRegister(Register reg, int value) => _registers[reg] = value;

    public void Toggle(int ptr)
    {
        if (ptr <= 0 || ptr >= _code.Count) return;

        (var instruction, var args, _) = _code[ptr];
        if (args.Length == 1)
        {
            var toggled = (instruction == Instruction.Inc
                ? Instruction.Dec
                : Instruction.Inc);

            _code[ptr] = (toggled, args, true);
        }
        else if (args.Length == 2)
        {
            var toggled = (instruction == Instruction.Jnz
                ? Instruction.Cpy
                : Instruction.Jnz);

            _code[ptr] = (toggled, args, true);
        }
    }

    public (Instruction, object[] args, bool toggled) GetInstruction(int index)
    {
        if (index >= _code.Count)
        {
            return (Instruction.Terminate, new string[] { }, false);
        }

        return _code[index];
    }

    public static AssembunnyProgram Load(params string[] code)
    {
        return Load(code, 0);
    }

    public static AssembunnyProgram Load(string[] code, int a)
    {
        var program = new AssembunnyProgram();

        program._registers[Register.A] = a;
        program._registers[Register.B] = 0;
        program._registers[Register.C] = 0;
        program._registers[Register.D] = 0;

        foreach (var line in code)
        {
            var parts = line.Split(' ');
            if (!Enum.TryParse<Instruction>(parts[0], true, out var instruction))
            {
                throw new InvalidOperationException($"Invalid instruction: {parts[0]}.");
            }

            List<object> arguments = new();
            parts = parts[1..].Select(p => p.ToUpper()).ToArray();

            switch (instruction)
            {
                case Instruction.Cpy:
                {
                    ValidateInstruction(Instruction.Cpy, 2, parts);

                    if (int.TryParse(parts[0], out var value))
                    {
                        arguments.Add(value);
                    }
                    else if (TryParseRegister(parts[0], out var srcRegister))
                    {
                        arguments.Add(srcRegister);
                    }
                    else
                    {
                        throw new ArgumentException(message: $"{instruction} instruction was given an invalid first argument: '{parts[0]}'");
                    }

                    if (TryParseRegister(parts[1], out var dstRegister))
                    {
                        arguments.Add(dstRegister);
                    }
                    else
                    {
                        throw new ArgumentException(message: $"{instruction} instruction was given an invalid second argument: '{parts[1]}'");
                    }

                    break;
                }

                case Instruction.Inc:
                {
                    ValidateInstruction(Instruction.Inc, 1, parts);

                    if (TryParseRegister(parts[0], out var dstRegister))
                    {
                        arguments.Add(dstRegister);
                    }
                    else
                    {
                        throw new ArgumentException(message: $"{instruction} instruction was given an invalid argument: '{parts[0]}'");
                    }

                    break;
                }

                case Instruction.Dec:
                {
                    ValidateInstruction(Instruction.Dec, 1, parts);

                    if (TryParseRegister(parts[0], out var dstRegister))
                    {
                        arguments.Add(dstRegister);
                    }
                    else
                    {
                        throw new ArgumentException(message: $"{instruction} instruction was given an invalid argument: '{parts[0]}'");
                    }

                    break;
                }

                case Instruction.Jnz:
                {
                    ValidateInstruction(Instruction.Jnz, 2, parts);

                    if (int.TryParse(parts[0], out var value))
                    {
                        arguments.Add(value);
                    }
                    else if (TryParseRegister(parts[0], out var cmprRegister))
                    {
                        arguments.Add(cmprRegister);
                    }
                    else
                    {
                        throw new ArgumentException(message: $"{instruction} instruction was given an invalid first argument: '{parts[0]}'");
                    }

                    if (int.TryParse(parts[1], out var steps))
                    {
                        arguments.Add(steps);
                    }
                    else if (TryParseRegister(parts[1], out var jmpRegister))
                    {
                        arguments.Add(jmpRegister);
                    }
                    else
                    {
                        throw new ArgumentException(message: $"{instruction} instruction was given an invalid second argument: '{parts[1]}'");
                    }

                    break;
                }

                case Instruction.Tgl:
                {
                    ValidateInstruction(Instruction.Tgl, 1, parts);

                    if (TryParseRegister(parts[0], out var dstRegister))
                    {
                        arguments.Add(dstRegister);
                    }
                    else
                    {
                        throw new ArgumentException(message: $"{instruction} instruction was given an invalid argument: '{parts[0]}'");
                    }

                    break;
                }
            }

            program._code.Add((instruction, arguments.ToArray(), false));
        }

        return program;
    }

    private static void ValidateInstruction(Instruction instruction, int argCount, IEnumerable<object> args)
    {
        if (args.Count() != argCount)
        {
            throw new ArgumentException(message: $"{instruction} instruction requires exactly {argCount} parameters.");
        }
    }

    private static bool TryParseRegister(string arg, out Register register)
    {
        return Enum.TryParse<Register>(arg, out register);
    }
}

public class AssembunnyInterpretor
{
    private static AssembunnyProgram _program;

    public static void Execute(AssembunnyProgram program)
    {
        _program = program;
        int ptr = 0;

        var instruction = Instruction.Cpy;
        while (instruction != Instruction.Terminate)
        {
            object[] args;
            bool toggled;
            (instruction, args, toggled) = program.GetInstruction(ptr);

            try
            {
            switch (instruction)
            {
                case Instruction.Cpy:
                {
                    var value = GetValue(args[0]);
                    program.SetRegister((Register)args[1], value);
                    break;
                }

                case Instruction.Inc:
                {
                    var reg = (Register)args[0];
                    program.SetRegister(reg, program.GetRegister(reg)+1);
                    break;
                }

                case Instruction.Dec:
                {
                    var reg = (Register)args[0];
                    program.SetRegister(reg, program.GetRegister(reg)-1);
                    break;
                }

                case Instruction.Jnz:
                {
                    var cmprValue = GetValue(args[0]);
                    if (cmprValue != 0)
                    {
                        ptr += GetValue(args[1]) - 1;
                    }
                    break;
                }

                case Instruction.Terminate:
                    // Do nothing, just terminate...
                    break;

                case Instruction.Tgl:
                    program.Toggle(ptr + GetValue(args[0]));
                    break;

                default:
                    throw new InvalidOperationException($"Invalid instruction '{instruction}'");
            }
            }
            catch (Exception)
            {
                if (!toggled) throw;
            }
            ptr++;
        }
    }

    private static int GetValue(object arg)
    {
        return (arg is Register reg)
            ? _program.GetRegister(reg)
            : (int)arg;
    }
}

public class TestClass
{
    public void Cpy1()
    {
        // Arrange
        var program = AssembunnyProgram.Load(
            "cpy 34 a"
        );

        // Act
        AssembunnyInterpretor.Execute(program);

        // Assert
        program.GetRegister(Register.A).ShouldBe(34);
    }

    public void Cpy2()
    {
        // Arrange
        var program = AssembunnyProgram.Load(
            "cpy 37 a",
            "cpy a b"
        );

        // Act
        AssembunnyInterpretor.Execute(program);

        // Assert
        program.GetRegister(Register.B).ShouldBe(37);
    }

    public void Inc()
    {
        // Arrange
        var program = AssembunnyProgram.Load(
            "inc c"
        );

        // Act
        AssembunnyInterpretor.Execute(program);

        // Assert
        program.GetRegister(Register.C).ShouldBe(1);
    }

    public void Dec()
    {
        // Arrange
        var program = AssembunnyProgram.Load(
            "dec d"
        );

        // Act
        AssembunnyInterpretor.Execute(program);

        // Assert
        program.GetRegister(Register.D).ShouldBe(-1);
    }

    public void Jnz1()
    {
        // Arrange
        var program = AssembunnyProgram.Load(
            "jnz a 2",
            "inc b"
        );

        // Act
        AssembunnyInterpretor.Execute(program);

        // Assert
        program.GetRegister(Register.B).ShouldBe(1);
    }

    public void Jnz2()
    {
        // Arrange
        var program = AssembunnyProgram.Load(
            "cpy 1 a",
            "cpy 2 b",
            "jnz a b",
            "dec b"
        );

        // Act
        AssembunnyInterpretor.Execute(program);

        // Assert
        program.GetRegister(Register.B).ShouldBe(2);
    }

    public void SmallSample()
    {
        // Arrange
        var program = AssembunnyProgram.Load(
            "cpy 2 a",
            "tgl a",
            "tgl a",
            "tgl a",
            "cpy 1 a",
            "dec a",
            "dec a"
        );

        // Act
        AssembunnyInterpretor.Execute(program);

        // Assert
        program.GetRegister(Register.A).ShouldBe(3);
    }

    public static void TestAll()
    {
        WriteLine();

        var successCount = 0;
        var failureCount = 0;

        foreach (var method in typeof(TestClass)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
        {
            var testClass = new TestClass();
            try
            {
                WriteLine($"Executing test '{method.Name}'...");
                method.Invoke(testClass, new Type[] {});
                successCount++;
            }
            catch (Exception ex)
            {
                failureCount++;
                Write($"{method.Name}(): ");
                ForegroundColor = ConsoleColor.Red;
                WriteLine(ex.InnerException.Message);
                ResetColor();
            }
        }

        WriteResult(successCount, failureCount);
    }

    private static void WriteResult(int successCount, int failureCount)
    {
        if (failureCount == 0)
        {
            ForegroundColor = ConsoleColor.Green;
            Write(successCount);
            ResetColor();
            Write(" test cases ");
            ForegroundColor = ConsoleColor.Green;
            WriteLine("succeeded!");
            ResetColor();
        }
        else
        {
            ForegroundColor = ConsoleColor.Green;
            Write(successCount);
            ResetColor();
            Write(" out of ");
            ForegroundColor = ConsoleColor.Cyan;
            Write(successCount + failureCount);
            ResetColor();
            WriteLine(" test cases succeeded. ");

            ForegroundColor = ConsoleColor.Red;
            Write(failureCount);
            ResetColor();
            WriteLine(" failed!");
        }
    }

    private static void WriteFailure(int id, string actual, string expected, bool reversed = false)
    {
        Write("Test case ");
        ForegroundColor = ConsoleColor.Cyan;
        Write($"#{id} ");
        if (reversed) Write("(reversed) ");
        ForegroundColor = ConsoleColor.Red;
        Write("failed!");
        ResetColor();
        Write(" Expected '");
        ForegroundColor = ConsoleColor.Green;
        Write(expected);
        ResetColor();
        Write("' but got '");
        ForegroundColor = ConsoleColor.Red;
        Write(actual);
        ResetColor();
        WriteLine("'");
    }
}

public static class Extensions
{
    public static void ShouldBe(this object actual, object expected)
    {
        if (!actual.Equals(expected))
        {
            throw new Exception($"Expected {expected.ToString()}, but got {actual.ToString()}.");
        }
    }
}
