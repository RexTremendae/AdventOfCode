using System;
using System.IO;
using System.Linq;
using System.Text;
using static System.Console;

TestClass.TestAll();

var sum = 0L;
foreach (var line in File.ReadAllLines("Day18.txt"))
{
    sum += Evaluator.Evaluate(line);
}
WriteLine(sum);

public static class Evaluator
{
    public static long Evaluate(string expression)
    {
        var pStart = 0;
        for(;;)
        {
            pStart = expression.LastIndexOf('(');
            if (pStart < 0) break;
            var pEnd = expression.IndexOf(')', pStart);
            var value = EvaluateNoBrackets(expression[(pStart+1)..pEnd]);
            expression = expression[..pStart] + value.ToString() + expression[(pEnd+1)..];
        }
        return EvaluateNoBrackets(expression);
    }

    private static long EvaluateNoBrackets(string expression)
    {
        var result = 0L;
        var parts = expression.Split(' ').ToList();

        for (;;)
        {
            var firstAddIdx = -1;
            var firstMulIdx = -1;
            for (int i = 0; i < parts.Count; i++)
            {
                if (firstAddIdx < 0 && parts[i] == "+") firstAddIdx = i;
                if (firstMulIdx < 0 && parts[i] == "*") firstMulIdx = i;
            }

            if (firstAddIdx > 0)
            {
                var longVal =
                    long.Parse(parts[firstAddIdx-1]) +
                    long.Parse(parts[firstAddIdx+1]);

                parts.RemoveRange(firstAddIdx-1, 3);
                parts.Insert(firstAddIdx-1, longVal.ToString());
            }
            else if (firstMulIdx > 0)
            {
                var longVal =
                    long.Parse(parts[firstMulIdx-1]) *
                    long.Parse(parts[firstMulIdx+1]);

                parts.RemoveRange(firstMulIdx-1, 3);
                parts.Insert(firstMulIdx-1, longVal.ToString());
            }
            else
            {
                result = long.Parse(parts[0]);
                break;
            }
        }

        return result;
    }
}

public static class TestClass
{
    public static
        (int id, string expression, long expected) [] TestData = new[]
    {
        (1, "1 + (2 * 3) + (4 * (5 + 6))", 51L),
        (2, "2 * 3 + (4 * 5)", 46L),
        (3, "5 + (8 * 3 + 9 + 3 * 4 * 3)", 1445L),
        (4, "5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))", 669060L),
        (5, "((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2", 23340L)
    };

    public static void TestAll()
    {
        WriteLine();

        var successCount = 0;
        var failureCount = 0;

        foreach (var (id, expression, expected) in TestData)
        {
            var actual = 0L;

            try
            {
                actual = Evaluator.Evaluate(expression);
                if (actual == expected)
                {
                    successCount++;
                }
                else
                {
                    failureCount++;
                    WriteFailure(id: id, actual: actual, expected: expected);
                }
            }
            catch (Exception ex)
            {
                failureCount++;
                Write($"Test case #");
                ForegroundColor = ConsoleColor.Cyan;
                Write(id);
                ResetColor();
                Write(": ");
                ForegroundColor = ConsoleColor.Red;
                WriteLine(ex?.Message);
                WriteLine(ex.StackTrace);
                ResetColor();
            }
        }

        WriteResult(successCount, failureCount);
    }

    private static void WriteResult(int successCount, int failureCount)
    {
        WriteLine();

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
            WriteLine(" test cases succeeded, ");

            ForegroundColor = ConsoleColor.Red;
            Write(failureCount);
            ResetColor();
            WriteLine(" failed!");
        }

        WriteLine();
    }

    private static void WriteFailure(int id, long actual, long expected)
    {
        Write("Test case ");
        ForegroundColor = ConsoleColor.Cyan;
        Write($"#{id} ");
        ForegroundColor = ConsoleColor.Red;
        Write("failed!");
        ResetColor();
        Write(" Expected ");
        ForegroundColor = ConsoleColor.Green;
        Write(expected);
        ResetColor();
        Write(" but got ");
        ForegroundColor = ConsoleColor.Red;
        WriteLine(actual);
        ResetColor();
    }
}
