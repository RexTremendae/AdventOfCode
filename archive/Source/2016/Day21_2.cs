using System;
using System.IO;
using System.Linq;
using System.Text;
using static System.Console;

TestClass.TestAll();

var input = "fbgdceah";
foreach (var line in File
    .ReadAllLines("Day21.txt")
    .Where(l => !string.IsNullOrWhiteSpace(l))
    .Reverse())
{
    try
    {
        input = Engine.Execute(line, input, reverse: true);
    }
    catch
    {
        ForegroundColor = ConsoleColor.Red;
        WriteLine("Error!");
        ResetColor();
        WriteLine(line);
        break;
    }
}
WriteLine(input);
WriteLine();

public static class Engine
{
    public static string Execute(string operation, string input, bool reverse = false)
    {
        var parts = operation.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return operation switch
        {
            string op when op.StartsWith("swap position") => SwapPosition(parts, input, reverse),
            string op when op.StartsWith("swap letter") => SwapLetter(parts, input, reverse),
            string op when op.StartsWith("reverse position") => ReversePosition(parts, input, reverse),
            string op when op.StartsWith("rotate left") => RotateLeft(parts, input, reverse),
            string op when op.StartsWith("rotate right") => RotateRight(parts, input, reverse),
            string op when op.StartsWith("rotate based on") => RotateBasedOn(parts, input, reverse),
            string op when op.StartsWith("move position") => MovePosition(parts, input, reverse),
            _ => throw new InvalidOperationException("Unknown operation!")
        };
    }

    private static string RotateBasedOn(string[] operationParts, string input, bool reverse)
    {
        var idx = input.IndexOf(operationParts[6][0]);
        var p = 0;

        if (reverse)
        {
            if (input.Length == 5)
            {
                p = idx switch
                {
                    3 => 3,
                    0 => 4,
                    _ => 0
                };
            }
            else if (input.Length == 8)
            {
                p = idx switch
                {
                    0 => 7,
                    1 => 7,
                    2 => 2,
                    3 => 6,
                    4 => 1,
                    5 => 5,
                    6 => 0,
                    7 => 4,
                    _ => 0
                };
            }
        }
        else
        {
            p = (1 + idx + (idx >= 4 ? 1 : 0)) % input.Length;
        }
        return input[^p..] + input[..^p];
    }

    private static string RotateLeft(string[] operationParts, string input, bool reverse)
    {
        if (reverse) return RotateRight(operationParts, input, false);

        var p = int.Parse(operationParts[2]) % input.Length;
        return input[p..] + input[..p];
    }

    private static string RotateRight(string[] operationParts, string input, bool reverse)
    {
        if (reverse) return RotateLeft(operationParts, input, false);

        var p = int.Parse(operationParts[2]) % input.Length;
        return input[^p..] + input[..^p];
    }

    private static string SwapLetter(string[] operationParts, string input, bool reverse)
    {
        var p1 = operationParts[2][0];
        var p2 = operationParts[5][0];

        return input
            .Replace(p1, '¤')
            .Replace(p2, p1)
            .Replace('¤', p2);
    }

    private static string MovePosition(string[] operationParts, string input, bool reverse)
    {
        var p1 = int.Parse(operationParts[2]);
        var p2 = int.Parse(operationParts[5]);

        if (reverse)
        {
            var tmp = p1;
            p1 = p2;
            p2 = tmp;
        }

        var toMove = input[p1];
        input = input[..p1] + input[(p1+1)..];

        return input[..p2] + toMove + input[(p2)..];
    }

    private static string SwapPosition(string[] operationParts, string input, bool reverse)
    {
        var p1 = int.Parse(operationParts[2]);
        var p2 = int.Parse(operationParts[5]);

        if (p1 > p2)
        {
            var tmp = p2;
            p2 = p1;
            p1 = tmp;
        }

        return input[..p1] + input[p2] + input[(p1+1)..p2] + input[p1] + input[(p2+1)..];
    }

    private static string ReversePosition(string[] operationParts, string input, bool reverse)
    {
        var p1 = int.Parse(operationParts[2]);
        var p2 = int.Parse(operationParts[4]);

        if (p1 > p2)
        {
            var tmp = p2;
            p2 = p1;
            p1 = tmp;
        }

        var reversed = new StringBuilder();
        for (int i = p2; i >= p1; i--)
            reversed.Append(input[i]);
        return input[..p1] + reversed.ToString() + input[(p2+1)..];
    }
}

public static class TestClass
{
    public static
        (int id,  string operation,                        string input,    string expected)  [] TestData = new[]
    {
        (     0,  "swap position 4 with position 0",       "abcde",         "ebcda"),
        (     1,  "swap letter d with letter b",           "ebcda",         "edcba"),
        (     2,  "reverse positions 0 through 4",         "edcba",         "abcde"),
        (     3,  "rotate left 1 step",                    "abcde",         "bcdea"),
        (     4,  "move position 1 to position 4",         "bcdea",         "bdeac"),
        (     5,  "move position 3 to position 0",         "bdeac",         "abdec"),
        (     6,  "rotate based on position of letter b",  "abdec",         "ecabd"),
        (     7,  "rotate based on position of letter d",  "ecabd",         "decab"),
        // ⇈ Test cases from puzzle specification ⇈
        // =========================================
        // ⇊ My home-cooked test cases ⇊
        (     8,  "rotate based on position of letter a",  "abcdefgh",      "habcdefg"),
        (     9,  "rotate based on position of letter b",  "abcdefgh",      "ghabcdef"),
        (    10,  "rotate based on position of letter c",  "abcdefgh",      "fghabcde"),
        (    11,  "rotate based on position of letter d",  "abcdefgh",      "efghabcd"),
        (    12,  "rotate based on position of letter e",  "abcdefgh",      "cdefghab"),
        (    13,  "rotate based on position of letter f",  "abcdefgh",      "bcdefgha"),
        (    14,  "rotate based on position of letter g",  "abcdefgh",      "abcdefgh"),
        (    15,  "rotate based on position of letter h",  "abcdefgh",      "habcdefg"),
        (    16,  "swap position 2 with position 3",       "abcde",         "abdce"),
        (    17,  "reverse positions 1 through 3",         "abcde",         "adcbe"),
        (    18,  "rotate right 7 steps",                  "afgedcbh",      "fgedcbha"),
    };

    public static void TestAll()
    {
        WriteLine();

        var successCount = 0;
        var failureCount = 0;

        foreach (var (id, operation, input, expected) in TestData)
        {
            var actual = "";

            try
            {
                actual = Engine.Execute(operation, input);
            }
            catch (Exception ex)
            {
                actual = $"ERROR! {ex.GetType()}: {ex.Message}";
            }

            if (actual == expected)
            {
                successCount++;
            }
            else
            {
                WriteFailure(id, actual, expected);
                failureCount++;
            }

            // And do the same operation, but reversed...
            var reversedInput = expected;
            var reversedExpected = input;

            try
            {
                actual = Engine.Execute(operation, reversedInput, reverse: true);
            }
            catch (Exception ex)
            {
                actual = $"ERROR! {ex.GetType()}: {ex.Message}";
            }

            if (actual == reversedExpected)
            {
                successCount++;
            }
            else
            {
                WriteFailure(id, actual, reversedExpected, reversed: true);
                failureCount++;
            }
        }

        WriteResult(successCount, failureCount);
        WriteLine();
    }

    private static void WriteResult(int successCount, int failureCount)
    {
        WriteLine();
        if (failureCount == 0)
        {
            Write("All test cases ");
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
