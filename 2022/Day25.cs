using System.Text;
using static System.Console;

public class Day25
{
    public async Task Part1()
    {
        /*
        var filename = "Day25.txt";
        await foreach(var line in System.IO.File.ReadLinesAsync(filename))
        {
        }
        */

        var numbers = new (int dcml, string snafu)[]
        {
            (        1,              "1"),
            (        2,              "2"),
            (        3,             "1="),
            (        4,             "1-"),
            (        5,             "10"),
            (        6,             "11"),
            (        7,             "12"),
            (        8,             "2="),
            (        9,             "2-"),
            (       10,             "20"),
            (       11,             "21"),
            (       12,             "22"),
            (       13,            "1=="),
            (       14,            "1=-"),
            (       15,            "1=0"),
            (       16,            "1=1"),
            (       17,            "1=2"),
            (       18,            "1-="),
            (       19,            "1--"),
            (       20,            "1-0"),
            (       21,            "1-1"),
            (       22,            "1-2"),
            (       23,            "10="),
            (       24,            "10-"),
            (       25,            "100"),
            (     2022,         "1=11-2"),
            (    12345,        "1-0---0"),
            (314159265,  "1121-1110-1=0")
        };

        TestFromDecimal(numbers);
        //TestToDecimal(numbers);
    }

    private void TestToDecimal((int dcml, string snafu)[] numbers)
    {
        foreach (var (dcml, snafu) in numbers)
        {
            var dcmlConverted = SNAFU.ToDecimal(snafu);
            Write($"{snafu}: ");
            if (dcmlConverted == dcml)
            {
                ForegroundColor = ConsoleColor.Green;
                WriteLine(dcmlConverted);
                ResetColor();
            }
            else
            {
                ForegroundColor = ConsoleColor.Red;
                Write(dcmlConverted);
                ResetColor();
                Write($" (should be ");
                ForegroundColor = ConsoleColor.Yellow;
                Write(snafu);
                ResetColor();
                WriteLine(")");
            }
        }
    }

    private void TestFromDecimal((int dcml, string snafu)[] numbers)
    {
        foreach (var (dcml, snafu) in numbers)
        {
            var snafuConverted = SNAFU.FromDecimal(dcml);
            Write($"{dcml}: ");
            if (snafuConverted == snafu)
            {
                ForegroundColor = ConsoleColor.Green;
                WriteLine(snafuConverted);
                ResetColor();
            }
            else
            {
                ForegroundColor = ConsoleColor.Red;
                Write(snafuConverted);
                ResetColor();
                Write($" (should be ");
                ForegroundColor = ConsoleColor.Yellow;
                Write(snafu);
                ResetColor();
                WriteLine(")");
            }
        }
    }

    public static class SNAFU
    {
        public static long ToDecimal(string value)
        {
            var five = 1L;
            var sum = 0L;
            foreach (var ch in value.Reverse())
            {
                sum += five * ch switch {
                    '2' => 2,
                    '1' => 1,
                    '-' => -1,
                    '=' => -2,
                    _ => 0
                };

                five *= 5;
            }

            return sum;
        }

        public static string FromDecimal(long value)
        {
            var builder = new StringBuilder("0");

            var leftOver = value % 5;
            while (value > 0)
            {
                var curr = (value+2)/5;
                value/=5;
                if (curr < 3)
                {
                    builder.Append(curr.ToString());
                }
                else
                {
                    builder.Append(curr == 4 ? '-' : '=');
                }
                //builder.Append(mod);
            }

            if (leftOver != 0)
            {
                builder.Append(leftOver switch
                {
                    var lo when lo == 4 => '-',
                    var lo when lo == 3 => '=',
                    _ => leftOver.ToString()
                });
            }

            return builder.ToString().TrimStart('0');
        }
    }
}
